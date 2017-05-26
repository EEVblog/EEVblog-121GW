using System;
using System.Collections.Generic;
using System.Text;
using SkiaSharp;

namespace rMultiplatform
{
    class ChartData : IChartRenderer
    {
        List<SKPoint> Data;

        ChartData() { }

        public bool Draw(SKCanvas c)
        {
            var data = Data.ToArray();
            var path = new SKPath();
            path.AddPoly(data, false);

            return false;
        }

        public bool Register(object o)
        {
            return false;
        }
        public List<Type> RequireRegistration()
        {
            return null;
        }
        public void SetParentSize(double w, double h)
        {
        }
    }
}
