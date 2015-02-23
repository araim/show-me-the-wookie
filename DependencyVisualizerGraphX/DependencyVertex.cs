using GraphX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyVisualizerGraphX
{
    internal class DependencyVertex:VertexBase
    {
        public string Text { get; set; }
        public string Color { get; set; }

        public override string ToString()
        {
            return Text;
        }

        public DependencyVertex()
            : this("")
        {
          Color = "Aqua";
        }

        public DependencyVertex(string text = "")
        {
            Text = text;
            Color = "Aqua";
        }
    }
}
