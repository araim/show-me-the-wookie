using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolutionDependencyScanner.Events
{
    public class SolutionScannedEventArgs:EventArgs
    {
        public SolutionScannedEventArgs(SolutionScaffold s)
        {
            Solution = s;
        }

        public SolutionScaffold Solution { get; private set; }
    }
}
