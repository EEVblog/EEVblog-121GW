using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace rMultiplatform
{
    public interface IChartRenderer
    {
        int Layer { get; }
        List<IChartRenderer> Children { get; set; }

        //Return true when redraw is required
        void Draw(SKCanvas c, SKSize dimension);
    };

    public abstract class AChartRenderer : IChartRenderer
    {
        public int Layer => throw new NotImplementedException();
        public abstract List<IChartRenderer> Children { get; set; }

        public          void Draw       (SKCanvas canvas, SKSize dimension)
        {
            foreach (var element in Children)
                element.Draw(canvas, dimension);
            DrawSelf(canvas, dimension);
        }
        public abstract void DrawSelf   (SKCanvas canvas, SKSize dimension);
    }
}
