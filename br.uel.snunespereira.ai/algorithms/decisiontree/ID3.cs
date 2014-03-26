using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using br.uel.snunespereira.ai.shared;
using br.uel.snunespereira.ai.algorithms.decisiontree;
using System.Collections;
using br.uel.snunespereira.ai.algorithms.decisiontree.structure;

namespace br.uel.snunespereira.ai.algorithms.decisiontree
{
    /// <summary>
    /// ID3 (+ Continous range handling) Decision Tree Algorithm
    /// </summary>
    public class ID3 : AlgorithmBase
    {
        /// <summary>
        /// Method that executes the process
        /// </summary>
        /// <returns>StringBuilder containing the results</returns>
        public static StringBuilder Execute(string[] data)
        {
            // creates a string builder
            StringBuilder execution = new StringBuilder();

            int size = data.Length - data.ToList().FindIndex(m => m.ToString().Trim().ToLower() == "@data");
            List<shared.Attribute> attributeList = AlgorithmBase.GetAttributes(data);

            // print some info
            execution.AppendLine();
            execution.AppendLine();
            execution.AppendFormat("The file has {0} entries", size);
            execution.AppendLine();
            execution.AppendLine("The attributes are: ");

            // for each founded attribute
            foreach (shared.Attribute att in attributeList)
                execution.AppendFormat("- {0}\n", att.Name);

            // format the data to usage
            string[][] filteredData =
                data.Skip(data.ToList().FindIndex(m => m.ToString().Trim().ToLower() == "@data") + 1)
                    .ToList()
                        .ConvertAll<string[]>(i => i.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries) ).ToArray();

            int trainingLength = (int)(filteredData.Length * 75 / 100);
            string[][] trainingData = filteredData.Take(trainingLength).ToArray();
            string[][] testData = filteredData.Skip(trainingLength).ToArray();

			TreeNode root = MountTree(attributeList.Take(attributeList.Count() - 1).ToArray(), attributeList.Last(), trainingData);

            execution.AppendLine();
            execution.AppendLine("The tree is:");

            PrintTree("", execution, root);

            int success = 0, failure = 0;

            foreach (var line in testData)
            {
                if (TestTree(root, line, attributeList.Last()))
                    success++;
                else
                    failure++;
            }

            execution.AppendLine();
            execution.AppendFormat("Successful: {0}\n", success);
            execution.AppendFormat("Failure: {0}\n", failure);
            execution.AppendFormat("Rate: {0}%\n", Math.Ceiling((double)success / (double)testData.Length * 100));

            return execution;
        }

        /// <summary>
        /// Method responsible to test the created tree
        /// </summary>
        /// <param name="node">Current TreeNode</param>
        /// <param name="line">Line of data to be tested</param>
        /// <param name="categoricalAttribute">The categorical attribute</param>
        /// <returns>true or false</returns>
        private static bool TestTree(TreeNode node, string[] line, shared.Attribute categoricalAttribute)
        {
            // if current node is categorical node
            if (node.Attribute.Name == categoricalAttribute.Name)
            {
                // return the positive or negative classification
                return node.Attribute.Values.First().Equals(line[categoricalAttribute.Index].Trim());
            }
            else
            {
                // if node type is real
                if (node.Attribute.Type == typeof(double))
                {
                    int index = 0;

                    // for each attribute value
                    foreach (string item in node.Attribute.Values)
                    {
                        // identify the token
                        if (item.IndexOf('<') > -1)
                        {
                            // makes the comparison
                            if (double.Parse(line[node.Attribute.Index]) < double.Parse(item.Replace("<", "")))
                                break; // item found, breaks the loop
                        } 
                        else
                        {
                            // makes the comparison
                            if (double.Parse(line[node.Attribute.Index]) >= double.Parse(item.Replace("=>", "")))
                                break; // item found, breaks the loop
                        }

                        index++;
                    }

                    // recalls the method, passing the correct node
                    return TestTree(node.Childs[index], line, categoricalAttribute);
                }
                else // or node has a domain
                {
                    int index = 0;

                    // for each attribute value
                    foreach (string item in node.Attribute.Values)
                    {
                        // makes the comparison
                        if (item.Trim() == line[node.Attribute.Index].Trim())
                            break; // item found, breaks the loop
                        index++;
                    }

                    // recalls the method, passing the correct node
                    return TestTree(node.Childs[index], line, categoricalAttribute);
                }
            }
        }

        /// <summary>
        /// Method responsible for print the output tree
        /// </summary>
        /// <param name="level">string indicanting the current level</param>
        /// <param name="execution">Output StringBuilder</param>
        /// <param name="node">Current TreeNode</param>
        private static void PrintTree(string level, StringBuilder execution, TreeNode node)
        {
            int index = 0;

            // select all nodes but the failure ones
            TreeNode[] childNodes = node.Childs.Where(m => m.Attribute.Name != "Failure").ToArray();

            // if node has any child
            if (childNodes.Length > 0)
            {
                // for each child in current node
                foreach (TreeNode childs in childNodes)
                {
                    // prints the results
                    execution.AppendLine(level + node.Attribute.Name + ":" + node.Attribute.Values[index]);

                    // recall the method, for each child
                    PrintTree(level + "-", execution, childs);

                    index++;
                }
            }
            else
            {
                // prints the results
                execution.AppendLine(level + node.Attribute.Name + ":" + node.Attribute.Values[index]);
            }
        }

        /// <summary>
        /// Method resposible for build up a tree
        /// </summary>
        /// <param name="attributeList">Array of attributes</param>
        /// <param name="currentAttribute">Categorized Attribute</param>
        /// <param name="data">Array of data</param>
        /// <returns>A single node</returns>
        private static TreeNode MountTree(shared.Attribute[] attributeList, 
                                            shared.Attribute categoricalAttribute, 
                                            string[][] data)
        {
            // if data is empty, return a single node with value failure
            if (data.Length == 0)
                return new TreeNode(new shared.Attribute(typeof(string), "Failure", -1, new string[] { "Failure" }));

            var groupingData = data.GroupBy(m => m[categoricalAttribute.Index]);

            /*
             * if data consists of records all with the same value for 
             * the categorical attribute, 
             * return a single node with that value
             */
            if (groupingData.Count() == 1)
                return new TreeNode(new 
                    shared.Attribute(
                        categoricalAttribute.Type,
                        categoricalAttribute.Name,
                        categoricalAttribute.Index, 
                        new string[] { groupingData.First().Key }));

            /* 
             * if attributeList is empty, the return a single node with as value
             * the most frequent of the values of the categorical attribute 
             * that are found in the records of data
             */
            if (attributeList.Length == 0)
                return new TreeNode(GetMostFrequentValue(categoricalAttribute, data));

            /*
             * Let bestAttribute be the attribute with the largest gain 
             * among attributes in listAttributes
             */
            shared.Attribute bestAttribute 
                = GetAttributeLargestGain(attributeList, categoricalAttribute, data);

            // creates a root
            TreeNode root = new TreeNode(bestAttribute);
            
            // creates a new attribute list
            shared.Attribute[] newAttributeList = attributeList.Where(m => m.Name.Trim() != bestAttribute.Name.Trim()).ToArray();

            // for each value in attributes
            foreach (string value in root.Attribute.Values)
            {
                // if node type is continous range
                if (root.Attribute.Type == typeof(double))
                {
                    string[][] newData = null;

                    // gets the new data
                    if (value.IndexOf('<') > -1)
                        newData = data.Where(m => double.Parse(m[bestAttribute.Index].Trim()) < double.Parse(value.Trim().Replace("<", ""))).ToArray();
                    else
                        newData = data.Where(m => double.Parse(m[bestAttribute.Index].Trim()) >= double.Parse(value.Trim().Replace("=", "").Replace(">", ""))).ToArray();

                    // calls again
                    TreeNode childContinous = MountTree(newAttributeList, categoricalAttribute, newData);
                    root.Childs.Add(childContinous);
                }
                else // or node has a domain
                {
                    // gets the new data
                    string[][] newData = data.Where(m => m[bestAttribute.Index].Trim() == value.Trim()).ToArray();

                    // calls again
                    TreeNode child = MountTree(newAttributeList, categoricalAttribute, newData);
                    root.Childs.Add(child);
                }
            }

            return root;
        }

        /// <summary>
        /// Method responsible for retrieve the most frequent value in data
        /// </summary>
        /// <param name="currentAttribute">Categorized Attribute</param>
        /// <param name="data">Array of data</param>
        /// <returns>Attribute</returns>
        private static shared.Attribute GetMostFrequentValue(shared.Attribute categoricalAttribute, string[][] data)
        {
            // group data
            var groupingData = data.GroupBy(m => m[categoricalAttribute.Index]);

            // gets the maximum value
            string maxValue = groupingData.OrderByDescending(m => m.Count()).First().Key;

            return new shared.Attribute(categoricalAttribute.Type,
                                        categoricalAttribute.Name,
                                        categoricalAttribute.Index,
                                        new string[] { maxValue });
        }

        /// <summary>
        /// Method responsible for select the best attribute, based on information gain
        /// </summary>
        /// <param name="attributeList">Array of attributes</param>
        /// <param name="categoricalAttribute">The categorical attribute</param>
        /// <param name="data">Array of training data</param>
        /// <returns>Best attribute</returns>
        private static shared.Attribute GetAttributeLargestGain(shared.Attribute[] attributeList, 
                                                                  shared.Attribute categoricalAttribute,
                                                                  string[][] data)
        {
            // creates a list that stores the attribute and its information gain
            List<Tuple<shared.Attribute, double>> listInformationGain = new List<Tuple<shared.Attribute, double>>();

            // for each attribute
            foreach (shared.Attribute attribute in attributeList)
            {
                List<double> entropyList = new List<double>();

                // if the attribute has a domain
                if (attribute.Type == typeof(string))
                {
                    // for each value to the attribute
                    foreach(string value in attribute.Values)
                    {
                        // gets entropy and it is proportion
                        double proportion = 0;
                        double entropy = GetEntropy(attribute, categoricalAttribute, value, data, out proportion);

                        // add the entropy to entropyList
                        entropyList.Add(entropy * proportion);
                    }

                    // the information gain is 1.0 minus the sum of all entropies
                    listInformationGain.Add(new Tuple<shared.Attribute, double>(attribute, 1D - entropyList.Sum()));
                }
                else
                {
                    // clear all possible values
                    attribute.Values = new List<string>();

                    // gets a distinct ordered list from data
                    double[] orderedValues = data.ToList().OrderBy(m => double.Parse(m[attribute.Index]))
                        .ToList().ConvertAll<double>(m => double.Parse(m[attribute.Index])).Distinct().ToArray();

                    List<double> continousEntropyList = new List<double>();
                    List<Tuple<string, double>> continousGainList = new List<Tuple<string, double>>();

                    // for each distinct value in data
                    foreach (double item in orderedValues)
                    {
                        // for each value to that specific partition (first partition)
                        foreach (double line in orderedValues.Where(m => m < item).ToArray())
                        {
                            // gets entropy and it is proportion
                            double proportion = 0;
                            double entropy = GetEntropy(attribute, categoricalAttribute, line.ToString(), data, out proportion);

                            // add the entropy to entropyList
                            continousEntropyList.Add(entropy * proportion);
                        }

                        // the information gain is 1.0 minus the sum of all entropies
                        if (continousEntropyList.Sum() > 0)
                            continousGainList.Add(new Tuple<string, double>("<" + item, 1D - continousEntropyList.Sum()));

                        continousEntropyList.Clear();

                        // for each value to that specific partition (last partition)
                        foreach (double line in orderedValues.Where(m => m >= item).ToArray())
                        {
                            // gets entropy and it is proportion
                            double proportion = 0;
                            double entropy = GetEntropy(attribute, categoricalAttribute, line.ToString(), data, out proportion);

                            // add the entropy to entropyList
                            continousEntropyList.Add(entropy * proportion);
                        }

                        // the information gain is 1.0 minus the sum of all entropies
                        if (continousEntropyList.Sum() > 0)
                            continousGainList.Add(new Tuple<string, double>("=>" + item, 1D - continousEntropyList.Sum()));
                    }

                    // if there is any item in gain list
                    if (continousGainList.Count() > 0)
                    {
                        // gets the maximum gain
                        var maxGainContinuous = continousGainList.OrderByDescending(m => m.Item2).First();

                        // add value to the current attribute
                        attribute.Values.Add(maxGainContinuous.Item1);

                        // add the another partition to values as well
                        if (maxGainContinuous.Item1.IndexOf("=>") > -1)
                            attribute.Values.Add(maxGainContinuous.Item1.Replace("=>", "<"));
                        else
                            attribute.Values.Add(maxGainContinuous.Item1.Replace("<", "=>"));

                        // the information gain is 1.0 minus the sum of all entropies
                        listInformationGain.Add(new Tuple<shared.Attribute, double>(attribute, maxGainContinuous.Item2));
                    }
                }
            }

            // gets the max information gain element
            if (listInformationGain.Count() > 0)
                return listInformationGain.OrderByDescending(m => m.Item2).First().Item1;
            else
                return attributeList.First();
        }

        /// <summary>
        /// Method responsible to calculte the entropy for some attribute
        /// </summary>
        /// <param name="attribute">The attribute used to calculate the entropy</param>
        /// <param name="categoricalAttribute">The categorical attribute</param>
        /// <param name="value">Current value</param>
        /// <param name="data">Array of training data</param>
        /// <param name="proportion">Output containing the proprortional value in whole set</param>
        /// <returns>Entropy</returns>
        private static double GetEntropy(shared.Attribute attribute, 
                                           shared.Attribute categoricalAttribute, 
                                           string value, 
                                           string[][] data,
                                           out double proportion)
        {
            // declare variables
            double totalCountValue = 0;
            double totalFirstCategoricalValue = 0;
            
            // if we are working with continous range
            if (value == null)
            {
                // gets the total count of current value
                totalCountValue = data.Length;
                // gets the total of positive itens in current value
                totalFirstCategoricalValue = data.Where(m => m[categoricalAttribute.Index].Trim() == categoricalAttribute.Values.Last().Trim()).Count();
            }
            else 
            {
                // gets the total count of current value
                totalCountValue = data.Where(m => m[attribute.Index].Trim() == value.Trim()).Count();
                // gets the total of positive itens in current value
                totalFirstCategoricalValue = data.Where(m => m[attribute.Index].Trim() == value.Trim()
                                                          && m[categoricalAttribute.Index].Trim() == categoricalAttribute.Values.Last().Trim()).Count();
            }

            // gets the proportion
            proportion = totalCountValue / data.Length;

            // if there is only positive or negative examples returns entropy 0
            if (totalFirstCategoricalValue == 0 || totalCountValue - totalFirstCategoricalValue == 0)
            {
                return 0;
            }
            else
            {
                /* 
                 * return H(value) = - (positive values / total values) * log2 (positive values / total values)
                 *                   - (negative values / total values) * log2 (negative values / total values)
                 */
                return -(totalFirstCategoricalValue / totalCountValue) * Math.Log(totalFirstCategoricalValue / totalCountValue)
                   - ((totalCountValue - totalFirstCategoricalValue) / totalCountValue) * Math.Log((totalCountValue - totalFirstCategoricalValue) / totalCountValue);
            }
        }
    }
}
