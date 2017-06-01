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
    interface IChartRenderer : IComparable
    {
        int Layer
        {
            get;
        }

        bool            Register(Object o);
        List<Type>      RequireRegistration();

        //Return true when redraw is required
        bool Draw           (SKCanvas c);
        void SetParentSize  (double w, double h);

        bool RegisterParent(Object c);
        void InvalidateParent();
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
        //The padding around the control
        private SKPaint     mDrawPaint;
        private SKBitmap    mBitmap;
        private SKCanvas    mCanvas;

        private double _Aspect;
        public double Aspect
        {
            set
            {
                _Aspect = value;
            }
            get
            {
                return _Aspect;
            }
        }

        //Defines the padding around the boarder of the control
        private Padding _Padding;
        public Padding Padding
        {
            set
            {
                _Padding = value;
                InvalidateSurface();
            }
            get
            {
                return _Padding;
            }
        }

        //Stores all chart elements, this handles rendering too
        private List<IChartRenderer> ChartElements;

        //Wrappers for the supported chart elements
        private void AddElement(IChartRenderer pInput)
        {
            ChartElements.Add(pInput);
            ChartElements.Sort();
        }
        public void AddAxis(ChartAxis pInput)
        {
            AddElement(pInput as IChartRenderer);
        }
        public void AddData(ChartData pInput)
        {
            AddElement(pInput as IChartRenderer);
        }
        public void AddGrid(ChartGrid pInput)
        {
            AddElement(pInput as IChartRenderer);
        }

        //Resizes the control and registers resize with parents
        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            var h = width / Aspect;
            HeightRequest = h;

            //Null out the bitmap and canvas
            mBitmap?.Dispose();
            mBitmap = null;
            mCanvas?.Dispose();
            mCanvas = null;

            foreach (IChartRenderer Element in ChartElements)
                Element.SetParentSize(Width, Height);
        }

        //Renders the chart and child objects
        bool RequireRegister = true;
        protected void Register()
        {
            //Register all elements
            foreach (var Element in ChartElements)
            {
                //Register parent (this class) with children who need it
                Element.RegisterParent(this);

                //Get list of types to register
                var Types = Element.RequireRegistration();
                if (Types == null)
                    continue;

                //
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
            var canvas = e.Surface.Canvas;

            //Reinitialise the buffer canvas if it is undefined at all.
            if (mBitmap == null || mCanvas == null)
            {
                mBitmap = new SKBitmap((int)CanvasSize.Width, (int)CanvasSize.Height);
                mCanvas = new SKCanvas(mBitmap);
                mCanvas.Clear();
            }
            canvas.Scale(CanvasSize.Width / (float)Width);

            //If the child elements are not registered with each other do that
            // before rendering
            if (RequireRegister)
                Register();

            //Let all child elements render, layers are already sorted
            foreach ( var Element in ChartElements )
            {
                //This allows controls to rescale retrospectively'
                var layer = mBitmap.Copy();
                while (Element.Draw(mCanvas))
                {
                    mCanvas.DrawBitmap(layer, 0, 0, mDrawPaint);
                    layer = mBitmap.Copy();
                }
            }

            //Draw to canvas
            canvas.Clear(App_112GW.Globals.BackgroundColor.ToSKColor());
            canvas.DrawBitmap(mBitmap, 0, 0, mDrawPaint);
            mCanvas.Clear(App_112GW.Globals.BackgroundColor.ToSKColor());
        }

        //Initialises the object
        public Chart()
        {
            //Setup chart elements
            ChartElements = new List<IChartRenderer>();

            //Must always fill parent container
            VerticalOptions = LayoutOptions.Fill;
            HorizontalOptions = LayoutOptions.Fill;

            //Default aspect ratio 1:3
            Aspect = 3;

            //Default draw brush paints transparent
            var transparency = SKColors.Transparent;
            mDrawPaint = new SKPaint();
            mDrawPaint.BlendMode = SKBlendMode.SrcOver;
            mDrawPaint.ColorFilter = SKColorFilter.CreateBlendMode(transparency, SKBlendMode.DstOver);

            //Setup the padding object
            Padding = new Padding(0.1f);
            ChartElements.Add(Padding);
        }

    }
}
