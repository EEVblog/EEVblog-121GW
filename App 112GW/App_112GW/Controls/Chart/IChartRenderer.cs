using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace rMultiplatform
{
    public interface IChartRenderer : IComparable
    {
        int Layer
        {
            get;
        }
        List<IChartRenderer> Children { get; set; }

        //Return true when redraw is required
        void Draw(SKCanvas c, SKSize dimension);
    };

    public abstract class AChartRenderer : IChartRenderer
    {
        public int Layer => throw new NotImplementedException();

        public abstract List<IChartRenderer> Children { get; set; }
        public abstract int CompareTo(object obj);

        public void Draw(SKCanvas canvas, SKSize dimension)
        {
            foreach (var element in Children)
                element.Draw(canvas, dimension);
            DrawSelf(canvas, dimension);
        }
        public abstract void DrawSelf(SKCanvas c, SKSize dimension);
    }
}
