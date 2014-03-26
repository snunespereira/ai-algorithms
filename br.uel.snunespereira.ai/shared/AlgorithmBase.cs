using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace br.uel.snunespereira.ai.shared
{
    /// <summary>
    /// Base class that supports other algorithms classes
    /// </summary>
    public abstract class AlgorithmBase
    {
        /// <summary>
        /// Method responsible for identifing the atribute list in data
        /// </summary>
        /// <param name="data">Array of Data</param>
        /// <returns>List of attributes</returns>
        protected static List<shared.Attribute> GetAttributes(string[] data)
        {
            List<shared.Attribute> attributeList = new List<shared.Attribute>();
            int index = 0;

            // for each attribute found
            foreach (string line in data.Where(m => m.ToLower().IndexOf("@attribute") > -1))
            {
                string[] stringParts = line.Replace("\t", " ").Split(' ');

                // the attribute has a domain
                if (line.IndexOf('{') > -1)
                {
                    string values = line.Substring(line.IndexOf('{')).Replace("{", string.Empty).Replace("}", string.Empty).Trim();
                    attributeList.Add(new shared.Attribute(typeof(string), stringParts.Skip(1).Take(1).First(), index, values.Split(',')));
                }
                else
                {
                    attributeList.Add(
                        new shared.Attribute(Type.GetType("System." + stringParts.Last().ToLower().Replace("real", "Double")),
                        stringParts.Skip(1).Take(1).First(), index));
                }

                index++;
            }

            return attributeList;
        }
    }
}
