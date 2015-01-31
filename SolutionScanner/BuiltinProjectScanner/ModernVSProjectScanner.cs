using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace SolutionDependencyScanner.BuiltinProjectScanner
{

    internal class ModernVSProjectScanner : AbstractProjectScanner
    {
        private const string ProjectReferenceXPath = "/msbuild2003:Project/msbuild2003:ItemGroup/msbuild2003:ProjectReference";
        private const string DllReferenceXPath = "/msbuild2003:Project/msbuild2003:ItemGroup/msbuild2003:Reference/msbuild2003:HintPath";
        private const string AssemblyNameXPath = "/msbuild2003:Project/msbuild2003:PropertyGroup/msbuild2003:AssemblyName";
        private const string ProjectGUIDXPath = "/msbuild2003:Project/msbuild2003:PropertyGroup/msbuild2003:ProjectGuid";
        private const string MSBuildNamespace = "http://schemas.microsoft.com/developer/msbuild/2003";

        private readonly XmlNamespaceManager mgr;
        private Project proj;

        public ModernVSProjectScanner(XmlDocument xml, string path, string projectID = null)
            : base(xml, path)
        {
            mgr = new XmlNamespaceManager(xml.NameTable);
            mgr.AddNamespace("msbuild2003", MSBuildNamespace);
        }


        public override Project ScanProject()
        {
            proj = new Project();
            proj.Path = path;
            proj.AssemblyName = FindAssemblyName();
            proj.ID = FindProjectGUID();
            ProcessProjectReferences(GetReferencedDll, DllReferenceXPath);
            ProcessProjectReferences(GetProjectReference, ProjectReferenceXPath);

            return proj;
        }

        private string FindProjectGUID()
        {
            XmlNode n = xml.SelectSingleNode(ProjectGUIDXPath, mgr);
            return n.InnerText.Replace("}", "").Replace("{", "").ToUpperInvariant();
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
            dllPath = PathExtensions.GetAbsolutePath(path,dllPath);
            proj.Dependencies.MissingDependencies.Add(dllPath);
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
