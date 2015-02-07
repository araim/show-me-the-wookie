using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyVisualizerGraphX
{
  public static class GraphKeeper
  {
    private static DependencyGraph _graph = null;

    internal static DependencyGraph Graph
    { 
      get 
      { 
        return _graph;
      }
      set
      {
        _graph = value;
      }
    }
  }
}
