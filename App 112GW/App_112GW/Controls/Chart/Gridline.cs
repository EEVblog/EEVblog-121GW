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
    public class GridlineEventArgs
    {
        public double                       Position;
        public ChartAxis.AxisOrientation    Orientation;
        public SKCanvas                     Canvas;

        public GridlineEventArgs (double Pos, ChartAxis.AxisOrientation Ori, SKCanvas Can)
        {
            Position = Pos;
            Orientation = Ori;
            Canvas = Can;
        }
    }

    public class Gridline : IChartRenderer
    {
        SKPoint Point1;
        SKPoint Point2;
        SKPaint Paint;



        public Color        Color
        {
            set
            {
                Paint.Color = value.ToSKColor();
            }
        }
        public int          Layer
        {
            get
            {
                return 0;
            }
        }
        public              Gridline(SKPoint p1, SKPoint p2, SKPaint paint)
        {
            Point1 = p1;
            Point2 = p2;
            Paint = paint;
        }
        public bool         Draw (SKCanvas c)
        {
            c.DrawLine(Point1.X, Point1.Y, Point2.X, Point2.Y, Paint);
            return true;
        }
        public void         SetParentSize (double w, double h){}
        public bool         Register (Object o)
        { return true; }
        public List<Type>   RequireRegistration(){return null;}
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

        public void         InvalidateParent()
        {
            throw new NotImplementedException();
        }
        public bool         RegisterParent(object c)
        {
            return false;
        }
    }
}