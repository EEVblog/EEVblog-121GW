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
        bool            Register(Object o);
        List<Type>      RequireRegistration();

        //Return true when redraw is required
        bool Draw           (SKCanvas c);
        void SetParentSize  (double w, double h);
    };

    public class Chart :
#if __ANDROID__
        SKGLView
#elif __IOS__
        SKGLView
#else
        SKCanvasView
#endif
    {
        Random random = new Random();
        private float RandBetween(float min, float max)
        {
            var output = (float)random.NextDouble() * (max - min) + min;
            return output;
        }

        SKPaint     mDrawPaint;
        double      Aspect;
        public ChartData ChartData = new ChartData(ChartData.ChartDataMode.eRolling, "time(s)", "volts(V)", 0.1f, 10f);
        List<IChartRenderer> ChartElements;
        public Chart ()
        {
            ChartElements = new List<IChartRenderer>();
            ChartElements.Add(new ChartAxis(10, 5, 0, 40) { Label = "time(s)",    Orientation = ChartAxis.AxisOrientation.Horizontal, Direction = ChartAxis.AxisDirection.Standard, AxisLocation = 0.5, AxisStart = 0.1, AxisEnd = 0.05});
            ChartElements.Add(new ChartAxis(10, 5, -20, 20) { Label = "volts(V)",   Orientation = ChartAxis.AxisOrientation.Vertical,   Direction = ChartAxis.AxisDirection.Standard, AxisLocation = 0.1, AxisStart = 0.05, AxisEnd = 0.05 });

            
            ChartElements.Add(ChartData);
            ChartData.Sample(0);

            ChartElements.Add(new ChartGrid() { });

            VerticalOptions = LayoutOptions.Fill;
            HorizontalOptions   = LayoutOptions.Fill;

            Aspect = 3;

            var transparency = SKColors.Transparent;
            mDrawPaint = new SKPaint();
            mDrawPaint.BlendMode = SKBlendMode.SrcOver;
            mDrawPaint.ColorFilter = SKColorFilter.CreateBlendMode(transparency, SKBlendMode.DstOver);
        }

        public void Sample(float Value)
        {
            ChartData.Sample(Value);
            InvalidateSurface();
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            var h = width / Aspect;
            HeightRequest = h;

            foreach (IChartRenderer Element in ChartElements)
                Element.SetParentSize(Width, Height);
        }

        bool RequireRegister = true;
        protected void Register()
        {
            //Register all elements
            foreach (var Element in ChartElements)
            {
                //Get list of types to register
                var Types = Element.RequireRegistration();
                if (Types == null)
                    continue;

                foreach (var SubElement in ChartElements)
                {
                    //Skip self registration
                    if (Element.Equals(SubElement))
                        continue;

                    //Register if it is a required type
                    foreach (var RegType in Types)
                        if (RegType == SubElement.GetType())
                            Element.Register(SubElement);
                }
            }

            RequireRegister = false;
        }

#if __ANDROID__
        protected override void OnPaintSurface(SKPaintGLSurfaceEventArgs e)
#elif __IOS__
        protected override void OnPaintSurface(SKPaintGLSurfaceEventArgs e)
#else
        protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
#endif
        {
            if (RequireRegister)
                Register();

            var canvas = e.Surface.Canvas;
            canvas.Scale(CanvasSize.Width / (float)Width);
            canvas.Clear(App_112GW.Globals.BackgroundColor.ToSKColor());
            foreach ( var Element in ChartElements )
            {
                //This allows controls to rescale retrospectively
                var can_id = canvas.SaveLayer(null);
                while (Element.Draw(canvas))
                {
                    canvas.RestoreToCount(can_id);
                    can_id = canvas.SaveLayer(null);
                }
            }
        }
    }
}
