using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace br.uel.snunespereira.ai.shared
{
    /// <summary>
    /// Enumerator that defines which algorithm will be used
    /// </summary>
    public enum AlgorithmType
    {
        DecisionTree = 1,
        BackPropagation = 2,
        SVM = 3,
        Undefined = 0
    }
}
