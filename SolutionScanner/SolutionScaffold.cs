using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolutionDependencyScanner
{
    public class SolutionScaffold
    {

        public string Name { get; set; }
        public string FullPath { get; set; }
        public IList<ProjectReference> Projects { get; private set; }


        internal SolutionScaffold()
        {
            Projects = new List<ProjectReference>();
        }

        public override string ToString()
        {
            return "SolutionScaffold { Name = " + Name + ", FullPath = " + FullPath + ", Projects = " + string.Join(",", Projects) + "}";
        }
    }
}
