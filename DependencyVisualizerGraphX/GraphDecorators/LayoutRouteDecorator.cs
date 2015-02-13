using GraphX;
using GraphX.Logic;
using QuickGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyVisualizerGraphX.GraphDecorators
{
    internal class LayoutRouteDecorator<V, E, G, GL> : IGraphDecorator<V, E, G, GL>
        where G : BidirectionalGraph<V, E>
        where GL : GXLogicCore<V, E, G>
        where E : EdgeBase<V>
        where V : VertexBase
    {

        private readonly LayoutAlgorithmTypeEnum algorithm;

        public LayoutRouteDecorator(LayoutAlgorithmTypeEnum algorithm)
        {
            this.algorithm = algorithm;
        }

        public void apply(GL glogic)
        {
            glogic.DefaultLayoutAlgorithm = algorithm;
        }

    }
}
