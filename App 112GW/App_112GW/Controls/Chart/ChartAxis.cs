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
using System.Diagnostics;

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
        public ChartAxis.AxisOrientation    Orientation;
        public float                        Position;

        public ChartAxisDrawEventArgs(AxisLabel Label, ChartAxis.AxisOrientation Orientation, float Position, int Index, int MaxIndex) : base()
        {
            this.AxisLabel = Label;
            this.Orientation = Orientation;
            this.Position = Position;
            this.Index = Index;
            this.MaxIndex = MaxIndex;
        }
    };

    public class ChartAxis : CappedRange, IChartRenderer
    {
        public delegate bool ChartAxisDrawEvent(ChartAxisDrawEventArgs o);
        public delegate bool ChartAxisEvent(Object o);

        double Dist (double A, double B)
        {
            if (A > B)
                return A - B;
            return B - A;
        }
        public void Pan(double X, double Y)
        {
            double dist = 1.0f;
            dist = -GetScalePoint((Orientation == AxisOrientation.Vertical) ? Y : X);
            Pan(dist);
            CalculateScales();
        }
        public void Zoom(double X, double Y, SKPoint About)
        {
            double zoom = 1.0f;
            double about = 0;
            if (Orientation == AxisOrientation.Vertical)
            {
                zoom = Y;
                about = GetPoint(About.Y);
            }
            else
            {
                zoom = X;
                about = GetPoint(About.X);
            }

            Debug.WriteLine("about : " + about.ToString());
            Debug.WriteLine("zoom : " + zoom.ToString());
            Debug.WriteLine(base.GetRange().String);
            Zoom(zoom, about);
            Debug.WriteLine(base.GetRange().String);
            Debug.WriteLine("\n\n");
            CalculateScales();
        }

        List<Range>                 AxisDataRanges;
        List<SKColor>               AxisDataColors;
        List<ChartAxisEvent>        AxisDataEvents;
        List<ChartAxisDrawEvent>    AxisDrawEvents;
        
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
        private bool Ready = false;
        public void        CalculateScales()
        {
            var rangesize = Distance;
            if (rangesize > 0)
            {
                var viewsize = AxisSize;
                if (viewsize > 0)
                {
                    var scale = viewsize / rangesize;
                    _MainTickDistance = rangesize / _MainTicks;
                    _MinorTickDistance = MainTickDistance / _MinorTicks;
                    MainTickDrawDistance = Math.Abs(_MainTickDistance) * scale;
                    MinorTickDrawDistance = Math.Abs(_MinorTickDistance) * scale;
                    AxisLocation = _AxisLocationScaler;

                    Ready = true;
                    return;
                }
            }
            return;
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
        public enum AxisOrientation
        {
            Vertical,
            Horizontal
        }
        public              AxisDirection       Direction
        {
            get; set;
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
                        return ParentPadding.PaddedHeight;
                    case AxisOrientation.Horizontal:
                        return ParentPadding.PaddedWidth;
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
                        return ParentPadding.GetTopPosition;
                    case AxisOrientation.Horizontal:
                        return ParentPadding.GetLeftPosition;
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
                        return ParentPadding.GetBottomPosition;
                    case AxisOrientation.Horizontal:
                        return ParentPadding.GetRightPosition;
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
                    return (Direction == AxisDirection.Standard);
                else
                    return (Direction != AxisDirection.Standard);
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

        public Color Color
        {
            set
            {
                MajorPaint.Color = value.ToSKColor();
                MinorPaint.Color = value.ToSKColor();
            }
        }
        public enum AxisLock
        {
            eStart  = 0,
            eMiddle = 999999998,
            eEnd    = 999999999
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
        float MajorTextSize = 12;
        float MinorTextSize = 10;
        public ChartAxis (double MainTicks, double MinorTicks, double Minimum, double Maximum)
            : base(Minimum, Maximum)
        {
            ParentPadding = new ChartPadding(0);

            //Setup divisions
            this.MainTicks = MainTicks;
            this.MinorTicks = MinorTicks;

            //
            MinorPaint = new SKPaint();
            MinorPaint.StrokeWidth = 1;
            MinorPaint.StrokeCap = SKStrokeCap.Round;
            MinorPaint.TextSize = MinorTextSize;
            MinorPaint.IsAntialias = true;
            MinorPaint.Typeface = SKTypeface.FromFamilyName("tahoma", SKTypefaceStyle.Normal);

            MajorPaint = new SKPaint();
            MajorPaint.StrokeWidth = 2;
            MajorPaint.StrokeCap = SKStrokeCap.Round;
            MajorPaint.TextSize = MajorTextSize;
            MajorPaint.IsAntialias = true;
            MajorPaint.Typeface = SKTypeface.FromFamilyName("tahoma", SKTypefaceStyle.Normal);

            ColorPaint = new SKPaint();
            ColorPaint.StrokeWidth = 4;
            ColorPaint.IsAntialias = true;
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
            MajorTickLineSize = 5;
            MinorTickLineSize = 3;

            //
            AxisDrawEvents  = new List<ChartAxisDrawEvent>();
            AxisDataEvents  = new List<ChartAxisEvent>();
            AxisDataColors  = new List<SKColor>();
            AxisDataRanges  = new List<Range>();

            //
            Rerange         = true;
            ShowDataKey     = false;
        }
        public SKMatrix GetTransform()
        {
            var scale = AxisSize / Distance;
            if (Inverting)
                scale = -scale;

            var matrix = SKMatrix.MakeIdentity();
            if (Orientation == AxisOrientation.Horizontal)
            {
                matrix.ScaleX = (float)(scale);
                matrix.TransX = (float)(AxisStart - scale * StartPoint);
            }
            else
            {
                matrix.ScaleY = (float)(scale);
                matrix.TransY = (float)(AxisStart - scale * StartPoint);
            }
            return matrix;
        }

        public double GetCoordinate(double Value)
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
        public void GetCoordinate(double Value, out double Output)
        {
            if (Minimum <= Value)
                Output = Minimum;
            if (Value >= Maximum)
                Output = Maximum;

            Output = GetCoordinate(Value);
        }
        public double GetScalePoint(double Value)
        {
            var scale = AxisSize / Distance;
            return Value /= scale;
        }
        public double GetPoint(double Value)
        {
            var scale = (AxisSize / Distance);
            double value = Value;
            value -= AxisStart;
            value /= scale;
            value += StartPoint;
            return value;
        }

        //Draws horozontal and vertical tick labels centred about the axis position
        void SendEvent (ref SKCanvas Can, SKColor Col, float Pos, int index, ChartAxisEventArgs.ChartAxisEventType EventType)
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
        
        public ChartDataEventReturn ChartDataEvent(ChartDataEventArgs e, ref Range VisRange)
        {
            if (e.Orientation == Orientation)
            {
                VisRange = GetRange();
                Set(Combine(AxisDataRanges));
                return new ChartDataEventReturn(GetTransform);
            }
            return null;
        }

        public new void Set(Range Input)
        {
            base.Set(Input);
            CalculateScales();
        }
        
        public bool ChartDrawEvent (ChartAxisDrawEventArgs e)
        {
            //No lock position set
            if (!_LockIndexSet)
                return false;

            //Positions can only be recieved from orthagonal axis
            if (Orientation == e.Orientation)
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

        void DrawTick (ref SKCanvas c, float Position, float length, SKPaint TickPaint)
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
        
        void DrawMajorTick (ref SKCanvas c, float Position, int index)
        {
            SendEvent(ref c, MajorPaint.Color, Position, index, ChartAxisEventArgs.ChartAxisEventType.DrawMajorTick);
            DrawTick(ref c, Position, MajorTickLineSize, MajorPaint);
        }
        
        void DrawMinorTick (ref SKCanvas c, float Position)
        {
            SendEvent(ref c, MinorPaint.Color, Position, 0, ChartAxisEventArgs.ChartAxisEventType.DrawMinorTick);
            DrawTick(ref c, Position, MinorTickLineSize, MinorPaint);
        }

        float   _LabelPadding = 0;
        float   _AxisLabelPadding = 0;
        float   TotalPadding
        {
            get
            {
                return _LabelPadding + _AxisLabelPadding;
            }
            set
            {
                if (value == 0)
                {
                    _LabelPadding = 0;
                    _AxisLabelPadding = 0;
                }
            }
        }
        float   LabelPadding
        {
            get
            {
                return _LabelPadding;
            }
            set
            {
                _LabelPadding = value;
                TotalPadding = _LabelPadding + _AxisLabelPadding;
            }
        }
        float   AxisLabelPadding
        {
            get
            {
                return _AxisLabelPadding;
            }
            set
            {
                _AxisLabelPadding = value;
                TotalPadding = _LabelPadding + _AxisLabelPadding;
            }
        }
        float   GetAxisLabelPosition()
        {
            switch (Orientation)
            {
                case AxisOrientation.Vertical:
                    return ParentPadding.GetLeftPosition;
                case AxisOrientation.Horizontal:
                    return ParentPadding.GetBottomPosition;
            }
            return 0;
        }

        void DrawTickLabel  (ref SKCanvas c, double Value, float Position, float length, SKPaint TickPaint)
        {
            var hei = MinorPaint.TextSize /2;
            var txt = String.Format("{0:0.00}", Value);
            var wid = MinorPaint.MeasureText(txt) + SpaceWidth;
            var pth = new SKPath();
            var tot_wid = wid + SpaceWidth*2;
            if (tot_wid > LabelPadding)
                LabelPadding = tot_wid;

            SKPoint pt1, pt2;
            length /= 2;
            switch (Orientation)
            {
                case AxisOrientation.Vertical:
                    if (TotalPadding > ParentPadding.L)
                        ParentPadding.L = TotalPadding;

                    pt1 = new SKPoint(GetAxisLabelPosition() - tot_wid, Position);
                    pt2 = new SKPoint(GetAxisLabelPosition() - SpaceWidth,   Position);
                    break;
                case AxisOrientation.Horizontal:
                    if (TotalPadding > ParentPadding.B)
                        ParentPadding.B = TotalPadding;

                    pt1 = new SKPoint(Position, GetAxisLabelPosition() + tot_wid);
                    pt2 = new SKPoint(Position, GetAxisLabelPosition() + SpaceWidth);
                    break;
                default:
                    return;
            }
            var pts = new SKPoint[] { pt1, pt2 };
            pth.AddPoly(pts, false);
            c.DrawTextOnPath(txt, pth, 0, hei / 2, MinorPaint);
        }
        void DrawLabel      (ref SKCanvas c, float length)
        {
            var hei = MajorPaint.TextSize;
            if (hei > AxisLabelPadding)
                AxisLabelPadding = hei + SpaceWidth;

            var txt = Convert.ToString(Label);
            var wid = MajorPaint.MeasureText(txt) + SpaceWidth;
            var pth = new SKPath();
            SKPoint pt1, pt2;

            float xmakoffset = 0, ymakoffset = 0, xfilloffset = 0, yfilloffset = 0;
            float xfillsoffset = 0, yfillsoffset = 0;
            var offset = 0;

            length /= 2;
            switch (Orientation)
            {
                case AxisOrientation.Vertical:
                    {
                        offset          = (int)LabelPadding + (int)SpaceWidth;
                        var x           = GetAxisLabelPosition() - offset;
                        var ys          = (float)(AxisStart + 2 * SpaceWidth);
                        pt2             = new SKPoint(x, ys);
                        pt1             = new SKPoint(x, ys + wid);
                        xmakoffset      = (CircleRadius + SpaceWidth * 2.0f);
                        xfilloffset     = - hei;
                        yfilloffset     = -SpaceWidth;
                        xfillsoffset    = SpaceWidth;
                        yfillsoffset    = SpaceWidth;
                    }
                    break;
                case AxisOrientation.Horizontal:
                    {
                        offset          = (int)TotalPadding - (int)SpaceWidth;
                        var xs          = (float)AxisEnd - SpaceWidth * 2;
                        var y           = GetAxisLabelPosition() + offset;
                        pt1             = new SKPoint(xs - wid, y);
                        pt2             = new SKPoint(xs,       y);
                        ymakoffset      = - (CircleRadius + hei + SpaceWidth);
                        yfilloffset     = -hei;
                        xfilloffset     = SpaceWidth;
                        yfillsoffset    = SpaceWidth;
                        xfillsoffset    = -SpaceWidth;
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
            DrawColors(ref c, ref pts);
        }
        void DrawColors     (ref SKCanvas c, ref SKPoint[] p)
        {
            if (ShowDataKey)
            {
                if (p.Length > 2)
                    throw (new Exception("Must contain two points."));

                //
                var pt1 = p[0];
                var pt2 = p[1];
                var dx  = pt2.X - pt1.X;
                var dy  = pt2.Y - pt1.Y;

                //
                var inc = 1.0f / (  - 1 );
                var t = 0.0f;
                for ( var i = 0; i < AxisDataColors.Count; i++ )
                {
                    var x = pt1.X + dx * t;
                    var y = pt1.Y + dy * t;

                    ColorPaint.IsStroke = true;
                    ColorPaint.Color    = Globals.BackgroundColor.ToSKColor();
                    c.DrawCircle(x, y, CircleRadius, ColorPaint);

                    ColorPaint.IsStroke = false;
                    ColorPaint.Color    = AxisDataColors[i];
                    c.DrawCircle(x, y, CircleRadius, ColorPaint);

                    t += inc;
                }
            }
        }

        //Draws the entire system
        bool IChartRenderer.Draw (SKCanvas c)
        {
            var redraw = false;
            if (Ready)
            {
                var NextMain = AxisStart;
                var ercor = MinorTickDrawDistance / 2;
                var cur = StartPoint;
                int index = 0;

                //
                if (Label.Length > 0)
                    DrawLabel(ref c, 20);

                for (var Tick = AxisStart; Tick <= AxisEnd + ercor; Tick += MinorTickDrawDistance)
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
                        DrawMinorTick(ref c, pos);

                    //
                    cur += MinorTickDistance;
                }
            }
            return redraw;
        }
        float Scale = 1.0f;
        void IChartRenderer.SetParentSize (double w, double h, double scale)
        {
            Scale               = (float)scale;
            MinorPaint.TextSize = MinorTextSize * Scale;
            MajorPaint.TextSize = MajorTextSize * Scale;
            SpaceWidth          = MajorPaint.MeasureText(" ");
            TotalPadding        = 0;
            ParentWidth         = w;
            ParentHeight        = h;
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
            Parent = c as Chart;
            return false;
        }

        private Chart Parent;
        public void InvalidateParent()
        {
            Parent.InvalidateSurface();
        }
    }
}