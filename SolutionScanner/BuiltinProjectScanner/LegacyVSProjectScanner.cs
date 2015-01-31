using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SolutionDependencyScanner.BuiltinProjectScanner
{
    internal class LegacyVSProjectScanner : AbstractProjectScanner
    {
        private const string AssemblyNameXPath = "VisualStudioProject/CSHARP/Build/Settings";
        private const string ProjectReferencesXPath = "VisualStudioProject/CSHARP/Build/References/Reference";

        private Project proj;

        public LegacyVSProjectScanner(XmlDocument xml, string path)
            : base(xml, path)
        {
        }


        public override Project ScanProject()
        {
            proj = new Project();
            proj.Path = path;
            proj.AssemblyName = FindAssemblyName();
            proj.ID = null;
            ProcessProjectReferences();
            return proj;
        }


        private string FindAssemblyName()
        {
            XmlNode n = xml.SelectSingleNode(AssemblyNameXPath);
            return n.Attributes["AssemblyName"].Value;
        }

        private void ProcessProjectReferences()
        {
            foreach (XmlNode node in xml.SelectNodes(ProjectReferencesXPath))
            {
                if (node.Attributes["AssemblyName"] != null)
                {
                    string dll = ProcessDllReference(node);
                    proj.Dependencies.MissingDependencies.Add(dll);

                }
                else
                {
                    ProjectReference pr = ProcessProjectReference(node);
                    proj.Dependencies.ReferencedProjectDependencies.Add(pr);
                }

            }
        }

        private string ProcessDllReference(XmlNode node)
        {
            return PathExtensions.GetAbsolutePath(path, node.Attributes["HintPath"].Value);
        }

        private ProjectReference ProcessProjectReference(XmlNode node)
        {
            return new ProjectReference()
            {
                ID = node.Attributes["Project"].Value,
                Name = node.Attributes["Name"].Value,
                Path = null
            };
        }

    }
}
