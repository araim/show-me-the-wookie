using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolutionDependencyScanner.Events
{
    public class ScanErrorEventArgs : EventArgs
    {
        public Exception ExceptionThrown { get; private set; }
        public string ErrorMessage { get; private set; }

        internal ScanErrorEventArgs(Exception e)
        {
            ExceptionThrown = e;
            ErrorMessage = e.ToString();
        }

    }
}
