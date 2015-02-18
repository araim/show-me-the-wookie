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
        private DependencyGraph graph;
        private ISet<string> alreadySelectedProjects;
        private Queue<Project> projectsToProcess;
        private Dictionary<string, DependencyVertex> vertices;
        private GraphCreationOptions options;
        private Solution root;


        private readonly SolutionDependencyScanProduct scanProduct;

        public GraphFactory(SolutionDependencyScanProduct sr)
        {
            scanProduct = sr;
        }

        public DependencyGraph CreateGraph(Solution rootSolution, GraphCreationOptions options = GraphCreationOptions.None)
        {
            PrepareEmptyGraph(rootSolution, options);
            IncludeInitialSetOfProjects();

            while (projectsToProcess.Count > 0)
            {
                Project p = projectsToProcess.Dequeue();
                ProcessProjectAndItsDependencies(p);
            }
            IncludeRootSolution();
            ProcessOptionalGraphElements();
            return graph;

        }

        private void IncludeRootSolution()
        {
            IncludeSolution("SOLUTION : " + root.Name, root.Projects);
        }

        private void ProcessOptionalGraphElements()
        {
            if (options.HasFlag(GraphCreationOptions.IncludeSolutionsReferencingSelectedProjects))
            {
                AddSolutionsThatDependOnSelectedProjects();
            }
        }

        private void ProcessProjectAndItsDependencies(Project p)
        {
            if (!alreadySelectedProjects.Contains(p.ID))
            {
                AddProjectVertexAndRegisterProject(p);
            }
            ProcessImplicitProjectDependencies(p);
            ProcessReferencedProjects(p);
        }

        private void ProcessReferencedProjects(Project p)
        {
            foreach (ProjectReference subp in p.Dependencies.ReferencedProjectDependencies)
            {
                if (scanProduct.Projects.ContainsKey(subp.ID))
                {
                    string pname = scanProduct.Projects[subp.ID].AssemblyName;
                    if (!alreadySelectedProjects.Contains(subp.ID))
                    {
                        AddProjectVertexAndRegisterProject(pname, subp.ID);
                    }
                    AddEdge(p.AssemblyName, pname);
                }
            }
        }

        private void ProcessImplicitProjectDependencies(Project p)
        {
            foreach (Project subp in p.Dependencies.ImplicitProjectDependencies)
            {
                if (!alreadySelectedProjects.Contains(subp.ID))
                {
                    AddProjectVertexAndRegisterProject(subp);
                    projectsToProcess.Enqueue(subp);
                }
                AddEdge(p.AssemblyName, subp.AssemblyName);
            }
        }

        private void AddEdge(string from, string to)
        {
            graph.AddEdge(new DependencyEdge(vertices[from], vertices[to], 1));
        }

        private void AddProjectVertexAndRegisterProject(Project p)
        {
            AddProjectVertexAndRegisterProject(p.AssemblyName, p.ID);
        }


        private void AddVertex(DependencyVertex dv)
        {
            vertices[dv.Text] = dv;
            graph.AddVertex(dv);
        }

        private void AddSolutionVertex(string name)
        {
            AddVertex(new SolutionVertex(name));
        }

        private void AddProjectVertexAndRegisterProject(string name, string projectId)
        {
            AddVertex(new ProjectVertex(name));
            alreadySelectedProjects.Add(projectId);
        }

        private void IncludeInitialSetOfProjects()
        {
            foreach (var p in root.Projects)
            {
                projectsToProcess.Enqueue(p);
            }
        }

        private void PrepareEmptyGraph(Solution rootSolution, GraphCreationOptions opts)
        {
            root = rootSolution;
            graph = new DependencyGraph();
            options = opts;
            alreadySelectedProjects = new HashSet<string>();
            projectsToProcess = new Queue<Project>();
            vertices = new Dictionary<string, DependencyVertex>();
        }

        private void AddSolutionsThatDependOnSelectedProjects()
        {
            foreach (var s in scanProduct.Solutions)
            {
                var cross = s.Projects.Where((el, idx) => alreadySelectedProjects.Contains(el.ID));
                if (cross.Count() > 0)
                {
                    IncludeSolution("SOLUTION:" + s.Name, cross);
                }
            }
        }

        private void IncludeSolution(string name, IEnumerable<Project> cross)
        {
            AddSolutionVertex(name);
            foreach (var p in cross)
            {
                AddEdge(name, p.AssemblyName);
            }
        }

    }
}
