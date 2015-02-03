using SolutionDependencyScanner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyVisualizerGraphX
{
    internal class GraphFactory
    {

        private readonly SolutionDependencyScanProduct scanProduct;

        public GraphFactory(SolutionDependencyScanProduct sr)
        {
            scanProduct = sr;
        }

        public DependencyGraph CreateGraph(Solution rootSolution, GraphCreationOptions options = GraphCreationOptions.None)
        {
            var graph = new DependencyGraph();
            ISet<string> alreadySelectedProjects = new HashSet<string>();
            Queue<Project> projectsToProcess = new Queue<Project>();
            Dictionary<string, DependencyVertex> vertices = new Dictionary<string, DependencyVertex>();

            foreach (var p in rootSolution.Projects)
            {
                projectsToProcess.Enqueue(p);
            }
            while (projectsToProcess.Count > 0)
            {
                Project p = projectsToProcess.Dequeue();
                if (!alreadySelectedProjects.Contains(p.ID))
                {
                    alreadySelectedProjects.Add(p.ID);
                    var dv = new DependencyVertex(p.AssemblyName);
                    vertices[p.AssemblyName] = dv;
                    graph.AddVertex(dv);
                }
                foreach (Project subp in p.Dependencies.ImplicitProjectDependencies)
                {
                    if (!alreadySelectedProjects.Contains(subp.ID))
                    {
                        var dv = new DependencyVertex(subp.AssemblyName);
                        vertices[subp.AssemblyName] = dv;
                        graph.AddVertex(dv);
                        alreadySelectedProjects.Add(subp.ID);
                        projectsToProcess.Enqueue(subp);
                    }
                    graph.AddEdge(new DependencyEdge(vertices[p.AssemblyName], vertices[subp.AssemblyName], 1));
                }
                foreach (ProjectReference subp in p.Dependencies.ReferencedProjectDependencies)
                {

                    if (scanProduct.Projects.ContainsKey(subp.ID))
                    {
                        string pname = scanProduct.Projects[subp.ID].AssemblyName;
                        if (!alreadySelectedProjects.Contains(subp.ID))
                        {
                            var dv = new DependencyVertex(pname);
                            vertices[pname] = dv;
                            graph.AddVertex(dv);
                            alreadySelectedProjects.Add(subp.ID);
                        }
                        graph.AddEdge(new DependencyEdge(vertices[p.AssemblyName], vertices[pname], 1));
                    }
                }

            }
            if (options.HasFlag(GraphCreationOptions.IncludeSolutionsReferencingSelectedProjects))
            {
                AddSolutionsThatDependOnSelectedProjects(graph, alreadySelectedProjects, vertices);
            }
            return graph;

        }

        private void AddSolutionsThatDependOnSelectedProjects(DependencyGraph graph, ISet<string> displayedProjects, Dictionary<string, DependencyVertex> vertices)
        {

            foreach (var s in scanProduct.Solutions)
            {
                var cross = s.Projects.Where((el, idx) => displayedProjects.Contains(el.ID));
                if (cross.Count() > 0)
                {
                    string sol = "SOLUTION:" + s.Name;
                    var dv = new DependencyVertex(sol);
                    vertices[sol] = dv;
                    graph.AddVertex(dv);

                    foreach (var p in cross)
                    {
                        graph.AddEdge(new DependencyEdge(vertices[sol], vertices[p.AssemblyName], 1));
                    }
                }
            }
        }

    }
}
