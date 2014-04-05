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

					List<string> formattedValues = values.Split (',').ToList ();
					for (int x = 0; x < formattedValues.Count (); x++)
						formattedValues [x] = formattedValues [x].Trim ();

					attributeList.Add(new shared.Attribute(typeof(string), stringParts.Skip(1).Take(1).First(), index, formattedValues.ToArray()));
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
    
		/// <summary>
		/// Crossfolds the data.
		/// </summary>
		/// <returns>The data.</returns>
		/// <param name="data">Data.</param>
		/// <param name="folds">Folds.</param>
		protected static List<string[]> CrossfoldData(string[] data, int folds)
		{
			// gets the maximum lines per fold
			int linesPerFold = data.Length / folds;

			// creates a list the contains the folded data
			List<string[]> returningList = new List<string[]> ();

			// creates a lists that contains used positions
			List<int> usedPositions = new List<int> ();

			// creates a random value
			Random rnd = new Random ();

			// for each fold
			for(int fold = 0; fold < folds; fold++) {

				// create a list of lines container
				List<string> linesInFold = new List<string> ();

				// for each line in fold
				for(int line = 0; line < linesPerFold; line++) {

					// gets the next value
					int position = rnd.Next (0, data.Length);

					// while this position it is used
					while (usedPositions.Contains (position)) {
						position = rnd.Next (0, data.Length); // gets another position
					}

					// add the line in line fold array
					linesInFold.Add (data [position]);

					// saves the position
					usedPositions.Add (position);
				}

				// the array of selected lines is saved in current fold
				returningList.Add (linesInFold.ToArray ());
			}

			// return the folded list
			return returningList;
		}
	}
}
