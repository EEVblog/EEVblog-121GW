using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using System.Reflection;
using System.Resources;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System.Runtime.CompilerServices;

using App_112GW;

namespace rMultiplatform
{
    public class ChartAxisEventArgs : EventArgs
    {
        public enum ChartAxisEventType
        {
            DrawMajorTick,
            DrawMinorTick,
            DrawLabel
        };

        public AxisLabel                    Label;
        public SKCanvas                     Canvas;
        public SKColor                      Color;
        public double                       Position;
        public ChartAxis.AxisOrientation    Orientation;
        public ChartAxisEventType           EventType;
        public float                        TickLength;

        public ChartAxisEventArgs(AxisLabel Label,  SKCanvas Can,    Color      Col,    double Pos,     double TickLen,     ChartAxis.AxisOrientation  Ori,     ChartAxisEventType Typ) :base()
        {
            this.Label = Label;
            TickLength = (float)TickLen;
            Canvas = Can;
            Color = Col.ToSKColor();
            Position = Pos;
            Orientation = Ori;
            EventType = Typ;
        }
        public ChartAxisEventArgs(AxisLabel Label,  SKCanvas Can,    SKColor    Col,    double Pos,     double TickLen,     ChartAxis.AxisOrientation  Ori,     ChartAxisEventType Typ) : base()
        {
            this.Label = Label;
            TickLength = (float)TickLen;
            Canvas = Can;
            Color = Col;
            Position = Pos;
            Orientation = Ori;
            EventType = Typ;
        } 
    }
    public class ChartAxisDrawEventArgs : EventArgs
    {
        public int                          Index;
        public int                          MaxIndex;
        public AxisLabel                    AxisLabel;
        public ChartAxis.AxisOrientation    EventType;
        public float                        Position;

        public ChartAxisDrawEventArgs(AxisLabel Label, ChartAxis.AxisOrientation EventType, float Position, int Index, int MaxIndex) : base()
        {
            this.AxisLabel = Label;
            this.EventType = EventType;
            this.Position = Position;
            this.Index = Index;
            this.MaxIndex = MaxIndex;
        }
    };

    public class ChartAxis : Range, IChartRenderer
    {
        public delegate bool ChartAxisDrawEvent(ChartAxisDrawEventArgs o);
        public delegate bool ChartAxisEvent(Object o);
        List<Range>                 AxisDataRanges;
        List<ChartAxisEvent>        AxisDataEvents;
        List<ChartAxisDrawEvent>    AxisDrawEvents;
        List<SKColor>               AxisDataColors;

        //Parent properties
        private ChartPadding _ParentPadding;
        private ChartPadding ParentPadding
        {
            set
            {
                _ParentPadding = value;
                CalculateScales();
            }
            get
            {
                return _ParentPadding;
            }
        }
            
        private double      ParentWidth;
        private double      ParentHeight;

        //Privates
        private SKPaint     MajorPaint;
        private SKPaint     ColorPaint;
        private SKPaint     MinorPaint;
        private SKPaint     MaskPaint;

        private float       SpaceWidth;
        private double      MainTickDrawDistance;
        private double      MinorTickDrawDistance;
        private void        CalculateScales()
        {
            var rangesize = Distance;
            var viewsize = AxisSize;
            var scale = viewsize / rangesize;

            //
            _MainTickDistance = Distance / _MainTicks;
            _MinorTickDistance = MainTickDistance / _MinorTicks;

            MainTickDrawDistance = Math.Abs(_MainTickDistance) * scale;
            MinorTickDrawDistance = Math.Abs(_MinorTickDistance) * scale;

            //
            AxisLocation    = _AxisLocationScaler;
        }

        //Properties
        private AxisLabel   _Label;
        public string       Label
        {
            get
            {
                return _Label.Text;
            }
            set
            {
                _Label = new AxisLabel(value);
            }
        }

        private double      _MainTickDistance;
        private double      MainTickDistance
        {
            get
            {
                return _MainTickDistance;
            }
        }
        private double      _MinorTickDistance;
        private double      MinorTickDistance
        {
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
        private double      _MinorTicks;
        public double       MinorTicks
        {
            get
            {
                return _MinorTicks;
            }
            set
            {
                _MinorTicks = value;
                CalculateScales();
            }
        }
        private double      _MainTicks;
        public double       MainTicks
        {
            get
            {
                return _MainTicks;
            }
            set
            {
                _MainTicks = value;
                CalculateScales();
            }
        }

        public enum         AxisDirection
        {
            Standard,
            Inverted
        }
        public              AxisDirection       Direction
        {
            get; set;
        }
        public enum         AxisOrientation
        {
            Vertical,
            Horizontal
        }
        public              AxisOrientation     Orientation
        {
            get;
            set;
        }

        private double      VisualScale
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
        private double      AltVisualScale
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
        private double      AxisSize
        {
            get
            {
                switch (Orientation)
                {
                    case AxisOrientation.Vertical:
                        return ParentPadding.H;
                    case AxisOrientation.Horizontal:
                        return ParentPadding.W;
                }
                return 0;
            }
        }
        double              AxisStart
        {
            get
            {
                switch (Orientation)
                {
                    case AxisOrientation.Vertical:
                        return ParentPadding.T;
                    case AxisOrientation.Horizontal:
                        return ParentPadding.L;
                }
                return 0;
            }
        }
        double              AxisEnd
        {
            get
            {
                switch (Orientation)
                {
                    case AxisOrientation.Vertical:
                        return ParentPadding.B;
                    case AxisOrientation.Horizontal:
                        return ParentPadding.R;
                }
                return 0;
            }
        }
        private double      _AxisLocationScaler;
        private float       __AxisLocation;
        private float       _AxisLocation
        {
            set
            {
                __AxisLocation = value;
                _AxisLocationScaler = value / AltVisualScale;
            }
            get
            {
                return __AxisLocation;
            }
        }
        public double       AxisLocation
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
        private double      StartPoint
        {
            get
            {
                if (MinorTickDistance < 0)
                    return Maximum;
                else
                    return Minimum;
            }
        }
        private double      EndPoint
        {
            get
            {
                if (MinorTickDistance < 0)
                    return Minimum;
                else
                    return Maximum;
            }
        }
        private bool        Inverting
        {
            get
            {
                if (Orientation == AxisOrientation.Vertical)
                {
                    if (Direction == AxisDirection.Standard)
                        return true;
                    else
                        return false;
                }
                else
                {
                    if (Direction == AxisDirection.Standard)
                        return false;
                    else
                        return true;
                }
            }
        }
        private bool        _Rerange;
        public bool         Rerange
        {
            set
            {
                _Rerange = value;
            }
        }
        public int          Layer
        {
            get
            {
                return 3;
            }
        }

        public Color        Color
        {
            set
            {
                MajorPaint.Color = value.ToSKColor();
                MinorPaint.Color = value.ToSKColor();
            }
        }
        public enum         AxisLock
        {
            eStart = 0,
            eMiddle = 999999998,
            eEnd = 999999999
        }
        public AxisLock     LockAlignment
        {
            set
            {
                LockIndex = (int)value;
            }
        }

        private AxisLabel   _LockAxisLabel = null;
        public string       LockToAxisLabel
        {
            set
            {
                _LockAxisLabel = new AxisLabel(value);
            }
            get
            {
                return _LockAxisLabel.Text;
            }
        }
        private bool        _LockIndexSet = false;
        private int         _LockIndex = 0;
        public int          LockIndex
        {
            set
            {
                _LockIndexSet = true;
                _LockIndex = value;
            }
            get
            {
                return _LockIndex;
            }
        }

        private float       _MajorTickLineSize;
        public float        MajorTickLineSize
        {
            set
            {
                _MajorTickLineSize = value;
            }
            get
            {
                return _MajorTickLineSize;
            }
        }
        private float       _MinorTickLineSize;
        public float        MinorTickLineSize
        {
            set
            {
                _MinorTickLineSize = value;
            }
            get
            {
                return _MinorTickLineSize;
            }
        }
        private float       _CircleRadius;
        public float        CircleRadius
        {
            set
            {
                _CircleRadius = value;
            }
            get
            {
                return _CircleRadius;
            }
        }

        public bool         ShowDataKey;

        //
        public ChartAxis       (double MainTicks, double MinorTicks, double Minimum, double Maximum)
            : base(Minimum, Maximum)
        {
            ParentPadding = new ChartPadding(0);

            //Setup divisions
            this.MainTicks = MainTicks;
            this.MinorTicks = MinorTicks;

            //
            MinorPaint = new SKPaint();
            MinorPaint.StrokeWidth = 1;

            MajorPaint = new SKPaint();
            MajorPaint.StrokeWidth = 4;
            MajorPaint.StrokeCap = SKStrokeCap.Round;
            MajorPaint.TextSize = 16;
            MajorPaint.FakeBoldText = true;

            ColorPaint = new SKPaint();
            ColorPaint.StrokeWidth = 4;
            ColorPaint.StrokeCap = SKStrokeCap.Round;
            ColorPaint.TextSize = 10;

            MaskPaint = new SKPaint();
            MaskPaint.Color = Globals.BackgroundColor.ToSKColor();

            //Setup unique color for axis
            Color = Globals.TextColor;

            //Calculate the width of a space charater
            SpaceWidth = MajorPaint.MeasureText(" ");

            //Must be set last as it depends on above
            AxisLocation = 0.5;
            CircleRadius = SpaceWidth * 2;
            MajorTickLineSize = 20;
            MinorTickLineSize = 5;

            //
            AxisDrawEvents = new List<ChartAxisDrawEvent>();
            AxisDataEvents = new List<ChartAxisEvent>();
            AxisDataColors = new List<SKColor>();
            AxisDataRanges = new List<Range>();
            Rerange = true;
            ShowDataKey = true;
        }
        public double       GetCoordinate   (double Value)
        {
            var scale = (AxisSize / Distance);

            //Handle axis inversion
            if (Inverting)
                scale = -scale;
            Value -= StartPoint;

            //Value now represents the full scale
            Value *= scale;

            //Offset from the base
            Value += AxisStart;
            
            return Value;
        }

        //Draws horozontal and vertical tick labels centred about the axis position
        void                SendEvent       (ref SKCanvas Can, SKColor Col, float Pos, int index, ChartAxisEventArgs.ChartAxisEventType EventType)
        {
            var args = new ChartAxisEventArgs(_Label, Can, Col, Pos, MinorTickDistance, Orientation, EventType);

            foreach (var registrant in AxisDataEvents)
                if (registrant(args))
                    break;

            if (EventType == ChartAxisEventArgs.ChartAxisEventType.DrawMajorTick)
            {
                var arg = new ChartAxisDrawEventArgs(_Label, Orientation, Pos, index, (int)MainTicks - 1);
                for (int i = 0; i < AxisDrawEvents.Count; i++)
                    if (!AxisDrawEvents[i](arg))
                        AxisDrawEvents.RemoveAt(i);
            }
        }
        
        public ChartDataEventReturn 
            ChartDataEvent(ChartDataEventArgs e)
        {
            if (e.Orientation == Orientation)
            {
                var temp = Combine(AxisDataRanges);

                Maximum = temp.Maximum;
                Minimum = temp.Minimum;
                CalculateScales();
                return new ChartDataEventReturn(GetCoordinate);
            }
            return null;
        }

        public bool         ChartDrawEvent  (ChartAxisDrawEventArgs e)
        {
            //No lock position set
            if (!_LockIndexSet)
                return false;

            //Positions can only be recieved from orthagonal axis
            if (Orientation == e.EventType)
                return false;

            //Lcok mode is not enabled
            if (_LockAxisLabel == null)
                return false;

            //Registered axis lable must match
            if (_LockAxisLabel.Text != e.AxisLabel.Text)
                return false;
            
            //Move axis positoin to lock position
            if (LockIndex > e.MaxIndex + 1)
            {
                if (LockIndex == (int)AxisLock.eMiddle)
                    LockIndex = e.MaxIndex / 2;
                else
                    LockIndex = e.MaxIndex + 1;
            }

            //Lock index doesn't match
            if (e.Index == LockIndex)
                if (_AxisLocation != e.Position)
                    _AxisLocation = e.Position;

            return true;
        }
        void                DrawTick        (ref SKCanvas c, float Position, float length, SKPaint TickPaint)
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
        void                DrawMajorTick   (ref SKCanvas c, float Position, int index)
        {
            SendEvent(ref c, MajorPaint.Color, Position, index, ChartAxisEventArgs.ChartAxisEventType.DrawMajorTick);
            DrawTick(ref c, Position, MajorTickLineSize, MajorPaint);
        }
        void                DrawMinorTick   (ref SKCanvas c, float Position)
        {
            SendEvent(ref c, MinorPaint.Color, Position, 0, ChartAxisEventArgs.ChartAxisEventType.DrawMinorTick);
            DrawTick(ref c, Position, MinorTickLineSize, MinorPaint);
        }
        void                DrawTickLabel   (ref SKCanvas c, double Value, float Position, float length, SKPaint TickPaint)
        {
            var hei = MajorPaint.TextSize /2;
            var txt = String.Format("{0:0.00}", Value);
            var wid = MajorPaint.MeasureText(txt) + SpaceWidth;
            var pth = new SKPath();
            SKPoint pt1, pt2;

            length /= 2;
            switch (Orientation)
            {
                case AxisOrientation.Vertical:
                    pt1 = new SKPoint(_AxisLocation - (wid + SpaceWidth), Position - hei);
                    pt2 = new SKPoint(_AxisLocation - (SpaceWidth),       Position - hei);
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
        void                DrawLabel       (ref SKCanvas c, float length)
        {
            var hei = (float)MajorPaint.TextSize;
            var txt = Convert.ToString(Label);
            var wid = MajorPaint.MeasureText(txt) + SpaceWidth;

            var pth = new SKPath();
            SKPoint pt1, pt2;

            float xmakoffset = 0, ymakoffset = 0, xfilloffset = 0, yfilloffset = 0;
            float xfillsoffset = 0, yfillsoffset = 0;
            var offset = MajorTickLineSize / 2.0f;

            length /= 2;
            switch (Orientation)
            {
                case AxisOrientation.Vertical:
                    {
                        offset += hei;
                        var x = _AxisLocation + offset;
                        var ys = (float)(AxisStart + 2 * SpaceWidth);

                        pt2 = new SKPoint(x, ys);
                        pt1 = new SKPoint(x, ys + wid);

                        xmakoffset = (CircleRadius + SpaceWidth * 2.0f);

                        xfilloffset = - hei;
                        yfilloffset = -SpaceWidth;

                        xfillsoffset = SpaceWidth;
                        yfillsoffset = SpaceWidth;
                    }
                    break;
                case AxisOrientation.Horizontal:
                    {
                        offset += hei / 2.0f;
                        var xs = (float)AxisEnd - SpaceWidth * 2;
                        var y = _AxisLocation - offset;

                        pt1 = new SKPoint(xs - wid, y);
                        pt2 = new SKPoint(xs,       y);

                        ymakoffset = - (CircleRadius + hei + SpaceWidth);

                        yfilloffset = -hei;
                        xfilloffset = SpaceWidth;

                        yfillsoffset = SpaceWidth;
                        xfillsoffset = -SpaceWidth;
                    }
                    break;
                default:
                    return;
            }

            var pts = new SKPoint[] { pt1, pt2 };
            pth.AddPoly(pts, false);
            var rect = new SKRect(pt1.X + xfillsoffset, pt1.Y + yfillsoffset, pt2.X + xfilloffset, pt2.Y + yfilloffset);

            c.DrawRoundRect(rect, SpaceWidth, SpaceWidth, MaskPaint);
            c.DrawTextOnPath(txt, pth, 0, 0, MajorPaint);
            //
            pth.Offset(xmakoffset, ymakoffset);
            pts = pth.Points;

            //
            if (ShowDataKey)
                DrawColors(ref c, ref pts);
        }
        void                DrawColors      (ref SKCanvas c, ref SKPoint[] p)
        {
            if (p.Length > 2)
                throw (new Exception("Must contain two points."));

            //
            var pt1 = p[0];
            var pt2 = p[1];
            var dx  = pt2.X - pt1.X;
            var dy  = pt2.Y - pt1.Y;

            //
            var t = 0.0f;
            var inc = 1.0f/((float)AxisDataColors.Count-1);

            //

            for (var i = 0; i < AxisDataColors.Count; i++)
            {
                var x = pt1.X + dx * t;
                var y = pt1.Y + dy * t;

                ColorPaint.IsStroke = true;
                ColorPaint.Color = App_112GW.Globals.BackgroundColor.ToSKColor();
                c.DrawCircle(x, y, CircleRadius, ColorPaint);

                ColorPaint.IsStroke = false;
                ColorPaint.Color = AxisDataColors[i];
                c.DrawCircle(x, y, CircleRadius, ColorPaint);

                t += inc;
            }
        }

        //Draws the entire system
        bool                IChartRenderer.Draw (SKCanvas c)
        {
            var redraw      = false;
            var NextMain    = AxisStart;
            var ercor       = MinorTickDrawDistance / 2;
            var cur         = StartPoint;

            int index = 0;

            //
            if (Label.Length > 0)
                DrawLabel(ref c, 20);

            for ( var Tick = AxisStart; Tick <= AxisEnd + ercor; Tick += MinorTickDrawDistance )
            {
                //This avoids multiple cast operations
                var pos = (float)Tick;

                //Need to add error compensation to avoid missing steps due to double maths issues
                if (Tick + ercor >= NextMain)
                {
                    //Draw main tick
                    DrawMajorTick(ref c, pos, index);
                    DrawTickLabel(ref c, cur, pos, 20, MajorPaint);

                    //Increment to anticipate the next main tick
                    NextMain += MainTickDrawDistance;
                    index++;
                }
                else
                    //
                    DrawMinorTick(ref c, pos);

                //
                cur += MinorTickDistance;
            }
            return redraw;
        }
        void                IChartRenderer.SetParentSize (double w, double h)
        {
            ParentWidth = w;
            ParentHeight = h;
            CalculateScales();
        }

        //Required 
        public bool         Register(Object o)
        {
            if      (o.GetType() == typeof(ChartAxisEvent))
                AxisDataEvents.Add(o as ChartAxisEvent);

            else if (o.GetType() == typeof(ChartAxis))
            {
                var v = o as ChartAxis;
                AxisDrawEvents.Add(v.ChartDrawEvent);
            }

            else if (o.GetType() == typeof(ChartData))
            {
                var d = o as ChartData;

                if (Orientation == AxisOrientation.Horizontal)
                {
                    if (d.HorizontalLabel == Label)
                        AxisDataColors.Add(d.LineColor);

                    //
                    AxisDataRanges.Add(d.HorozontalSpan);
                }
                else
                {
                    if (d.VerticalLabel == Label)
                        AxisDataColors.Add(d.LineColor);

                    //
                    AxisDataRanges.Add(d.VerticalSpan);
                }
            }

            else if (o.GetType() == typeof(ChartPadding))
                ParentPadding = o as ChartPadding;

            return true;
        }
        public List<Type>   RequireRegistration()
        {
            var lst = new List<Type>();
            lst.Add(typeof(ChartPadding));
            lst.Add(typeof(ChartAxis));
            lst.Add(typeof(ChartData));
            return lst;
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
        public bool         RegisterParent(object c)
        {
            return false;
        }
        public void         InvalidateParent()
        {
            throw new NotImplementedException();
        }
    }
}