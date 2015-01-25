using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace SolutionDependencyScanner
{



    internal class ProjectScanner : IProjectScanner
    {
        private const string ProjectReferenceXPath = "/Project/ItemGroup/ProjectReference";
        private const string DllReferenceXPath = "/Project/ItemGroup/Reference";


        public Project scanProject(string p)
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(p);
            return scanProject(xml.DocumentElement);
        }

        public Project scanProject(XmlElement document)
        {
            GetProjectReferences(document);
            GetReferencedDlls(document);
            return new Project();
        }

        private void GetReferencedDlls(XmlElement document)
        {
            foreach (var node in document.SelectNodes(DllReferenceXPath))
            {
                Console.WriteLine(node);
            }
        }


        private void GetProjectReferences(XmlElement document)
        {
            foreach (var node in document.SelectNodes(ProjectReferenceXPath))
            {
                Console.WriteLine(node);
            }
        }

    }
}
