using SolutionDependencyScanner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScannerRunner
{
    class Program
    {
        static void Main(string[] args)
        {
            Scanner s = new Scanner(@"C:\Users\fox\Documents\visual studio 2013\Projects\show-me-the-wookie");
            s.ErrorEncountered += s_ErrorEncountered;
            s.SolutionScanned += s_SolutionScanned;
            Console.WriteLine(s.ScanAndFindRoots());
        }

        static void s_SolutionScanned(object sender, EventArgs e)
        {
            Console.WriteLine("[{0}] {1}: {2}", "INFO", sender, e);
        }

        static void s_ErrorEncountered(object sender, EventArgs e)
        {
            Console.WriteLine("[{0}] {1}: {2}", "ERROR", sender, e);
        }
    }
}
