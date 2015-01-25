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
        private readonly IProjectScanner ps;
        public SolutionScanner(IProjectScanner ps)
        {
            this.ps = ps;
        }

        public Solution scan(System.IO.FileInfo f)
        {
            Solution s = new Solution();
            s.FullPath = f.FullName;
            using (var stream = f.OpenText())
            {

                //Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "Glympse", "Glympse\Glympse.csproj", "{08026ECD-191E-442E-BA5D-95D44D4A93FE}"   
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
