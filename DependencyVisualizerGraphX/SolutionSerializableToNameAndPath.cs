using SolutionDependencyScanner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyVisualizerGraphX
{
    internal class SolutionSerializableToNameAndPath
    {
        public Solution Solution { get; private set; }

        public SolutionSerializableToNameAndPath(Solution s)
        {
            Solution = s;
        }
        public override string ToString()
        {
            return Solution.Name + "[" + Solution.FullPath + "]";
        }
    }
}
