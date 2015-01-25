using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolutionDependencyScanner
{
    public class Solution
    {
        public string FullPath { get; set; }
        public IList<ProjectReference> Projects { get; private set; }


        internal Solution()
        {
            Projects = new List<ProjectReference>();
        }

        public override string ToString()
        {
            return "Solution { FullPath = " + FullPath + ", Projects = " + string.Join(",", Projects) + "}";
        }
    }
}
