using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace rMultiplatform
{
    interface IChartRenderer : IComparable
    {
        int Layer
        {
            get;
        }

        bool Register(Object o);
        List<Type> RequireRegistration();

        //Return true when redraw is required
        bool Draw(SKCanvas c);
        void SetParentSize(double w, double h, double scale = 1.0);
        bool RegisterParent(Object c);
        void InvalidateParent();
    };

    abstract class AChartRenderer : IChartRenderer
    {
        public int Layer => throw new NotImplementedException();

        public int CompareTo(object obj)
        {
            throw new NotImplementedException();
        }

        public bool Draw(SKCanvas c)
        {
            throw new NotImplementedException();
        }

        public void InvalidateParent()
        {
            throw new NotImplementedException();
        }

        public bool Register(object o)
        {
            throw new NotImplementedException();
        }

        public bool RegisterParent(object c)
        {
            throw new NotImplementedException();
        }

        public List<Type> RequireRegistration()
        {
            throw new NotImplementedException();
        }

        public void SetParentSize(double w, double h, double scale = 1)
        {
            throw new NotImplementedException();
        }
    }
}
