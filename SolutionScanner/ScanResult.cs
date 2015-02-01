using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolutionDependencyScanner
{
    public sealed class ScanResult
    {

        internal ISet<SolutionScaffold> Scaffolds { get; set; }
        public ISet<Solution> Solutions { get; internal set; }
        public IDictionary<string,Project> Projects { get; internal set; }
        public Dictionary<string, Project> AssembliesMap { get; internal set; }
    }
}
