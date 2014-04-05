using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace br.uel.snunespereira.ai.algorithms.decisiontree.structure
{
    public class TreeNode
    {
        /// <summary>
        /// 
        /// </summary>
        public shared.Attribute Attribute { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<TreeNode> Childs { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="attribute"></param>
        public TreeNode(shared.Attribute attribute)
        {
            this.Attribute = attribute;
            this.Childs = new List<TreeNode>();
        }
    }
}
