using Microsoft.Win32;
using QuickGraph;
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

namespace DependencyVisualizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private ObservableCollection<SolutionSerializableToNameAndPath> slns = new ObservableCollection<SolutionSerializableToNameAndPath>();
        private IBidirectionalGraph<object, IEdge<object>> _graphToVisualize;
        private ScanResult sr;
        
        public IBidirectionalGraph<object, IEdge<object>> GraphToVisualize
        {
            get { return _graphToVisualize; }
        }

        public MainWindow()
        {
            InitializeComponent();
            SolutionList.ItemsSource = slns;
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
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
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

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var g = new BidirectionalGraph<object, IEdge<object>>();

            SolutionSerializableToNameAndPath slnser = SolutionList.SelectedItem as SolutionSerializableToNameAndPath;
            Solution sln = slnser.Solution;

            ISet<string> displayedProjects = new HashSet<string>();
            Queue<Project> projectsToProcess = new Queue<Project>();



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
                    g.AddVertex(p.AssemblyName);
                }
                foreach (Project subp in p.Dependencies.ImplicitProjectDependencies)
                {
                    if (!displayedProjects.Contains(subp.ID))
                    {
                        g.AddVertex(subp.AssemblyName);
                        displayedProjects.Add(subp.ID);
                        projectsToProcess.Enqueue(subp);
                    }
                    g.AddEdge(new Edge<object>(p.AssemblyName, subp.AssemblyName));
                }
                foreach (ProjectReference subp in p.Dependencies.ReferencedProjectDependencies)
                {

                    if (sr.Projects.ContainsKey(subp.ID))
                    {
                        string pname = sr.Projects[subp.ID].AssemblyName;
                        if (!displayedProjects.Contains(subp.ID))
                        {
                            g.AddVertex(pname);
                            displayedProjects.Add(subp.ID);
                        }
                        g.AddEdge(new Edge<object>(p.AssemblyName, pname));
                    }
                }
            }

            foreach (var s in sr.Solutions)
            {
                var cross = s.Projects.Where((el, idx) => displayedProjects.Contains(el.ID));
                if (cross.Count() > 0)
                {
                    string sol = "SOLUTION:" + s.Name;
                    g.AddVertex(sol);
                    foreach (var p in cross)
                    {
                        g.AddEdge(new Edge<object>(sol, p.AssemblyName));
                    }
                }
            }


            _graphToVisualize = g;
            graphLayout.Graph = g;

        }

        private void graphLayout_MouseDown(object sender, MouseButtonEventArgs e)
        {
            zc.AnimationLength = TimeSpan.Zero;
        }

        private void graphLayout_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            //graphLayout.HighlightAlgorithm = 
        }
    }
}
