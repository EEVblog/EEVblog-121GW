using System;

using Xamarin.Forms;
using SkiaSharp;
using SkiaSharp.Views;
using SkiaSharp.Views.Forms;
using System.Collections.Generic;
using System.Text;
using App_112GW;
using System.Diagnostics;
using System.Threading;

namespace rMultiplatform
{
    using Point = SKPoint;
    using Points = List<SKPoint>;
    public class ChartDataEventArgs: EventArgs
    {
        public ChartAxis.AxisOrientation   Orientation;
        public Range NewRange;
        public ChartDataEventArgs(ChartAxis.AxisOrientation pOrientation, Range pNewRange)
        {
            Orientation = pOrientation;
            NewRange = pNewRange;
        }
    };
    public class ChartDataEventReturn
    {
        //public delegate double GetCoordinate(double Value);
        public delegate bool GetCoordinate(double Value, out double Output);
        public delegate SKMatrix GetTransform();
        public GetCoordinate    Function;
        public GetTransform     Transform;
        public ChartDataEventReturn(GetCoordinate pFunction)
        {
            Function = pFunction;
        }
        public ChartDataEventReturn(GetTransform pFunction)
        {
            Transform = pFunction;
        }
    }
    public class ChartData : IChartRenderer
    {
        public delegate ChartDataEventReturn ChartDataEvent(ChartDataEventArgs e, ref Range VisRange);
        public          List<ChartAxis> Registrants;

        //
        public void ToCSV()
        {
            DataMux.WaitOne();
            if (Data.Count > 1)
            {
                string output = "";
                foreach (var item in Data)
                    output += item.X.ToString() + ", " + item.Y.ToString() + "\r\n";
                Files.SaveFile(output);
            }
            DataMux.ReleaseMutex();
        }

        //
        public int      Layer
        {
            get
            {
                return 2;
            }
        }
        public enum     ChartDataMode
        {
            eRolling,
            eRescaling,
            eScreen
        }

        //
        private string  _HorizontalUnits;
        public string   HorizontalUnits
        {
            get { return _HorizontalUnits; }
        }
        private string  _HorizontalLabel;
        public string   HorizontalLabel
{
            get
            {
                return _HorizontalLabel + " ( " + _HorizontalUnits + " )";
            }
            set
            {
                var scts = value.Split('(', ')');
                var txt = scts[0];

                if (scts.Length == 1)
                    throw (new Exception("Must contain units in the following format 'Label(units)'."));

                var units = scts[1].Replace(" ", "");

                if (units.Length == 0)
                    throw (new Exception("Must contain units in the following format 'Label(units)'."));

                _HorizontalUnits = units;
                _HorizontalLabel = txt;
            }
        }

        //
        private string  _VerticalUnits;
        public string   VerticalUnits
        {
            get { return _VerticalUnits; }
        }
        private string  _VerticalLabel;
        public string   VerticalLabel
        {
            get
            {
                return _VerticalLabel + " ( " + _VerticalUnits + " )";
            }
            set
            {
                var scts = value.Split('(', ')');
                var txt = scts[0];

                if (scts.Length == 1)
                    throw (new Exception("Must contain units in the following format 'Label(units)'."));

                var units = scts[1].Replace(" ", "");

                if (units.Length == 0)
                    throw (new Exception("Must contain units in the following format 'Label(units)'."));

                _VerticalUnits = units;
                _VerticalLabel = txt;
            }
        }

        //
        private SKPaint DrawPaint;
        public float    LineWidth
        {
            set{ DrawPaint.StrokeWidth = value; }
        }
        public SKColor  LineColor
        {
            set { DrawPaint.Color = value; }
            get { return DrawPaint.Color; }
        }

        //
        Points _Data;
        public Range    HorozontalSpan;
        public Range    VerticalSpan;


        public delegate void ListChanged(List<Point> Data);
        public event ListChanged DataChanged;
        void DataChange()
        {
            InvalidateParent();
            DataChanged?.Invoke(Data);
        }
        public Points Data
        {
            get
            {
                return _Data;
            }
            set
            {
                DataMux.WaitOne();
                _Data = value;
                DataMux.ReleaseMutex();
            }
        }

        bool Combination = false;
        public void CombineDataRanges(ChartData A, ChartData B)
        {
            Combination = true;

            var horz = Range.Fit(A.HorozontalSpan, B.HorozontalSpan);
            var vert = Range.Fit(A.VerticalSpan, B.VerticalSpan);

            VerticalSpan.Set(vert.Minimum, vert.Maximum);
            HorozontalSpan.Set(horz.Minimum, horz.Maximum);

            InvalidateParent();
        }

        //
        float           TimeSpan;
        ChartDataMode   Mode;

        //
        public      ChartData (ChartDataMode pMode, string pHorzLabel, string pVertLabel, float pTimeSpan)
        {
            Mode = pMode;
            TimeSpan = pTimeSpan;

            //
            HorizontalLabel = pHorzLabel;
            VerticalLabel = pVertLabel;

            //
            Data        = new Points();
            Registrants = new List<ChartAxis>();

            //
            var col = Globals.UniqueColor;
            DrawPaint = new SKPaint () { Color = col.ToSKColor(), IsStroke = true, StrokeWidth = 2, IsAntialias = true };

            //
            HorozontalSpan  = new Range (0, pTimeSpan);
            VerticalSpan    = new Range (0, 0);
        }

        public bool Draw (SKCanvas c)
        {
            if (Data.Count == 0)
                return false;

            if (VerticalSpan == null)
                return false;

            //Scale vertical axis
            var horz = HorozontalSpan;
            var vert = VerticalSpan;

            //Rescale axis
            Range VisX = null;
            Range VisY = null;
            ChartDataEventReturn x = null, y = null, temp;
            foreach (var axis in Registrants)
            {
                if (        (temp = axis.ChartDataEvent(new ChartDataEventArgs(ChartAxis.AxisOrientation.Horizontal, horz), ref VisX)) != null)
                    x = temp;
                else if (   (temp = axis.ChartDataEvent(new ChartDataEventArgs(ChartAxis.AxisOrientation.Vertical, vert), ref VisY  )) != null)
                    y = temp;
            }

            //
            if (    x == null || 
                    y == null )
                throw (new Exception("Graph object must contain an horizontal and vertical axis to plot data."));

            uint DataStart   = ( uint ) Data.FindIndex        (val => (val.X >=    VisX.Minimum    ));
            uint DataEnd     = ( uint ) Data.FindIndex        (val => (val.X >     VisX.Maximum    ));
            uint Length      = ( uint ) Data.Count;
            bool Overflow = DataEnd > Length;
            if (Overflow)
                DataEnd = (uint)Data.Count;

            if (DataEnd > DataStart)
            {
                var output = Data.GetRange((int)DataStart, (int)(DataEnd - DataStart)).ToArray();
                var path = new SKPath();
                path.AddPoly(output, false);

                var y_t = y.Transform();
                var x_t = x.Transform();

                path.Transform(y_t);
                path.Transform(x_t);

                if (SetMode)
                    Debug.WriteLine("Stop");
                c.DrawPath(path, DrawPaint);
            }
            return false;
        }

        Mutex DataMux = new Mutex();
        DateTime start = DateTime.Now;
        public void Sample (double pPoint)
        {
            TimeSpan    t_diff  = DateTime.Now.Subtract(start);
            double      ms_diff = t_diff.TotalMilliseconds / 1000;
            switch (Mode)
            {
                case ChartDataMode.eRolling:
                    if (ms_diff > HorozontalSpan.Maximum)
                    {
                        DataMux.WaitOne();
                        if (Data.Count > 0)
                            Data.RemoveAt(0);
                        DataMux.ReleaseMutex();
                        HorozontalSpan.ShiftRangeToFitValue(ms_diff);
                    }
                    break;
                case ChartDataMode.eRescaling:
                    HorozontalSpan.RescaleRangeToFitValue(ms_diff);
                    break;
                case ChartDataMode.eScreen:
                    if (ms_diff > HorozontalSpan.Maximum)
                    {
                        DataMux.WaitOne();
                        Data.Clear();
                        DataMux.ReleaseMutex();
                        VerticalSpan.Set(0, 0);
                        HorozontalSpan.ShiftRange(HorozontalSpan.Distance);
                    }
                    break;
            };

            //Rescale vertical
            VerticalSpan.RescaleRangeToFitValue(pPoint);
            DataMux.WaitOne();
            Data.Add(new Point((float)ms_diff, (float)pPoint));
            DataMux.ReleaseMutex();
            DataChange();
        }

        bool SetMode = false;

        public (Range, Range) CalculateRanges(Points pPoints)
        {
            //Fast exit
            var count = pPoints.Count;
            switch(count)
            {
                case 0:
                    return (null, null);
                case 1:
                    {
                        var a = pPoints[0];
                        return (new Range(a.X, a.X), new Range(a.Y, a.Y));
                    }
                case 2:
                    {
                        var a = pPoints[0];
                        var b = pPoints[1];
                        return (new Range(a.X, b.X), new Range(a.Y, b.Y));
                    }
                default:
                    {
                        var a = pPoints[0];
                        var b = pPoints[count - 1];
                        var horz = new Range(a.X, b.X);
                        var vert = new Range(a.Y, a.Y);
                        foreach (var point in pPoints)
                        {
                            var y = point.Y;
                            if (y < vert.Minimum)
                                vert.Minimum = y;
                            else if (y > vert.Maximum)
                                vert.Maximum = y;
                        }
                        return (horz, vert);
                    }
            }
        }
        public void Set(Points pPoints)
        {
            if (pPoints.Count >= 2)
            {
                SetMode = true;

                (var horz, var vert) = CalculateRanges(pPoints);
                if (horz.Distance > 0 && vert.Distance > 0)
                {
                    VerticalSpan = vert;
                    HorozontalSpan = horz;

                    foreach (var axis in Registrants)
                    {
                        if (axis.Orientation == ChartAxis.AxisOrientation.Horizontal)
                            axis.Set(horz);
                        else
                            axis.Set(vert);
                    }

                    //Data cannot be changed when it is in use.
                    DataMux.WaitOne();
                    Data = pPoints;
                    DataMux.ReleaseMutex();
                    DataChange();
                }
            }
        }

        //Required functions by interface defition
        public bool Register (object o)
        {
            if (o.GetType() != typeof(ChartAxis))
                return false;

            //Add the object to registrants after testing whether axis is of relevant units
            var axis = o as ChartAxis;
            bool reg = false;
            if (axis.Orientation == ChartAxis.AxisOrientation.Horizontal)
                if (axis.Label == HorizontalLabel)
                    reg = true;
            if (axis.Orientation == ChartAxis.AxisOrientation.Vertical)
                if (axis.Label == VerticalLabel)
                    reg = true;
            if (reg)
                Registrants.Add(axis);
            return true;
        }
        public List<Type> RequireRegistration()
        {
            var Types = new List<Type>();
            Types.Add(typeof(ChartAxis));
            return Types;
        }
        public void SetParentSize(double w, double h, double scale)
        {
            //Doesn't do anything
        }

        //For the sortability of layers
        public int CompareTo(object obj)
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

        private Chart Parent;
        public bool RegisterParent(Object c)
        {
            if (c is Chart)
            {
                Parent = c as Chart;
                return true;
            }
            return false;
        }
        public void InvalidateParent()
        {
            if (Parent != null)
                Parent.InvalidateSurface();
        }
    }
}
