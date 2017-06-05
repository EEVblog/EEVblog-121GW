using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using System.Reflection;
using System.Resources;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System.Runtime.CompilerServices;

namespace rMultiplatform
{
    interface IChartRenderer
    {
        //Return true when redraw is required
        bool Draw(SKCanvas c);

        void SetParentSize(double w, double h);
    }


    public class Chart :
#if __ANDROID__
        SKGLView
#elif __IOS__
        SKGLView
#else
        SKCanvasView
#endif
    {
        double Aspect;
        List<IChartRenderer> ChartElements;
        public Chart ()
        {
            ChartElements = new List<IChartRenderer>();
            ChartElements.Add(new ChartAxis(10, 10, 20, 100, Height));

            VerticalOptions     = LayoutOptions.Fill;
            HorizontalOptions   = LayoutOptions.Fill;

            Aspect = 1.5;
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            var h = width / Aspect;
            HeightRequest = h;

            foreach (IChartRenderer Element in ChartElements)
                Element.SetParentSize(Width, Height);
        }

#if __ANDROID__
        protected override void OnPaintSurface(SKPaintGLSurfaceEventArgs e)
#elif __IOS__
        protected override void OnPaintSurface(SKPaintGLSurfaceEventArgs e)
#else
        protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
#endif
        {
            var canvas = e.Surface.Canvas;
            canvas.Scale(CanvasSize.Width / (float)Width);

            canvas.Clear();
            foreach (IChartRenderer Element in ChartElements)
            {
                //This allows controls to rescale retrospectively
                canvas.Save();
                while (Element.Draw(canvas))
                {
                    canvas.Restore();
                    canvas.Save();
                }
            }
        }
    }
}
