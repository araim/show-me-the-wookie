using GraphX;
using GraphX.Logic;
using QuickGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DependencyVisualizerGraphX.GraphDecorators
{
    internal interface IGraphDecorator<V, E, G, GL>
        where G : BidirectionalGraph<V, E>
        where GL : GXLogicCore<V, E, G>
        where E : EdgeBase<V>
        where V : VertexBase
    {
        void apply(GL glogic);
    }
}
