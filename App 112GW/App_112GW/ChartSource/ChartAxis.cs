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
    public class ChartAxisEventArgs: EventArgs
    {
        public enum ChartAxisEventType
        {
            DrawMajorTick,
            DrawMinorTick,
            DrawLabel
        };

        public SKCanvas Canvas;
        public SKColor Color;
        public double Position;
        public ChartAxis.AxisOrientation   Orientation;
        public ChartAxisEventType EventType;
        public float TickLength;

        public ChartAxisEventArgs(SKCanvas Can, Color     Col, double     Pos,  double TickLen, ChartAxis.AxisOrientation  Ori, ChartAxisEventType Typ) :base()
        {
            TickLength = (float)TickLen;
            Canvas = Can;
            Color = Col.ToSKColor();
            Position = Pos;
            Orientation = Ori;
            EventType = Typ;
        }
        public ChartAxisEventArgs(SKCanvas Can, SKColor Col, double     Pos, double TickLen, ChartAxis.AxisOrientation  Ori, ChartAxisEventType Typ) : base()
        {
            TickLength = (float)TickLen;
            Canvas = Can;
            Color = Col;
            Position = Pos;
            Orientation = Ori;
            EventType = Typ;
        } 
    }
    public class ChartAxis: Range, IChartRenderer
    {
        public delegate bool ChartAxisEvent(Object o);
        List<ChartAxisEvent> Registrants;

        //Privates
        private SKPaint MajorPaint;
        private SKPaint MinorPaint;
        private float   SpaceWidth;
        private double  MainTickDrawDistance;
        private double  MinorTickDrawDistance;
        private double  ParentWidth;
        private double  ParentHeight;
        private void    CalculateScales()
        {
            var rangesize = Distance;
            var viewsize = AxisSize;

            var scale = viewsize / rangesize;

            MainTickDrawDistance = _MainTick * scale;
            MinorTickDrawDistance = _MinorTickDistance * scale;

            AxisStart = _AxisStartScaler;
            AxisEnd = _AxisEndScaler;
            AxisLocation = _AxisLocationScaler;
        }

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

        private double  _MinorTickDistance;
        public double   MinorTickDistance
        {
            set
            {
                _MinorTickDistance = value;
            }
            get
            {
                if (Orientation == AxisOrientation.Vertical)
                {
                    if (Direction == AxisDirection.Standard)
                        return -_MinorTickDistance;
                    else
                        return _MinorTickDistance;
                }
                else
                {
                    if (Direction == AxisDirection.Standard)
                        return _MinorTickDistance;
                    else
                        return -_MinorTickDistance;
                }

            }
        }
        public double   MinorTicks
        {
            get
            {
                return (double)(MainTick/_MinorTickDistance);
            }
            set
            {
                MinorTickDistance = MainTick / (double)value;
                CalculateScales();
            }
        }


        public enum AxisDirection
        {
            Standard,
            Inverted
        }
        public      AxisDirection Direction
        {
            get;set;
        }

        public enum AxisOrientation
        {
            Vertical,
            Horizontal
        }
        public AxisOrientation Orientation
        {
            get;
            set;
        }

        private double VisualScale
        {
            get
            {
                if (Orientation == AxisOrientation.Vertical)
                {
                    return ParentHeight;
                }
                else
                {
                    return ParentWidth;
                }
            }
        }
        private double AltVisualScale
        {
            get
            {
                if (Orientation == AxisOrientation.Vertical)
                {
                    return ParentWidth;
                }
                else
                {
                    return ParentHeight;
                }
            }
        }

        private double AxisSize
        {
            get
            {
                return VisualScale * ( 1.0 - ( _AxisStartScaler + _AxisEndScaler ) );
            }
        }

        private double _AxisStartScaler;
        private float _AxisStart;
        public double AxisStart
        {
            set
            {
                _AxisStartScaler = value;
                _AxisStart = (float)(VisualScale * _AxisStartScaler);
            }
            private get
            {
                return _AxisStart;
            }
        }
        
        private double _AxisEndScaler;
        private float _AxisEnd;
        public double AxisEnd
        {
            set
            {
                _AxisEndScaler = value;
                _AxisEnd = (float)(VisualScale * _AxisEndScaler);
            }
            private get
            {
                return _AxisEnd;
            }
        }

        private double _AxisLocationScaler;
        private float _AxisLocation;
        public double AxisLocation
        {
            set
            {
                _AxisLocationScaler = value;
                _AxisLocation = (float)(AltVisualScale * _AxisLocationScaler);
            }
            get
            {
                return _AxisLocation;
            }
        }

        private double StartPoint
        {
            get
            {
                if (MinorTickDistance < 0)
                    return Maximum;
                else
                    return Minimum;
            }
        }

        public ChartAxis (double MainTick, double MinorTicks, double Minimum, double Maximum) 
            : base(Minimum, Maximum)
        {
            //Setup divisions
            this.MainTick = MainTick;
            this.MinorTicks = MinorTicks;

            //Handle range error
            if (MainTick >= Distance)
                throw (new Exception("Cannot have ticks larger than the full range (Maximum - Minimum)"));

            //
            MinorPaint = new SKPaint();
            MinorPaint.Color = App_112GW.Globals.TextColor.ToSKColor();
            MinorPaint.StrokeWidth = 1;

            MajorPaint = new SKPaint();
            MajorPaint.Color = App_112GW.Globals.TextColor.ToSKColor();
            MajorPaint.StrokeWidth = 4;
            MajorPaint.StrokeCap = SKStrokeCap.Round;
            MajorPaint.TextSize = 16;

            //Calculate the width of a space charater
            SpaceWidth = MajorPaint.MeasureText(" ");

            //Must be set last as it depends on above
            AxisStart = 0.1;
            AxisEnd = 0.1;
            AxisLocation = 0.5;

            //
            Registrants = new List<ChartAxisEvent>();
        }

        void SendEvent(ref SKCanvas Can, SKColor Col, float Pos, ChartAxisEventArgs.ChartAxisEventType EventType)
        {
            var args = new ChartAxisEventArgs(Can, Col, Pos, MinorTickDistance, Orientation, EventType);
            foreach (var registrant in Registrants)
                if (registrant(args))
                    break;
        }

        //Draws horozontal and vertical tick labels centred about the axis position
        void DrawTick       (ref SKCanvas c, float Position, float length, SKPaint TickPaint)
        {
            length /= 2;
            switch (Orientation)      
            {
                case AxisOrientation.Vertical:
                    c.DrawLine(
                        _AxisLocation - length, //x1
                        Position,               //y1
                        _AxisLocation + length, //x2
                        Position,               //y2
                        TickPaint);
                    break;
                case AxisOrientation.Horizontal:
                    c.DrawLine(
                        Position,               //x1
                        _AxisLocation - length, //y1
                        Position,               //x2
                        _AxisLocation + length, //y2
                        TickPaint);
                    break;
                default:
                    break;
            }
        }
        void DrawMajorTick  (ref SKCanvas c, float Position)
        {
            SendEvent(ref c, MajorPaint.Color, Position, ChartAxisEventArgs.ChartAxisEventType.DrawMajorTick);
            DrawTick(ref c, Position, 20, MajorPaint);
        }
        void DrawMinorTick  (ref SKCanvas c, float Position)
        {
            SendEvent(ref c, MinorPaint.Color, Position, ChartAxisEventArgs.ChartAxisEventType.DrawMinorTick);
            DrawTick(ref c, Position, 5, MinorPaint);
        }
        void DrawTickLabel  (ref SKCanvas c, double Value, float Position, float length, SKPaint TickPaint)
        {
            var hei = MajorPaint.TextSize /2;
            var txt = Convert.ToString(Value);
            var wid = MajorPaint.MeasureText(txt) + SpaceWidth;
            var pth = new SKPath();
            SKPoint pt1, pt2;

            length /= 2;
            switch (Orientation)
            {
                case AxisOrientation.Vertical:

                    pt1 = new SKPoint(_AxisLocation + SpaceWidth,       Position - hei);
                    pt2 = new SKPoint(_AxisLocation + wid + SpaceWidth, Position - hei);

                    break;
                case AxisOrientation.Horizontal:

                    pt1 = new SKPoint(Position - hei,                   _AxisLocation + wid + SpaceWidth);
                    pt2 = new SKPoint(Position - hei,                   _AxisLocation + SpaceWidth);

                    break;
                default:
                    return;
            }
            var pts = new SKPoint[] { pt1, pt2 };
            pth.AddPoly(pts, false);
            c.DrawTextOnPath(txt, pth, 0, hei / 2, MajorPaint);
        }
        void DrawLabel      (ref SKCanvas c, float length)
        {
            var hei = (double)MajorPaint.TextSize;
            var txt = Convert.ToString(Label);
            var wid = MajorPaint.MeasureText(txt) + SpaceWidth;

            var pth = new SKPath();
            SKPoint pt1, pt2;

            length /= 2;
            switch (Orientation)
            {
                case AxisOrientation.Vertical:
                    {
                        var x = (float)(_AxisLocation - hei);
                        pt2 = new SKPoint(x, (float)(AxisStart));
                        pt1 = new SKPoint(x, (float)(AxisStart + wid));
                    }
                    break;
                case AxisOrientation.Horizontal:
                    {
                        var y = (float)(_AxisLocation - hei);
                        pt1 = new SKPoint((float)(VisualScale - AxisEnd - wid),               y);
                        pt2 = new SKPoint((float)(VisualScale - AxisEnd),   y);
                    }
                    break;
                default:
                    return;
            }
            var pts = new SKPoint[] { pt1, pt2 };
            pth.AddPoly(pts, false);
            c.DrawTextOnPath(txt, pth, 0, 0, MajorPaint);
        }

        bool IChartRenderer.Draw(SKCanvas c)
        {
            var redraw      = false;
            var NextMain    = AxisStart;
            var ercor       = MinorTickDrawDistance / 2;
            var cur         = StartPoint;

            if (Label.Length > 0)
                DrawLabel(ref c, 20);

            for ( var Tick = AxisStart; Tick <= (VisualScale - AxisEnd) + ercor; Tick += MinorTickDrawDistance )
            {
                //This avoids multiple cast operations
                var pos = (float)Tick;

                //Need to add error compensation to avoid missing steps due to double maths issues
                if (Tick + ercor >= NextMain)
                {
                    //Draw main tick
                    DrawMajorTick(ref c, pos);
                    DrawTickLabel(ref c, cur, pos, 20, MajorPaint);

                    //Increment to anticipate the next main tick
                    NextMain += MainTickDrawDistance;
                }
                else
                    //
                    DrawMinorTick(ref c, pos);

                //
                cur += MinorTickDistance;
            }
            return redraw;
        }
        void IChartRenderer.SetParentSize(double w, double h)
        {
            ParentWidth = w;
            ParentHeight = h;

            CalculateScales();
        }

        public bool Register(Object o)
        {
            if (o.GetType() != typeof(ChartAxisEvent))
                return false;

            Registrants.Add(o as ChartAxisEvent);
            return true;
        }
        public List<Type> RequireRegistration()
        {
            return null;
        }
    }
}