using GraphX;
using GraphX.Controls;
using GraphX.GraphSharp.Algorithms.EdgeRouting;
using GraphX.GraphSharp.Algorithms.Layout.Simple.FDP;
using GraphX.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using FirstFloor.ModernUI.Windows;

namespace DependencyVisualizerGraphX.Pages
{
  /// <summary>
  /// Interaction logic for Graph.xaml
  /// </summary>
  public partial class GraphPresenter : UserControl, IContent
  {
    

    private Color c1 = Color.FromRgb(0x33, 0x33, 0x33);
    private Color c2 = Color.FromRgb(0x66, 0x22, 0x22);
    private Color c3 = Color.FromRgb(0x99, 0x11, 0x11);

    private Color c4 = Color.FromRgb(0x33, 0x33, 0x33);
    private Color c5 = Color.FromRgb(0x22, 0x22, 0x66);
    private Color c6 = Color.FromRgb(0x11, 0x11, 0x99);

    public GraphPresenter()
    {
      InitializeComponent();
      DataContext = this;

      GraphArea_Setup();
      Area.VertexDoubleClick += Area_VertexDoubleClick;
    }

    private void GraphArea_Setup()
    {
      //Lets create logic core and filled data graph with edges and vertices
      var logicCore = new DependencyGraphLogicCore() { Graph = new DependencyGraph() };
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

    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
      Area.GenerateGraph(true, true);
      zoomctrl.ZoomToFill();
    }

    void Area_VertexDoubleClick(object sender, VertexSelectedEventArgs args)
    {
      ClearHighlight();

      new HighlightSpreadAnimation<DependencyEdge, DependencyVertex, DependencyGraphArea>(Area, c1, c2, c3).AnimateVertexForward(args.VertexControl);
      new HighlightSpreadAnimation<DependencyEdge, DependencyVertex, DependencyGraphArea>(Area, c4, c5, c6, false).AnimateVertexForward(args.VertexControl);

    }

    private void ClearHighlight()
    {

      foreach (VertexControl vc in Area.GetAllVertexControls())
      {
        vc.Foreground = new SolidColorBrush(c1); ;
      }
      foreach (EdgeControl ec in Area.EdgesList.Values)
      {
        ec.Foreground = new SolidColorBrush(c1); ;
      }
    }

    public void OnNavigatedFrom(FirstFloor.ModernUI.Windows.Navigation.NavigationEventArgs e)
    {
      if (GraphKeeper.Graph != null)
      {
        DrawNewGraph(GraphKeeper.Graph);
      }
    }


    private void DrawNewGraph(DependencyGraph graph)
    {
      Area.ClearLayout();
     
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


    public void OnFragmentNavigation(FirstFloor.ModernUI.Windows.Navigation.FragmentNavigationEventArgs e)
    {
      //throw new NotImplementedException();
    }

    

    public void OnNavigatedTo(FirstFloor.ModernUI.Windows.Navigation.NavigationEventArgs e)
    {
      if (GraphKeeper.Graph != null)
      {
        DrawNewGraph(GraphKeeper.Graph);
      }
    }

    public void OnNavigatingFrom(FirstFloor.ModernUI.Windows.Navigation.NavigatingCancelEventArgs e)
    {
      //throw new NotImplementedException();
    }
  }
}
