using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SolutionDependencyScanner.BuiltinProjectScanner
{
    internal class MultiVersionProjectScanner:IProjectScanner
    {
        private readonly XmlDocument xml;
        private string path;

        private AbstractProjectScanner scanner;

        private const string LegacyVSRootElementName = "VisualStudioProject";
        private const string ModernVSRootElementName = "Project";
        
     
        public MultiVersionProjectScanner(string projectPath,string projectID = null)
        {
            try
            {
                path = projectPath;
                xml = new XmlDocument();
                xml.Load(projectPath);
                if (xml.DocumentElement.Name == LegacyVSRootElementName)
                {
                    scanner = new LegacyVSProjectScanner(xml, path);
                }
                else if (xml.DocumentElement.Name == ModernVSRootElementName)
                {
                    scanner = new ModernVSProjectScanner(xml, path);
                }
                else
                {
                    throw new ArgumentException("Provided path is not a recognized VS project file or is not accesible", "projectPath");
                }
            }
            catch (Exception e)
            {
                throw new ArgumentException("Provided path is not a valid XML document or is not accesible", "projectPath", e);
            }
        }


        public Project ScanProject()
        {
            return scanner.ScanProject();
        }
    }
}
