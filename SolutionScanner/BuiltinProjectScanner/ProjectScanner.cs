using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace SolutionDependencyScanner.BuiltinProjectScanner
{

    internal class ProjectScanner : IProjectScanner
    {
        private const string ProjectReferenceXPath = "/ns:Project/ns:ItemGroup/ns:ProjectReference";
        private const string DllReferenceXPath = "/ns:Project/ns:ItemGroup/ns:Reference/ns:HintPath";
        private const string AssemblyNameXPath = "/ns:Project/ns:PropertyGroup/ns:AssemblyName";
        private const string MSBuildNamespace = "http://schemas.microsoft.com/developer/msbuild/2003";

        private readonly XmlDocument xml;
        private readonly XmlNamespaceManager mgr;
        private Project proj;
        private string path;

        public ProjectScanner(string projectPath)
        {
            path = projectPath;
            xml = new XmlDocument();
            xml.Load(projectPath);
            mgr = new XmlNamespaceManager(xml.NameTable);
            mgr.AddNamespace("ns", MSBuildNamespace);
        }

        public Project ScanProject()
        {
            proj = new Project();
            proj.Path = path;
            proj.AssemblyName = FindAssemblyName();
            ProcessProjectReferences(GetReferencedDll, DllReferenceXPath);
            ProcessProjectReferences(GetProjectReference, ProjectReferenceXPath);

            return proj;
        }

        private string FindAssemblyName()
        {
            XmlNode n = xml.SelectSingleNode(AssemblyNameXPath, mgr);
            return n.InnerText;
        }

        private void ProcessProjectReferences(Action<XmlNode> processor, string XPath)
        {
            foreach (XmlNode node in xml.SelectNodes(XPath, mgr))
            {
                processor(node);
            }
        }

        private void GetReferencedDll(XmlNode node)
        {
            string dllPath = node.InnerText;
            string dllFullPath = Path.Combine(Path.GetDirectoryName(path), dllPath);
            dllFullPath = PathExtensions.Normalize(dllFullPath);
            proj.Dependencies.MissingDependencies.Add(dllFullPath);
        }


        private void GetProjectReference(XmlNode node)
        {
            string path = node.Attributes["Include"].Value;
            string id = string.Empty;
            string name = string.Empty;

            foreach (XmlNode n in node.ChildNodes)
            {
                if (n.Name == "Project")
                {
                    if (n.InnerText != null)
                    {
                        id = n.InnerText.Replace("}", "").Replace("{", "").ToUpperInvariant();
                    }
                }
                else if (n.Name == "Name")
                {
                    name = n.InnerText;
                }
            }
            ProjectReference pr = new ProjectReference()
            {
                ID = id,
                Name = name,
                Path = path
            };
            proj.Dependencies.ReferencedProjectDependencies.Add(pr);
        }

    }
}
