using CWDev.SLNTools.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolutionDependencyScanner
{
    internal class SolutionScanner : ISolutionScanner
    {
        public Solution scan(System.IO.FileInfo f)
        {
            SolutionFileReader sfr = new SolutionFileReader(f.OpenRead());
            SolutionFile sf = sfr.ReadSolutionFile();

            Solution s = new Solution();
            s.FullPath = sf.SolutionFullPath;
            foreach (var p in sf.Projects)
            {
                Project pr = new Project();
                pr.Path = p.RelativePath;
                s.Projects.Add(pr);

            }
            return s;
        }
    }
}
