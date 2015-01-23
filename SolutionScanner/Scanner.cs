﻿using CWDev.SLNTools.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolutionDependencyScanner
{
    public sealed class Scanner
    {
        public delegate void ScanEvent(object sender, EventArgs e);
        public event ScanEvent SolutionScanned;
        public event ScanEvent ErrorEncountered;


        private readonly DirectoryInfo scanRoot;
        private readonly ISolutionScanner scanner;


        public Scanner(string path)
        {
            try
            {
                scanRoot = new DirectoryInfo(path);
                if (!scanRoot.Exists)
                {
                    throw new ApplicationException();
                }
            }
            catch (Exception)
            {
                throw new ArgumentException("Invalid path specified", "path");
            }
            scanner = new SolutionScanner();
        }


        public ISet<Solution> ScanAndFindRoots()
        {
            HashSet<string> visited = new HashSet<string>();
            ISet<Solution> set = new HashSet<Solution>();
            Stack<DirectoryInfo> s = new Stack<DirectoryInfo>();
            s.Push(scanRoot);

            while (s.Count > 0)
            {
                DirectoryInfo di = s.Pop();
                if (visited.Contains(di.FullName))
                {
                    continue;
                }
                foreach (var f in di.EnumerateFiles("*.sln"))
                {
                    try
                    {
                        set.Add(scanSolution(f));
                    }
                    catch (Exception e)
                    {
                        SendErrorEvent(f, e);
                    }
                }
                foreach (var d in di.EnumerateDirectories())
                {
                    if (!visited.Contains(d.FullName))
                    {
                        s.Push(d);
                    }
                }
            }

            return set;
        }

        private void SendErrorEvent(FileInfo f, Exception e)
        {

            foreach (var err in ErrorEncountered.GetInvocationList())
            {
                try
                {
                    err.DynamicInvoke(f, new ScanErrorEventArgs(e));
                }
                catch (Exception)
                {
                    // can't do anything, ignore.
                }
            }
        }
        private void SendScanEvent(FileInfo f)
        {

            foreach (var err in SolutionScanned.GetInvocationList())
            {
                try
                {
                    err.DynamicInvoke(f, new EventArgs());
                }
                catch (Exception)
                {
                    // can't do anything, ignore.
                }
            }
        }

        private Solution scanSolution(FileInfo f)
        {
            SendScanEvent(f);
            return scanner.scan(f);
        }

    }
}