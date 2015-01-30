using GraphX;
using GraphX.Controls;
using GraphX.GraphSharp.Algorithms.EdgeRouting;
using GraphX.GraphSharp.Algorithms.Layout.Simple.FDP;
using GraphX.Models;
using SolutionDependencyScanner;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DependencyVisualizerGraphX
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private DependencyGraph graph = new DependencyGraph();
        private ObservableCollection<SolutionSerializableToNameAndPath> slns = new ObservableCollection<SolutionSerializableToNameAndPath>();

        private ScanResult sr;

        private Color c1 = Color.FromRgb(0x33, 0x33, 0x33);
        private Color c2 = Color.FromRgb(0x66, 0x22, 0x22);
        private Color c3 = Color.FromRgb(0x99, 0x11, 0x11);

        private Color c4 = Color.FromRgb(0x33, 0x33, 0x33);
        private Color c5 = Color.FromRgb(0x22, 0x22, 0x66);
        private Color c6 = Color.FromRgb(0x11, 0x11, 0x99);

        public MainWindow()
        {
            DataContext = this;
            InitializeComponent();
            GraphArea_Setup();
            SolutionList.ItemsSource = slns;
            Loaded += MainWindow_Loaded;
            Area.VertexDoubleClick += Area_VertexDoubleClick;
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Area.GenerateGraph(true, true);
            zoomctrl.ZoomToFill();
        }

        private DependencyGraph GraphExample_Setup()
        {
            //Lets make new data graph instance
            var dataGraph = new DependencyGraph();
            //Now we need to create edges and vertices to fill data graph
            //This edges and vertices will represent graph structure and connections
            //Lets make some vertices
            for (int i = 1; i < 10; i++)
            {
                //Create new vertex with specified Text. Also we will assign custom unique ID.
                //This ID is needed for several features such as serialization and edge routing algorithms.
                //If you don't need any custom IDs and you are using automatic Area.GenerateGraph() method then you can skip ID assignment
                //because specified method automaticaly assigns missing data ids (this behavior controlled by method param).
                var dataVertex = new DependencyVertex("MyVertex " + i) { ID = i };
                //Add vertex to data graph
                dataGraph.AddVertex(dataVertex);
            }

            //Now lets make some edges that will connect our vertices
            //get the indexed list of graph vertices we have already added
            var vlist = dataGraph.Vertices.ToList();
            //Then create two edges optionaly defining Text property to show who are connected
            var dataEdge = new DependencyEdge(vlist[0], vlist[1]) { Text = string.Format("{0} -> {1}", vlist[0], vlist[1]) };
            dataGraph.AddEdge(dataEdge);
            dataEdge = new DependencyEdge(vlist[2], vlist[3]) { Text = string.Format("{0} -> {1}", vlist[2], vlist[3]) };
            dataGraph.AddEdge(dataEdge);

            return dataGraph;
        }

        private void GraphArea_Setup()
        {
            //Lets create logic core and filled data graph with edges and vertices
            var logicCore = new DependencyGraphLogicCore() { Graph = GraphExample_Setup() };
            //This property sets layout algorithm that will be used to calculate vertices positions
            //Different algorithms uses different values and some of them uses edge Weight property.
            logicCore.DefaultLayoutAlgorithm = LayoutAlgorithmTypeEnum.FR;
            //Now we can set parameters for selected algorithm using AlgorithmFactory property. This property provides methods for
            //creating all available algorithms and algo parameters.
            logicCore.DefaultLayoutAlgorithmParams = logicCore.AlgorithmFactory.CreateLayoutParameters(LayoutAlgorithmTypeEnum.FR);
            //Unfortunately to change algo parameters you need to specify params type which is different for every algorithm.
            //            ((LinLogLayoutParameters)logicCore.DefaultLayoutAlgorithmParams).IterationCount = 15;

            //This property sets vertex overlap removal algorithm.
            //Such algorithms help to arrange vertices in the layout so no one overlaps each other.
            logicCore.DefaultOverlapRemovalAlgorithm = OverlapRemovalAlgorithmTypeEnum.FSA;
            //Default parameters are created automaticaly when new default algorithm is set and previous params were NULL
            logicCore.DefaultOverlapRemovalAlgorithmParams.HorizontalGap = 150;
            logicCore.DefaultOverlapRemovalAlgorithmParams.VerticalGap = 50;

            //This property sets edge routing algorithm that is used to build route paths according to algorithm logic.
            //For ex., SimpleER algorithm will try to set edge paths around vertices so no edge will intersect any vertex.
            //Bundling algorithm will try to tie different edges that follows same direction to a single channel making complex graphs more appealing.
            logicCore.DefaultEdgeRoutingAlgorithm = EdgeRoutingAlgorithmTypeEnum.SimpleER;

            //This property sets async algorithms computation so methods like: Area.RelayoutGraph() and Area.GenerateGraph()
            //will run async with the UI thread. Completion of the specified methods can be catched by corresponding events:
            //Area.RelayoutFinished and Area.GenerateGraphFinished.
            logicCore.AsyncAlgorithmCompute = false;

            //Finally assign logic core to GraphArea object
            Area.LogicCore = logicCore;// as IGXLogicCore<DataVertex, DataEdge, BidirectionalGraph<DataVertex, DataEdge>>;
        }

        private void BrowseFolder(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.ShowNewFolderButton = false;

            dlg.SelectedPath = RootPath.Text;

            DialogResult result = dlg.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                RootPath.Text = dlg.SelectedPath;
              scanBtn.IsEnabled = true;
            }
          
        }

        private void scanBtn_Click(object sender, RoutedEventArgs e)
        {
          scanBtn.IsEnabled = false;
            string path = RootPath.Text;
            SolutionDependencyScanner.Scanner s = new SolutionDependencyScanner.Scanner(path);
            sr = s.Scan();
            slns.Clear();
            List<Solution> l = new List<Solution>();
            l.AddRange(sr.Solutions);

            l.Sort((s1, s2) => s1.Name.CompareTo(s2.Name));

            foreach (Solution sln in l)
            {
                slns.Add(new SolutionSerializableToNameAndPath(sln));
            }

          scanBtn.IsEnabled = true;
          plotBtn.IsEnabled = true;
          SolutionList.IsEnabled = true;
        }

        private void plotBtn_Click(object sender, RoutedEventArgs e)
        {
            //var g = new BidirectionalGraph<object, IEdge<object>>();

            SolutionSerializableToNameAndPath slnser = SolutionList.SelectedItem as SolutionSerializableToNameAndPath;
            Solution sln = slnser.Solution;

            ISet<string> displayedProjects = new HashSet<string>();
            Queue<Project> projectsToProcess = new Queue<Project>();
            Dictionary<string, DependencyVertex> vertices = new Dictionary<string, DependencyVertex>();
            Area.ClearLayout();
            graph = new DependencyGraph();

            foreach (var p in sln.Projects)
            {
                projectsToProcess.Enqueue(p);
            }
            while (projectsToProcess.Count > 0)
            {
                Project p = projectsToProcess.Dequeue();
                if (!displayedProjects.Contains(p.ID))
                {
                    displayedProjects.Add(p.ID);
                    var dv = new DependencyVertex(p.AssemblyName);
                    vertices[p.AssemblyName] = dv;
                    graph.AddVertex(dv);
                }
                foreach (Project subp in p.Dependencies.ImplicitProjectDependencies)
                {
                    if (!displayedProjects.Contains(subp.ID))
                    {
                        var dv = new DependencyVertex(subp.AssemblyName);
                        vertices[subp.AssemblyName] = dv;
                        graph.AddVertex(dv);
                        displayedProjects.Add(subp.ID);
                        projectsToProcess.Enqueue(subp);
                    }
                    graph.AddEdge(new DependencyEdge(vertices[p.AssemblyName], vertices[subp.AssemblyName], 1));
                }
                foreach (ProjectReference subp in p.Dependencies.ReferencedProjectDependencies)
                {

                    if (sr.Projects.ContainsKey(subp.ID))
                    {
                        string pname = sr.Projects[subp.ID].AssemblyName;
                        if (!displayedProjects.Contains(subp.ID))
                        {
                            var dv = new DependencyVertex(pname);
                            vertices[pname] = dv;
                            graph.AddVertex(dv);
                            displayedProjects.Add(subp.ID);
                        }
                        graph.AddEdge(new DependencyEdge(vertices[p.AssemblyName], vertices[pname], 1));
                    }
                }
            }

            foreach (var s in sr.Solutions)
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
          
            Area.GetLogicCore<DependencyGraphLogicCore>().DefaultEdgeRoutingAlgorithm = EdgeRoutingAlgorithmTypeEnum.SimpleER;
            var srp = (SimpleERParameters)Area.GetLogicCore<DependencyGraphLogicCore>().AlgorithmFactory.CreateEdgeRoutingParameters(EdgeRoutingAlgorithmTypeEnum.SimpleER);

            Area.GetLogicCore<DependencyGraphLogicCore>().DefaultEdgeRoutingAlgorithmParams = srp;

            Area.GetLogicCore<DependencyGraphLogicCore>().DefaultLayoutAlgorithm = LayoutAlgorithmTypeEnum.LinLog;


            Area.GetLogicCore<DependencyGraphLogicCore>().DefaultOverlapRemovalAlgorithm = OverlapRemovalAlgorithmTypeEnum.FSA;
            Area.GetLogicCore<DependencyGraphLogicCore>().DefaultOverlapRemovalAlgorithmParams = Area.LogicCore.AlgorithmFactory.CreateOverlapRemovalParameters(OverlapRemovalAlgorithmTypeEnum.FSA);
            Area.GetLogicCore<DependencyGraphLogicCore>().DefaultOverlapRemovalAlgorithmParams.HorizontalGap = 50;
            Area.GetLogicCore<DependencyGraphLogicCore>().DefaultOverlapRemovalAlgorithmParams.VerticalGap = 50;


            //Area.MouseOverAnimation = new HighlightSpreadAnimation<DependencyEdge, DependencyVertex, DependencyGraphArea>(Area);


            Area.GenerateGraph(graph, true);
            ClearHighlight();
            Area.SetVerticesDrag(true, true);
            zoomctrl.ZoomToFill();
            //Area.GenerateGraph(graph,true);
            /*Area.RelayoutGraph();
            Area.GenerateAllEdges();
            Area.RelayoutGraph(true);
            Area.SetVerticesDrag(true);*/
            ///zoomctrl.Mode = ZoomControlModes.Custom;
        }

        void Area_VertexDoubleClick(object sender, VertexSelectedEventArgs args)
        {
            ClearHighlight();

            new HighlightSpreadAnimation<DependencyEdge, DependencyVertex, DependencyGraphArea>(Area, c1, c2, c3).AnimateVertexForward(args.VertexControl);
            new HighlightSpreadAnimation<DependencyEdge, DependencyVertex, DependencyGraphArea>(Area, c4, c5, c6,false).AnimateVertexForward(args.VertexControl);

        }

        private void ClearHighlight()
        {
            
            foreach (VertexControl vc in Area.GetAllVertexControls())
            {
                vc.Foreground = new SolidColorBrush(c1);;
            }
            foreach (EdgeControl ec in Area.EdgesList.Values)
            {
                ec.Foreground = new SolidColorBrush(c1);;
            }
        }

        private void graphLayout_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //zc.AnimationLength = TimeSpan.Zero;
        }

        private void graphLayout_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            //graphLayout.HighlightAlgorithm = 
        }


    }
}
