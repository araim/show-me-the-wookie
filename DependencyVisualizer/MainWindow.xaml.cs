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

        private ObservableCollection<string> slns = new ObservableCollection<string>();
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
            foreach (Solution sln in sr.Solutions)
            {
                slns.Add(sln.Name);
            }

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var g = new BidirectionalGraph<object, IEdge<object>>();

            string solution = SolutionList.SelectedItem.ToString();

            Solution sln = null;
            foreach (Solution s in sr.Solutions)
            {
                if (s.Name == solution)
                {
                    sln = s;
                    break;
                }
            }

            g.AddVertex(sln);
            foreach (var p in sln.Projects)
            {
                g.AddVertex(p);
                g.AddEdge(new Edge<object>(sln, p));
            }


            _graphToVisualize = g;
            graphLayout.Graph = g;
        }
    }
}
