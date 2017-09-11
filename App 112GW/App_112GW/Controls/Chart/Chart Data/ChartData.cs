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
        public delegate SKMatrix GetTransform();
        public GetTransform     Transform;

        public ChartDataEventReturn(GetTransform pFunction)
        {
            Transform = pFunction;
        }
    }
    public class ChartData : AChartRenderer
    {
        public override int Layer
        {
            get
            {
                return 2;
            }
        }

        private Mutex DataMux = new Mutex();
        private DateTime DataStart = DateTime.Now;

        public delegate ChartDataEventReturn ChartDataEvent(ChartDataEventArgs e, ref Range VisRange);
        public          List<ChartAxis> Registrants;

        //
        public string GetCSV()
        {
            if (Data.Count > 1)
            {
                //The fallback values of axis labels are X, Y
                string horozontal_label = "X";
                string vertical_label   = "Y";

                //Get axis labels
                foreach (var axis in Registrants)
                {
                    if (axis.Orientation == ChartAxis.AxisOrientation.Vertical)
                        vertical_label = axis.Label;
                    else if (axis.Orientation == ChartAxis.AxisOrientation.Horizontal)
                        horozontal_label = axis.Label;
                }

                //The header row of the CSV
                string output = horozontal_label + ", " + vertical_label + "\r\n";

                //Print the rows of the CSV to the string.
                DataMux.WaitOne();
                foreach (var item in Data)
                    output += item.X.ToString() + ", " + item.Y.ToString() + "\r\n";
                DataMux.ReleaseMutex();

                //Return output ;) troll comment
                return output;
            }
            return "";
        }

        //
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
                _Data = value;
            }
        }

        public void CombineDataRanges(ChartData A, ChartData B)
        {
            var horz = Range.Fit(A.HorozontalSpan,  B.HorozontalSpan);
            var vert = Range.Fit(A.VerticalSpan,    B.VerticalSpan);

            VerticalSpan.Set(vert.Minimum, vert.Maximum);
            HorozontalSpan.Set(horz.Minimum, horz.Maximum);

            InvalidateParent();
        }

        //
        float           TimeSpan;
        ChartDataMode   Mode;

        //
        public ChartData(ChartDataMode pMode, string pHorzLabel, string pVertLabel, float pTimeSpan) : base(new List<Type>() { typeof(ChartAxis) })
        {
            //This filters when what children are added to this instance of ChartAxis
            RegistrationFilter = (
            (object o) => {
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

                return reg;
            });

            Mode = pMode;
            TimeSpan = pTimeSpan;

            //
            HorizontalLabel = pHorzLabel;
            VerticalLabel = pVertLabel;

            //
            Data        = new Points();
            Registrants = new List<ChartAxis>();

            //
            var col = Globals.UniqueColor();
            DrawPaint = new SKPaint () { Color = col.ToSKColor(), IsStroke = true, StrokeWidth = 2, IsAntialias = true };

            //
            HorozontalSpan  = new Range (0, pTimeSpan);
            VerticalSpan    = new Range (0, 0);
        }
        
        public void Reset()
        {
            HorozontalSpan.Rescale();
            VerticalSpan.Rescale();
            DataStart = DateTime.Now;
        }

        public override bool Draw (SKCanvas c)
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
                if ((temp = axis.ChartDataEvent(new ChartDataEventArgs(ChartAxis.AxisOrientation.Horizontal, horz), ref VisX)) != null)
                {
                    if (horz.Update)
                        axis.CalculateScales();
                    x = temp;
                }
                else if ((temp = axis.ChartDataEvent(new ChartDataEventArgs(ChartAxis.AxisOrientation.Vertical, vert),   ref VisY)) != null)
                {
                    if (vert.Update)
                        axis.CalculateScales();
                    y = temp;
                }
            }

            //
            if (    x == null || 
                    y == null )
                throw (new Exception("Graph object must contain an horizontal and vertical axis to plot data."));

            var DataStart   = Data.FindIndex (val => (val.X >=    VisX.Minimum    ));
            var DataEnd     = Data.FindIndex (val => (val.X >     VisX.Maximum    ));
            var Length      = Data.Count;

            if (DataStart < 0)
                DataStart = 0;
            if (DataEnd > Length || DataEnd < 0)
                DataEnd = Length;

            if (Data.Count > 0)
            {
                if (DataEnd > DataStart)
                {
                    var path = new SKPath();
                    var output = Data.GetRange(DataStart, (DataEnd - DataStart)).ToArray();
                    path.AddPoly(output, false);

                    var y_t = y.Transform();
                    var x_t = x.Transform();

                    path.Transform(y_t);
                    path.Transform(x_t);

                    c.DrawPath(path, DrawPaint);
                }
            }
            return false;
        }

        public void Sample (double pPoint)
        {
            TimeSpan t_diff  = DateTime.Now.Subtract(DataStart);
            double ms_diff = t_diff.TotalMilliseconds / 1000;
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
                case ChartDataMode.eRescaling:
                    HorozontalSpan.RescaleRangeToFitValue(ms_diff);
                    break;
            };

            //Rescale vertical
            VerticalSpan.RescaleRangeToFitValue(pPoint);
            Data.Add(new Point((float)ms_diff, (float)pPoint));
            DataChange();
        }
        public void SetVerticalRange(Range Vertical)
        {
            VerticalSpan.Set(Vertical);
            foreach (var axis in Registrants)
                if ((axis.Orientation == ChartAxis.AxisOrientation.Vertical))
                    axis.Set(VerticalSpan);
        }

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
        public void Set(Points pPoints, Range vert)
        {
            if (pPoints.Count >= 2)
            {
                var horz = new Range(pPoints[0].X, pPoints[pPoints.Count - 1].X);
                if (horz.Distance > 0 && vert.Distance > 0)
                {
                    //Set the ranges
                    VerticalSpan.Set(vert);
                    HorozontalSpan.Set(horz);

                    //
                    foreach (var axis in Registrants)
                        axis.Set((  axis.Orientation == ChartAxis.AxisOrientation.Horizontal)?
                                    horz: 
                                    vert);

                    //Data cannot be changed when it is in use.
                    Data = pPoints;
                    DataChange();
                }
            }
        }
    }
}
