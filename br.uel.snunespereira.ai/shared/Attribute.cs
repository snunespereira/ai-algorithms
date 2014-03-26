using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace br.uel.snunespereira.ai.shared
{
    /// <summary>
    /// Class that represents an attribute
    /// </summary>
    public class Attribute
    {
        /// <summary>
        /// Property Type
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// Property Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Property Values
        /// </summary>
        public List<string> Values { get; set; }

        /// <summary>
        /// Property Index
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type">Type</param>
        /// <param name="name">Attribute name</param>
        /// <param name="index">Attribute index in data</param>
        public Attribute(Type type, string name, int index)
        {
            this.Type = type;
            this.Name = name;
            this.Index = index;
            this.Values = new List<string>();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type">Type</param>
        /// <param name="name">Attribute name</param>
        /// <param name="index">Attribute index in data</param>
        /// <param name="values">Array of possible values</param>
        public Attribute(Type type, string name, int index, string[] values)
        {
            // trim all the strings
            values.ToList().ForEach(m => m = m.Trim());
            values = values.OrderBy(m => m.Trim()).ToArray();

            this.Values = new List<string>();
            this.Values.AddRange(values);
            this.Type = type;
            this.Name = name;
            this.Index = index;
        }
    }
}
