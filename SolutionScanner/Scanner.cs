﻿using System;
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
        public event ScanEvent SolutionEncountered;
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

        public ScanResult Scan()
        {
            ScanResult scr = new ScanResult();
            ISet<SolutionScaffold> set = ScanAndFindRoots();
            scr.Scaffolds = set;
            ResolveProjects(scr);
            DetermineDependencies(scr);
            return scr;
        }


        private void ResolveProjects(ScanResult sr)
        {
            ISet<Project> allProjects = new HashSet<Project>();
            Dictionary<string, Project> assembliesMap = new Dictionary<string, Project>();

            foreach (SolutionScaffold sln in sr.Scaffolds)
            {
                foreach (ProjectReference p in sln.Projects)
                {
                    IProjectScanner ps = new BuiltinProjectScanner.ProjectScanner(p.Path);
                    Project pr = ps.ScanProject();
                    allProjects.Add(pr);
                    assembliesMap[pr.AssemblyName] = pr;
                }
            }
            sr.Projects = allProjects;
            sr.AssembliesMap = assembliesMap;

        }

        private void DetermineDependencies(ScanResult sr)
        {
            foreach (Project p in sr.Projects)
            {
                foreach (string mp in p.Dependencies.MissingDependencies)
                {

                }
            }
            return;
        }


        private ISet<SolutionScaffold> ScanAndFindRoots()
        {
            HashSet<string> visited = new HashSet<string>();
            ISet<SolutionScaffold> set = new HashSet<SolutionScaffold>();
            Stack<DirectoryInfo> s = new Stack<DirectoryInfo>();
            s.Push(scanRoot);

            while (s.Count > 0)
            {
                DirectoryInfo di = s.Pop();
                if (visited.Contains(di.FullName))
                {
                    continue;
                }

                ScanDirectory(set, di);
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

        private void ScanDirectory(ISet<SolutionScaffold> set, DirectoryInfo di)
        {
            foreach (var f in di.EnumerateFiles("*.sln"))
            {
                try
                {
                    SolutionScaffold sln = scanSolution(f);
                    set.Add(sln);
                }
                catch (Exception e)
                {
                    SendErrorEvent(f, e);
                }
            }
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
        private void SendSolutionEncountered(FileInfo f)
        {

            foreach (var err in SolutionEncountered.GetInvocationList())
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

        private void SendScanEvent(FileInfo f, SolutionScaffold s)
        {

            foreach (var err in SolutionScanned.GetInvocationList())
            {
                try
                {
                    err.DynamicInvoke(f, new SolutionScannedEventArgs(s));
                }
                catch (Exception)
                {
                    // can't do anything, ignore.
                }
            }
        }

        private SolutionScaffold scanSolution(FileInfo f)
        {
            SendSolutionEncountered(f);
            SolutionScaffold s = scanner.scan(f);
            SendScanEvent(f, s);
            return s;
        }

    }
}
