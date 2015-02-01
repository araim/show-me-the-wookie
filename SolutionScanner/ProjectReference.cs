using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolutionDependencyScanner
{
    public sealed class ProjectReference
    {
        public string ID { get; set; }

        public string Name { get; set; }

        public string Path { get; set; }


        public override string ToString()
        {
            return "ProjectReference {ID = " + ID + " , Name = " + Name + ", Path = " + Path + "}";
        }
    }
}
