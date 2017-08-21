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

        List<IChartRenderer> Children { get; set; }

        //Return true when redraw is required
        void Draw(SKCanvas c, SKSize dimension);
        void DrawSelf(SKCanvas c, SKSize dimension);
    };

    abstract class AChartRenderer : IChartRenderer
    {
        public int Layer => throw new NotImplementedException();

        public List<IChartRenderer> Children { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public int CompareTo(object obj)
        {
            throw new NotImplementedException();
        }

        public void Draw(SKCanvas canvas, SKSize dimension)
        {
            foreach (var element in Children)
                element.Draw(canvas, dimension);
            DrawSelf(canvas, dimension);
        }
        public abstract void DrawSelf(SKCanvas c, SKSize dimension);
    }
}
