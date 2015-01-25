using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolutionDependencyScanner
{
    internal interface IProjectScanner
    {
        Project scanProject(string p);
    }
}
