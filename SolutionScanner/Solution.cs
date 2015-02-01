using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolutionDependencyScanner
{
    public sealed class Solution
    {

        public string Name { get; set; }
        public string FullPath { get; set; }
        public IList<Project> Projects { get; private set; }


        internal Solution()
        {
            Projects = new List<Project>();
        }

        public override string ToString()
        {
            return "Solution { Name = " + Name + ", FullPath = " + FullPath + ", Projects = " + string.Join(",", Projects) + "}";
        }

        internal static Solution FromScaffold(SolutionScaffold sc)
        {
            Solution s = new Solution();
            s.Name = sc.Name;
            s.FullPath = sc.FullPath;
            return s;
        }
    }
}
