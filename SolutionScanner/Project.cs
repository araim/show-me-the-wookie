using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolutionDependencyScanner
{
    public class Project
    {

        public Project()
        {
            Dependencies = new ProjectDependencies();
        }
        public string Path { get; internal set; }

        public string ID { get; internal set; }

        public string AssemblyName { get; internal set; }

        public bool IsMissing { get; internal set; }

        public string Name { get; internal set; }

        public ProjectDependencies Dependencies { get; internal set; }

        public override string ToString()
        {
            return "Project { ID = " + ID + ",Name = " + Name + ",Path = " + Path + ", AssemblyName = " + AssemblyName + ", Dependencies = [" + Dependencies + "]}";
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Project))
            {
                return false;
            }
            Project pr = obj as Project;
            return Path == pr.Path;
        }

        public override int GetHashCode()
        {
            return Path.GetHashCode();
        }
    }
}
