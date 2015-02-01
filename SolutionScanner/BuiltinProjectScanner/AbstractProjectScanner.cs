using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SolutionDependencyScanner.BuiltinProjectScanner
{
    internal abstract class AbstractProjectScanner : IProjectScanner
    {
        protected readonly XmlDocument xml;
        protected string path;

        public AbstractProjectScanner(XmlDocument xml, string path)
        {
            this.xml = xml;
            this.path = path;
        }

        public abstract Project ScanProject();
    }
}
