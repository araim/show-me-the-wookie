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
        private int duration = 1000;


        private Color c1;
        private Color c2;
        private Color c3;
        private bool fwd;

        public HighlightSpreadAnimation(TGraph g, Color start, Color mid, Color end,bool forward = true)
        {
            graph = g;
            c1 = start;
            c2 = mid;
            c3 = end;
            fwd = forward;
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

        private void AnimateFirst(EdgeControl edge, Color c1, Color c2, Color c3, int duration, int duration2)
        {
            var story = new Storyboard();
            var anim = new ColorAnimation(c1, c2, new Duration(TimeSpan.FromMilliseconds(duration)), FillBehavior.HoldEnd);
            anim.Completed += (sender, e) =>
            {
                AnimateSecond(edge, c2, c3, duration2);
                VertexControl v = fwd?edge.Target:edge.Source;
                foreach (var oe in graph.EdgesList.Keys)
                {
                   if ((fwd && oe.Source == v.Vertex) || (!fwd && oe.Target == v.Vertex))
                    {
                        AnimateEdgeForward(graph.EdgesList[oe]);
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
            AnimateFirst(target, c3, c2, c1, duration / 3, duration - duration / 3);
        }

        public void AnimateEdgeForward(EdgeControl target)
        {
            AnimateFirst(target, c1, c2, c3, duration / 3, duration - duration / 3);
        }

        void animation_Changed(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        public void AnimateVertexBackward(VertexControl target)
        {
            AnimateFirst(target, c3, c2, c1, duration / 3, duration - duration / 3);
            foreach (var oe in graph.EdgesList.Keys)
            {
                if ((fwd && oe.Source == target.Vertex) || (!fwd && oe.Target == target.Vertex))
                {
                    AnimateEdgeBackward(graph.EdgesList[oe]);
                }
            }
        }

        public void AnimateVertexForward(VertexControl target)
        {
            AnimateFirst(target, c1, c2, c3, duration / 3, duration - duration / 3);
            foreach (var oe in graph.EdgesList.Keys)
            {
                if ((fwd && oe.Source == target.Vertex) || (!fwd && oe.Target == target.Vertex))
                {
                    AnimateEdgeForward(graph.EdgesList[oe]);
                }
            }
        }


        double IBidirectionalControlAnimation.Duration
        {
            get
            {
                return ((double)duration) / 1000;
            }
            set
            {
                duration = (int)(value * 1000);
            }
        }
    }
}
