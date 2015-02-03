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
            ScanFileAndAddProjects(f, s);
            return s;
        }

        private void ScanFileAndAddProjects(System.IO.FileInfo f, SolutionScaffold s)
        {

            using (var stream = f.OpenText())
            {
                while (!stream.EndOfStream)
                {
                    ParseLineAndAddProjectIfFound(f, s, stream.ReadLine());
                }
            }
        }

        private void ParseLineAndAddProjectIfFound(System.IO.FileInfo f, SolutionScaffold s, string line)
        {
            if (line.StartsWith("Project("))
            {
                Match m = ProjectRegex.Match(line);
                if (m.Success && m.Groups[2].Value.EndsWith("proj")) //vsproj, vcproj, etc.
                {
                    CreateAndAddProjectReference(f, s, m);
                }
            }
        }

        private static void CreateAndAddProjectReference(System.IO.FileInfo f, SolutionScaffold s, Match m)
        {
            ProjectReference pr = new ProjectReference();
            pr.ID = m.Groups[3].Value;
            pr.Name = m.Groups[1].Value;
            pr.Path = Path.Combine(Path.GetDirectoryName(f.FullName), m.Groups[2].Value);
            s.Projects.Add(pr);
        }
    }
}
