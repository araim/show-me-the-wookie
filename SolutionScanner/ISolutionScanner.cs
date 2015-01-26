using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolutionDependencyScanner
{
    internal interface ISolutionScanner
    {
        SolutionScaffold scan(FileInfo f);

    }
}
