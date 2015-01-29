using SolutionDependencyScanner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyVisualizerGraphX
{
    internal class SolutionSerializableToNameAndPath : IComparable<SolutionSerializableToNameAndPath>
    {
        public Solution Solution { get; private set; }

        public SolutionSerializableToNameAndPath(Solution s)
        {
            Solution = s;
        }
        public override string ToString()
        {
            return Solution.Name + "(" + Solution.Projects.Count + ") [" + Solution.FullPath + "]";
        }

        public int CompareTo(SolutionSerializableToNameAndPath other)
        {
            int cmp = Solution.Projects.Count - other.Solution.Projects.Count;
            if (cmp == 0)
            {
                cmp = Solution.Name.CompareTo(other.Solution.Name);
            }
            return cmp;
        }
    }
}
