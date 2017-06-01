using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using System.Reflection;
using System.Resources;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System.Runtime.CompilerServices;

namespace rMultiplatform
{
    public class ChartGrid : IChartRenderer
    {
        private Padding     ParentPadding;
        private double      ParentWidth;
        private double      ParentHeight;

        public bool         EnableMajorLines;
        public bool         EnableMinorLines;

        public SKPaint      MajorPaint;
        public SKPaint      MinorPaint;

        public int          Layer
        {
            get
            {
                return 1;
            }
        }

        public              ChartGrid()
        {
            ParentPadding = new Padding(0);

            MajorPaint = new SKPaint();
            MajorPaint.StrokeWidth = 1;
            MajorPaint.BlendMode = SKBlendMode.Src;
            
            MinorPaint = new SKPaint();
            MinorPaint.StrokeWidth = 1;
            MinorPaint.BlendMode = SKBlendMode.Src;

            ParentWidth = 0;
            ParentHeight = 0;

            EnableMajorLines = true;
            EnableMinorLines = false;
        }
        public bool         Draw(SKCanvas c)
        {
            return false;
        }
        private void        DrawGridLine(SKCanvas c, Gridline o)
        {
            o.Draw(c);
        }
        public bool         ChartAxisEvent(Object o)
        {
            var     args = o as ChartAxisEventArgs;
            var     canvas = args.Canvas;
            SKPoint p1, p2;
            if (args.Orientation == ChartAxis.AxisOrientation.Horizontal)
                (p1, p2) = ParentPadding.GetVerticalLine((float)args.Position);
            else
                (p1, p2) = ParentPadding.GetHorozontalLine((float)args.Position);

            switch (args.EventType)     
            {
                case ChartAxisEventArgs.ChartAxisEventType.DrawMajorTick:
                    if (EnableMajorLines)
                    {
                        MajorPaint.ColorFilter = SKColorFilter.CreateBlendMode(args.Color, SKBlendMode.Dst);
                        MajorPaint.Color = args.Color;
                        MajorPaint.PathEffect = SKPathEffect.CreateDash(new[] {1f , 1f}, 0);
                        DrawGridLine(canvas, new Gridline(p1, p2, MajorPaint));
                    }
                    break;
                case ChartAxisEventArgs.ChartAxisEventType.DrawMinorTick:
                    if (EnableMinorLines)
                    {
                        MinorPaint.ColorFilter = SKColorFilter.CreateBlendMode(args.Color, SKBlendMode.DstOver);
                        MinorPaint.Color = args.Color;
                        MajorPaint.PathEffect = SKPathEffect.CreateDash(new[] { 1f, 1f }, 0);
                        DrawGridLine(canvas, new Gridline(p1, p2, MinorPaint));
                    }
                    break;
                default:
                    break;
            }

            //There may be multiple grids?
            return false;
        }
        public void         SetParentSize(double w, double h)
        {
            ParentWidth = w;
            ParentHeight = h;
        }
        public bool         Register(Object o)
        {
            if (o.GetType() == typeof(ChartAxis))          
            {
                var ax = (o as ChartAxis);
                ChartAxis.ChartAxisEvent t = ChartAxisEvent;
                ax.Register(t);
                return true;
            }
            else if (o.GetType() == typeof(Padding))
            {
                ParentPadding = o as Padding;
            }
            return true;
        }
        public List<Type>   RequireRegistration()
        {
            var Types = new List<Type>();

            //Types to register
            Types.Add(typeof(ChartAxis));
            Types.Add(typeof(Padding));

            //
            return Types;
        }
        public int          CompareTo(object obj)
        {
            if (obj is IChartRenderer)
            {
                var ob = obj as IChartRenderer;
                var layer = ob.Layer;

                if (layer > Layer)
                    return -1;
                else if (layer < Layer)
                    return 1;
                else
                    return 0;
            }
            return 0;
        }

        public bool RegisterParent(object c)
        {
            return false;
        }
        public void InvalidateParent()
        {
            throw new NotImplementedException();
        }
    }
}