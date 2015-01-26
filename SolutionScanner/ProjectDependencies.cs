using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolutionDependencyScanner
{
    public class ProjectDependencies
    {

        public ProjectDependencies()
        {
            ReferencedProjectDependencies = new List<ProjectReference>();
            ImplicitProjectDependencies = new List<Project>();
            HardcodedDependencies = new List<string>();
            MissingDependencies = new List<string>();
        }

        public IList<ProjectReference> ReferencedProjectDependencies { get; internal set; }

        public IList<Project> ImplicitProjectDependencies { get; internal set; }

        public IList<string> HardcodedDependencies { get; internal set; }

        public IList<string> MissingDependencies { get; internal set; }


        public override string ToString()
        {
            return "ProjectDependencies { \r\n ReferencedProjectDependencies = [" + string.Join("\r\n  ", ReferencedProjectDependencies) + "]\r\nImplicitProjectDependencies = ["
        + string.Join("\r\n  ", ImplicitProjectDependencies) + "]\r\nHardcodedDependencies = [" + string.Join("\r\n  ", HardcodedDependencies) + "]\r\nMissingDependencies = ["
        + string.Join("\r\n  ", MissingDependencies) + "]}\r\n";
        }
    }
}
