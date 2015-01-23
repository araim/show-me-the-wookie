using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolutionDependencyScanner
{
    public class Solution
    {
        public string FullPath { get; set; }
        public IList<Project> Projects { get; private set; }


        internal Solution()
        {
            Projects = new List<Project>();
        }

    }
}
