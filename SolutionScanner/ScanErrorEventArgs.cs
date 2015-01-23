﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolutionDependencyScanner
{
    public class ScanErrorEventArgs
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