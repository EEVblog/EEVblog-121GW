using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using System.Reflection;
using System.Resources;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System.Runtime.CompilerServices;
using System.Diagnostics;

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
        private double      _Aspect;
        public double       Aspect
        {
            set
            {
                _Aspect = value;
                InvalidateMeasure();
            }
            get
            {
                return _Aspect;
            }
        }

        //Defines the padding around the boarder of the control
        public ChartPadding Padding
        {
            set
            {
                for (var i = 0; i < ChartElements.Count; i++)
                {
                    if (ChartElements[i].GetType() == typeof(ChartPadding))
                    {
                        ChartElements[i] =  value;
                        InvalidateSurface();
                    }
                }

            }
            get
            {
                for (var a = 0; a < ChartElements.Count; a++)
                    if (a.GetType() == typeof(ChartPadding))
                    {
                        return ChartElements[a] as ChartPadding;
                    }
                return null;
            }
        }

        //Stores all chart elements, this handles rendering too
        private List<IChartRenderer> ChartElements;

        //Wrappers for the supported chart elements
        private void    AddElement(IChartRenderer pInput)
        {
            ChartElements.Add(pInput);
            ChartElements.Sort();
        }
        public void     AddAxis(ChartAxis pInput)
        {
            AddElement(pInput as IChartRenderer);
        }
        public void     AddData(ChartData pInput)
        {
            AddElement(pInput as IChartRenderer);
        }
        public void     AddGrid(ChartGrid pInput)
        {
            AddElement(pInput as IChartRenderer);
        }

        //Resizes the control and registers resize with parents
        bool Rescale = true;
        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            var h = width / Aspect;
            HeightRequest = h;

            //Null out the bitmap and canvas
            Rescale = true;
        }

        public void SaveCSV()
        {
            foreach(var Element in ChartElements)
                if (Element.GetType() == typeof (ChartData))
                    (Element as ChartData).ToCSV();
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
            if (mBitmap == null || mCanvas == null | Rescale)
            {
                //As base class initialises first the onSizeAllocated can be triggered before padding is intiialised
                if (Padding != null)
                    Padding.SetParentSize(CanvasSize.Width, CanvasSize.Height);

                foreach (IChartRenderer Element in ChartElements)
                    Element.SetParentSize(CanvasSize.Width, CanvasSize.Height);

                mBitmap = new SKBitmap((int)CanvasSize.Width, (int)CanvasSize.Height);
                mCanvas = new SKCanvas(mBitmap);
                Rescale = false;
            }
            //canvas.Scale(CanvasSize.Width / (float)Width);

            //If the child elements are not registered with each other do that
            // before rendering
            if (RequireRegister)
                Register();

            //Let all child elements render, layers are already sorted
            foreach ( var Element in ChartElements )
            {
                //This allows controls to rescale retrospectively'
                var layer = mBitmap.Copy();
                while ( Element.Draw(mCanvas) )
                {
                    mCanvas.DrawBitmap(layer, 0, 0, mDrawPaint);
                    layer = mBitmap.Copy();
                }
            }

            //Draw to canvas
            canvas.Clear();
            canvas.DrawBitmap(mBitmap, 0, 0, mDrawPaint);
            mCanvas.Clear(App_112GW.Globals.BackgroundColor.ToSKColor());
        }

        public event EventHandler Clicked;
        private void OnClicked(EventArgs e)
        {
            EventHandler handler = Clicked;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        public enum eControlInputState
        {
            eNone,
            ePressed,
            eHover
        }
        private eControlInputState _State;
        public eControlInputState State
        {
            get
            { return _State; }
            set
            {
                _State = value;
                InvalidateSurface();
            }
        }
        private Touch mTouch;
        private void MTouch_Press(object sender, rMultiplatform.TouchActionEventArgs args)
        {
            State = eControlInputState.ePressed;
            //ChangeColors();
        }
        private void MTouch_Hover(object sender, rMultiplatform.TouchActionEventArgs args)
        {
            State = eControlInputState.eHover;
            //ChangeColors();
        }
        private void MTouch_Release(object sender, rMultiplatform.TouchActionEventArgs args)
        {
            if (State == eControlInputState.ePressed)
                OnClicked(EventArgs.Empty);
            State = eControlInputState.eNone;
            //ChangeColors();
        }
        private void SetupTouch()
        {
            //Add the gesture recognizer 
            mTouch = new rMultiplatform.Touch();
            mTouch.Pressed += MTouch_Press;
            mTouch.Hover += MTouch_Hover;
            mTouch.Released += MTouch_Release;
            Effects.Add(mTouch);
        }
       
        
        //Initialises the object
        public Chart() : base()
        {
            //Must always fill parent container
            HorizontalOptions = LayoutOptions.Fill;
            VerticalOptions = LayoutOptions.StartAndExpand;

            //Setup chart elements
            mDrawPaint              = new SKPaint();
            ChartElements           = new List<IChartRenderer>();

            //Setup the padding object
            ChartElements.Add(new ChartPadding(0));

            //Default aspect ratio 1:3
            Aspect = 2;

            //Default draw brush paints transparent
            var transparency        = SKColors.Transparent;
            mDrawPaint.BlendMode    = SKBlendMode.SrcOver;
            mDrawPaint.ColorFilter  = SKColorFilter.CreateBlendMode(transparency, SKBlendMode.DstOver);

            //Setup touch input
            SetupTouch();
        }
    }
}
