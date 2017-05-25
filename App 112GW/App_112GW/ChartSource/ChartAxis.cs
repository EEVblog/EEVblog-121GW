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
    public class ChartAxis: Range, IChartRenderer
    {
        public enum AxisOrientation
        {
            Vertical,
            Horizontal
        }

        //Privates
        private SKPaint MajorPaint;
        private SKPaint MinorPaint;

        private float   SpaceWidth;
        private double  MainTickDrawDistance;
        private double  MinorTickDrawDistance;

        //Properties
        public string   _Label;
        public string   Label
        {
            get
            {
                return _Label;
            }
            set
            {
                _Label = value;
            }
        }

        private void    CalculateScales()
        {
            var rangesize = Distance;
            var viewsize = AxisSize;

            var scale = viewsize / rangesize;

            MainTickDrawDistance = MainTick * scale;
            MinorTickDrawDistance = MinorTickDistance * scale;
        }

        private double  _AxisSize;
        public double   AxisSize
        {
            set
            {
                _AxisSize = value - MajorPaint.TextSize * 2;

                //Recalculate DrawDistances
                CalculateScales();
            }
            private get
            {
                return _AxisSize;
            }
        }

        private double  _MainTick;
        public double   MainTick
        {
            get
            {
                return _MainTick;
            }
            set
            {
                _MainTick = value;
                CalculateScales();
            }
        }
        private double  MinorTickDistance;
        public double MinorTicks
        {
            get
            {
                return (double)(MainTick/MinorTickDistance);
            }
            set
            {
                MinorTickDistance = MainTick / (double)value;
                CalculateScales();
            }
        }

        public ChartAxis(double MainTick, double MinorTicks, double Minimum, double Maximum, double AxisSize) : base(Minimum, Maximum)
        {
            //Setup divisions
            this.MainTick = MainTick;
            this.MinorTicks = MinorTicks;

            //Handle range error
            if (MainTick >= Distance)
                throw (new Exception("Cannot have ticks larger than the full range (Maximum - Minimum)"));

            //
            MinorPaint = new SKPaint();
            MinorPaint.Color = SKColors.White;
            MinorPaint.StrokeWidth = 1;

            MajorPaint = new SKPaint();
            MajorPaint.Color = SKColors.White;
            MajorPaint.StrokeWidth = 4;
            MajorPaint.TextSize = 16;

            //Calculate the width of a space charater
            SpaceWidth = MajorPaint.MeasureText(" ");

            //Must be set last as it depends on above
            this.AxisSize = AxisSize;
        }


        //Perform the draw operation, this draws ticks based on orientation
        float MaxTextWidth = 1;
        bool IChartRenderer.Draw(SKCanvas c)
        {
            bool redraw = false;
            var offset = MaxTextWidth + SpaceWidth;
            double NextMain = 0;
            double cur = Minimum;
            var ercor = MinorTickDrawDistance / 2;
            var hei = MajorPaint.TextSize;

            for (double Tick = 0; Tick <= AxisSize + ercor; Tick += MinorTickDrawDistance)
            {
                //This avoids multiple cast operations
                var pos = (float)Tick;

                //Need to add error compensation to avoid missing steps due to double maths issues
                if (Tick + ercor >= NextMain)
                {
                    var txt = Convert.ToString(cur);
                    var wid = MajorPaint.MeasureText(txt);

                    //This measures text size, if it is larger than avaliable space then it will enlarge region
                    if (wid > MaxTextWidth)
                    {
                        MaxTextWidth = wid;
                        redraw = true;
                    }

                    //Draw main tick
                    c.DrawLine(offset, pos + hei, offset + 20, pos + hei, MajorPaint);
                    {
                        //Draw text
                        var pt1 = new SKPoint(offset - wid, pos + hei);
                        var pt2 = new SKPoint(offset, pos + hei);
                        var pts = new SKPoint[] { pt1, pt2 };
                        var pth = new SKPath();
                        pth.AddPoly(pts, false);
                        c.DrawTextOnPath(txt, pth, 0, hei/2, MajorPaint);
                    }

                    //Increment to anticipate the next main tick
                    NextMain += MainTickDrawDistance;
                }
                else
                    c.DrawLine(offset, pos + hei, offset + 5, pos + hei, MinorPaint);

                cur += MinorTickDistance;
            }
            return redraw;
        }

        void IChartRenderer.SetParentSize(double w, double h)
        {
            AxisSize = h;
        }
    }
}
