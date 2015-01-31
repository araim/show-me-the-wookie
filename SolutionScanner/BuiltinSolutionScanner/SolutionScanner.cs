using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SolutionDependencyScanner
{
    internal class SolutionScanner : ISolutionScanner
    {
        private const string ProjectRegexText = "^Project\\(\"{[A-Z0-9-]+}\"\\)\\s*=\\s*\"([^\"]+)\"\\s*,\\s*\"([^\"]+)\"\\s*,\\s*\"{([A-Z0-9-]+)}\"\\s*$";
        private readonly Regex ProjectRegex = new Regex(ProjectRegexText, RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
        public SolutionScanner()
        {
            
        }

        public SolutionScaffold scan(System.IO.FileInfo f)
        {
            SolutionScaffold s = new SolutionScaffold();
            s.FullPath = f.FullName;
            s.Name = Path.GetFileNameWithoutExtension(f.FullName);
            using (var stream = f.OpenText())
            {
                while (!stream.EndOfStream)
                {
                    string line = stream.ReadLine();
                    if (line.StartsWith("Project("))
                    {
                        Match m = ProjectRegex.Match(line);
                        if (m.Success)
                        {
                            ProjectReference pr = new ProjectReference();
                            pr.ID = m.Groups[3].Value;
                            pr.Name = m.Groups[1].Value;
                            pr.Path = Path.Combine(Path.GetDirectoryName(f.FullName), m.Groups[2].Value);
                            s.Projects.Add(pr);
                        }
                    }
                }
            }
            return s;
        }
    }
}
