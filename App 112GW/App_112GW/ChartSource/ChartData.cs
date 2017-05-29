using System;
using System.Collections.Generic;
using System.Text;
using SkiaSharp;

namespace rMultiplatform
{
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
        public delegate double GetCoordinate(double Value);
        public GetCoordinate Function;
        public ChartDataEventReturn(GetCoordinate pFunction)
        {
            Function = pFunction;
        }
    }
    public class ChartData : IChartRenderer
    {
        public delegate ChartDataEventReturn ChartDataEvent(ChartDataEventArgs e);
        public List<ChartDataEvent> Registrants;

        public enum ChartDataMode
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
        List<SKPoint>   Data;
        Range           HorozontalSpan;
        Range           _VerticalSpan;
        Range           VerticalSpan
        {
            get
            {
                float? min = null, max = null;

                switch (Mode)
                {
                    //////////////////////////////////////////////////
                    case ChartDataMode.eRescaling:
                        return _VerticalSpan;
                    //////////////////////////////////////////////////
                    case ChartDataMode.eRolling:
                    case ChartDataMode.eScreen:
                        foreach (SKPoint pt in Data)
                        {
                            var val = pt.Y;
                            //Setup initial conditions
                            if (min == null)
                                min = val;
                            if (max == null)
                                max = val;
                            //Setup the range
                            if (val > max)
                                max = val;
                            if (val < min)
                                min = val;
                        }

                        //Setup no-data condition
                        if (min == null)
                            min = 0;
                        if (max == null)
                            max = 0;
                        return new Range((float)min, (float)max);
                    //////////////////////////////////////////////////
                    default:
                        return new Range(0,0);
                    //////////////////////////////////////////////////
                };
            }
        }


        //
        float           Time;

        //
        float           SampleTime;
        float           SampleDistance;

        //
        float           TimeSpan;
        ChartDataMode   Mode;

        public ChartData(ChartDataMode pMode, string pHorzLabel, string pVertLabel, float pSampleTime, float pTimeSpan)
        {
            Mode = pMode;
            TimeSpan = pTimeSpan;
            SampleTime = pSampleTime;

            //
            HorizontalLabel = pHorzLabel;
            VerticalLabel = pVertLabel;

            //
            Data = new List<SKPoint>();
            Registrants = new List<ChartDataEvent>();

            //
            HorozontalSpan = new Range(0, pTimeSpan);
            _VerticalSpan = new Range(0, 0);
        }

        public bool Draw (SKCanvas c)
        {
            //Scale vertical axis
            var vert = VerticalSpan;
            var horz = HorozontalSpan;

            //Rescale axis
            ChartDataEventReturn x = null, y = null, temp;
            foreach (var axis in Registrants)
            {
                if((temp = axis(new ChartDataEventArgs(ChartAxis.AxisOrientation.Horizontal, horz))) != null)
                    x = temp;
                if((temp = axis(new ChartDataEventArgs(ChartAxis.AxisOrientation.Vertical, vert))) != null)
                    y = temp;
            }

            //
            if (x == null || y == null)
                throw (new Exception("Graph object must contain an horizontal and vertical axis to plot data."));

            //
            var data = Data.ToArray();
            for (int i = 0; i < data.Length; i++)
            {
                data[i].X = (float)x.Function(data[i].X);
                data[i].Y = (float)y.Function(data[i].Y);
            }

            //
            var path = new SKPath();
            path.AddPoly(data, false);
            c.DrawPath(path, new SKPaint() { Color = SKColors.Red, IsStroke = true, StrokeWidth = 2});
            return false;
        }
        public void Sample (float pPoint)
        {
            switch (Mode)
            {
                case ChartDataMode.eRolling:
                    if (Time > HorozontalSpan.Maximum)
                    {
                        Data.RemoveAt(0);
                        HorozontalSpan.ShiftRange(SampleTime);
                    }
                    break;
                case ChartDataMode.eRescaling:
                    HorozontalSpan.RescaleRangeToFitValue(Time);
                    VerticalSpan.RescaleRangeToFitValue(pPoint);
                    break;
                case ChartDataMode.eScreen:
                    if (Time > HorozontalSpan.Maximum)
                    {
                        Data.Clear();
                        HorozontalSpan.ShiftRange(HorozontalSpan.Distance);
                    }
                    break;
            };
            Data.Add(new SKPoint(Time, pPoint));
            Time += SampleTime;
        }

        //Required functions by interface defition
        public bool Register (object o)
        {
            if (o.GetType() != typeof(ChartAxis))
                return false;

            //Add the object to registrants after testing whether axis is of relevant units
            var axis = o as ChartAxis;
            Registrants.Add(axis.ChartDataEvent);
            return true;
        }
        public List<Type> RequireRegistration()
        {
            var Types = new List<Type>();
            Types.Add(typeof(ChartAxis));
            return Types;
        }
        public void SetParentSize(double w, double h)
        {
            //Doesn't do anything
        }
    }
}
