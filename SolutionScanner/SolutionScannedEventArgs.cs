using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolutionDependencyScanner
{
    public class SolutionScannedEventArgs:EventArgs
    {
        public SolutionScannedEventArgs(Solution s)
        {
            Solution = s;
        }

        public Solution Solution { get; private set; }
    }
}
