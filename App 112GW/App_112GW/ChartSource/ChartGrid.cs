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
        private double      ParentWidth;
        private double      ParentHeight;

        public bool         EnableMajorLines;
        public bool         EnableMinorLines;

        public SKPaint MajorPaint;
        public SKPaint MinorPaint;

        public              ChartGrid()
        {
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
            {
                p1 = new SKPoint((float)args.Position, 0);
                p2 = new SKPoint((float)args.Position, (float)ParentHeight);
            }
            else
            {
                p1 = new SKPoint(0, (float)args.Position);
                p2 = new SKPoint((float)ParentWidth, (float)args.Position);
            }

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
            return true;
        }
        public List<Type>   RequireRegistration()
        {
            var Types = new List<Type>();

            //Types to register
            Types.Add(typeof(ChartAxis));

            //
            return Types;
        }
    }
}