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
            FindSolutions(scr);
            return scr;
        }

        private void FindSolutions(ScanResult scr)
        {
            ISet<Solution> slns = new HashSet<Solution>();

            foreach (SolutionScaffold sc in scr.Scaffolds)
            {
                Solution s = Solution.FromScaffold(sc);
                foreach (ProjectReference pr in sc.Projects)
                {
                    if (scr.Projects.ContainsKey(pr.ID))
                    {
                        s.Projects.Add(scr.Projects[pr.ID]);
                    }
                    else
                    {
                        Project p = new Project();
                        p.ID = pr.ID;
                        p.Path = pr.Path;
                        p.AssemblyName = "Unknown from ReferenceName: " + pr.Name;
                        s.Projects.Add(p);
                    }
                }
                slns.Add(s);
            }
            scr.Solutions = slns;
        }


        private void ResolveProjects(ScanResult sr)
        {
            IDictionary<string, Project> allProjects = new Dictionary<string, Project>();
            Dictionary<string, Project> assembliesMap = new Dictionary<string, Project>();

            foreach (SolutionScaffold sln in sr.Scaffolds)
            {
                foreach (ProjectReference p in sln.Projects)
                {
                    try
                    {
                        IProjectScanner ps = new BuiltinProjectScanner.ProjectScanner(p.Path);
                        Project pr = ps.ScanProject();
                        allProjects.Add(pr.ID, pr);
                        assembliesMap[pr.AssemblyName] = pr;
                    }
                    catch (Exception e)
                    {
                        SendErrorEvent(p.Path, e);
                    }
                }
            }
            sr.Projects = allProjects;
            sr.AssembliesMap = assembliesMap;

        }

        private void DetermineDependencies(ScanResult sr)
        {
            foreach (Project p in sr.Projects.Values)
            {
                IList<string> newMissing = new List<string>();
                foreach (string mp in p.Dependencies.MissingDependencies)
                {
                    bool missing = true;
                    string assemblyFileName = Path.GetFileNameWithoutExtension(mp);
                    if (sr.AssembliesMap.ContainsKey(assemblyFileName))
                    {
                        p.Dependencies.ImplicitProjectDependencies.Add(sr.AssembliesMap[assemblyFileName]);
                        missing = false;
                    }
                    else
                    {
                        FileInfo fi = new FileInfo(mp);
                        if (fi.Exists)
                        {
                            p.Dependencies.HardcodedDependencies.Add(mp);
                            missing = false;
                        }
                    }
                    if (missing)
                    {
                        newMissing.Add(mp);
                    }
                }
                p.Dependencies.MissingDependencies = newMissing;
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
                    SendErrorEvent(f.FullName, e);
                }
            }
        }

        private void SendErrorEvent(string path, Exception e)
        {
            if (ErrorEncountered != null)
            {
                foreach (var err in ErrorEncountered.GetInvocationList())
                {
                    try
                    {
                        err.DynamicInvoke(path, new ScanErrorEventArgs(e));
                    }
                    catch (Exception)
                    {
                        // can't do anything, ignore.
                    }
                }
            }
        }
        private void SendSolutionEncountered(FileInfo f)
        {
            if (SolutionEncountered != null)
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
        }

        private void SendScanEvent(FileInfo f, SolutionScaffold s)
        {
            if (SolutionScanned != null)
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
