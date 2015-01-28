using GraphX;
using GraphX.Models.Interfaces;
using QuickGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace DependencyVisualizerGraphX
{
    class HighlightSpreadAnimation<TEdge, TVertex, TGraph> : IBidirectionalControlAnimation
        where TEdge : EdgeBase<TVertex>
        where TVertex : VertexBase
        where TGraph : GraphArea<TVertex, TEdge, BidirectionalGraph<TVertex, TEdge>>
    {
        private TGraph graph;
        private double duration = 1d;

        public HighlightSpreadAnimation(TGraph g)
        {
            graph = g;
        }

        public void AnimateEdge(GraphX.EdgeControl target)
        {
            var story = new Storyboard();
            var animation = new DoubleAnimation(1, 0, new Duration(TimeSpan.FromSeconds(1)), FillBehavior.Stop);
            animation.Completed += (sender, e) => { OnCompleted(target); };
            story.Children.Add(animation);
            Storyboard.SetTarget(animation, target as FrameworkElement);
            Storyboard.SetTargetProperty(animation, new PropertyPath(UIElement.OpacityProperty));
            story.Begin(target as FrameworkElement);

        }

        private void OnCompleted(GraphX.EdgeControl target)
        {
            TEdge e = (TEdge)target.Edge;
            TVertex v = e.Target;

            foreach (var oe in graph.EdgesList.Keys)
            {
                if (oe.Source == v)
                {
                    AnimateEdge(graph.EdgesList[oe]);
                }
            }
        }

        public void AnimateVertex(GraphX.VertexControl target)
        {
            foreach (var oe in graph.EdgesList.Keys)
            {
                if (oe.Source == target.Vertex)
                {
                    AnimateEdge(graph.EdgesList[oe]);
                }
            }
        }

        public event GraphX.Models.RemoveControlEventHandler Completed;

        public double Duration
        {
            get
            {
                return duration;
            }
            set
            {
                duration = value;
            }
        }
        private void AnimateSecond(EdgeControl edge, Color c1, Color c2, int duration)
        {
            var story = new Storyboard();
            var anim = new ColorAnimation(c1, c2, new Duration(TimeSpan.FromMilliseconds(duration)), FillBehavior.HoldEnd);
            story.Children.Add(anim);
            Storyboard.SetTarget(anim, edge as FrameworkElement);
            Storyboard.SetTargetProperty(anim, new PropertyPath("Foreground.Color"));
            story.Begin(edge as FrameworkElement);
        }

        private void AnimateFirst(EdgeControl edge,Color c1,Color c2,Color c3,int duration,int duration2)
        {
            var story = new Storyboard();
            var anim = new ColorAnimation(c1,c2,new Duration(TimeSpan.FromMilliseconds(duration)),FillBehavior.HoldEnd);
            anim.Completed += (sender, e) => { 
                AnimateSecond(edge,c2,c3, duration2);
                VertexControl v = edge.Target;
                foreach (var oe in graph.EdgesList.Keys)
                {
                    if (oe.Source == v.Vertex)
                    {
                        AnimateEdge(graph.EdgesList[oe]);
                    }
                }
            
            };
            story.Children.Add(anim);
            Storyboard.SetTarget(anim, edge as FrameworkElement);
            Storyboard.SetTargetProperty(anim, new PropertyPath("Foreground.Color"));
            story.Begin(edge as FrameworkElement);
        }

        private void AnimateSecond(VertexControl v, Color c1, Color c2, int duration)
        {
            var story = new Storyboard();
            var anim = new ColorAnimation(c1, c2, new Duration(TimeSpan.FromMilliseconds(duration)), FillBehavior.HoldEnd);
            story.Children.Add(anim);
            Storyboard.SetTarget(anim, v as FrameworkElement);
            Storyboard.SetTargetProperty(anim, new PropertyPath("Foreground.Color"));
            story.Begin(v as FrameworkElement);
        }

        private void AnimateFirst(VertexControl v, Color c1, Color c2, Color c3, int duration, int duration2)
        {
            var story = new Storyboard();
            var anim = new ColorAnimation(c1, c2, new Duration(TimeSpan.FromMilliseconds(duration)), FillBehavior.HoldEnd);
            anim.Completed += (sender, e) =>
            {
                AnimateSecond(v, c2, c3, duration2);               

            };
            story.Children.Add(anim);
            Storyboard.SetTarget(anim, v as FrameworkElement);
            Storyboard.SetTargetProperty(anim, new PropertyPath("Foreground.Color"));
            story.Begin(v as FrameworkElement);
        }
        public void AnimateEdgeBackward(EdgeControl target)
        {
            AnimateFirst(target, Color.FromRgb(99, 0, 0), Color.FromRgb(66, 0, 0), Color.FromRgb(00, 0, 0), 300, 700);
        }

        public void AnimateEdgeForward(EdgeControl target)
        {
            AnimateFirst(target, Color.FromRgb(0, 0, 0), Color.FromRgb(33, 0, 0), Color.FromRgb(99, 0, 0), 300, 700);
        }

        void animation_Changed(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        public void AnimateVertexBackward(VertexControl target)
        {
            AnimateFirst(target, Color.FromRgb(99, 0, 0), Color.FromRgb(66, 0, 0), Color.FromRgb(00, 0, 0), 300, 700);
            foreach (var oe in graph.EdgesList.Keys)
            {
                if (oe.Source == target.Vertex)
                {
                    AnimateEdgeBackward(graph.EdgesList[oe]);
                }
            }
        }

        public void AnimateVertexForward(VertexControl target)
        {
            AnimateFirst(target, Color.FromRgb(0, 0, 0), Color.FromRgb(33, 0, 0), Color.FromRgb(99, 0, 0), 300, 700);
            foreach (var oe in graph.EdgesList.Keys)
            {
                if (oe.Source == target.Vertex)
                {
                    AnimateEdgeForward(graph.EdgesList[oe]);
                }
            }
        }
    }
}
