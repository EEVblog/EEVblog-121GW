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
    public class ChartGrid : AChartRenderer
    {
        public override int Layer
        {
            get
            {
                return 1;
            }
        }

        public delegate bool    ChartGridEvent(Object o);

        public  bool            EnableMajorLines;
        public  bool            EnableMinorLines;
        
        public                  ChartGrid() : base( new List<Type>() { typeof(ChartAxis), typeof(ChartPadding) })
        {
            EnableMajorLines = true;
            EnableMinorLines = false;
        }
        public override bool Draw(SKCanvas c)
        {
            return false;
        }
        private void DrawGridLine(SKCanvas c, Gridline o)
        {
            o.Draw(c);
        }
        public bool ChartAxisEvent(Object o)
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
                        MinorPaint.PathEffect = SKPathEffect.CreateDash(new[] { 1f, 1f }, 0);
                        DrawGridLine(canvas, new Gridline(p1, p2, MinorPaint));
                    }
                    break;
                default:
                    break;
            }

            //There may be multiple grids?
            return false;
        }
    }
}