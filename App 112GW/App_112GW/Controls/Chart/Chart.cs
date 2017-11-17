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
using App_112GW;

namespace rMultiplatform
{
    public class Chart : GeneralView
    {
        GeneralRenderer mRenderer;
        public void Disable()
        {
            mRenderer = null;
            Content = null;
        }
        public void Enable()
        {
            mRenderer = new GeneralRenderer(PaintSurface);
            Content = mRenderer;
        }
        public new bool IsVisible
        {
            set
            {
                if (value)  Enable();
                else        Disable();

                base.IsVisible = value;
            }
        }

        //The padding around the control
        private SKPaint     mDrawPaint;

        //Defines the padding around the boarder of the control
        public new ChartPadding Padding
        {
            set
            {
                for (var i = 0; i < ChartElements.Count; i++)
                {
                    if (ChartElements[i].GetType() == typeof(ChartPadding))
                        ChartElements[i] =  value;
                }
                InvalidateSurface();
            }
            get
            {
                for (var a = 0; a < ChartElements.Count; a++)
                    if (a.GetType() == typeof(ChartPadding))
                        return ChartElements[a] as ChartPadding;
                return null;
            }
        }

        //Stores all chart elements, this handles rendering too
        private List<AChartRenderer> ChartElements;

        //Registers the change in ChartData if it exists
        public event ChartData.ListChanged DataChanged
        {
            add
            {
                foreach (var item in ChartElements)
                    if (item.GetType() == typeof(ChartData))
                        (item as ChartData).DataChanged += value;
            }
            remove
            {
                foreach (var item in ChartElements)
                    if (item.GetType() == typeof(ChartData))
                        (item as ChartData).DataChanged -= value;
            }
        }

        //Wrappers for the supported chart elements
        private void    AddElement(AChartRenderer pInput)
        {
            ChartElements.Add(pInput);
            ChartElements.Sort();
        }
        public void     AddAxis(ChartAxis pInput)
        {
            AddElement(pInput as AChartRenderer);
        }
        public void     AddData(ChartData pInput)
        {
            AddElement(pInput as AChartRenderer);
        }
        public void     AddGrid(ChartGrid pInput)
        {
            AddElement(pInput as AChartRenderer);
        }

        //Resizes the control and registers resize with parents
        bool Rescale = true;
        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            //Null out the bitmap and canvas
            Rescale = true;
        }

        public void SaveCSV()
        {
            string csv_string = "";
            foreach(var Element in ChartElements)
                if (Element.GetType() == typeof (ChartData))
                    csv_string = (Element as ChartData).GetCSV();

            Files.SaveFile(csv_string);
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
                var Types = Element.RequireRegistration;
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
        void UpdateCanvasSize(SKSize Size)
        {
            //As base class initialises first the onSizeAllocated can be triggered before padding is intiialised
            if (Padding != null)
                Padding.SetParentSize(Size.Width, Size.Height, Size.Width / Width);
            foreach (var Element in ChartElements)
                Element.SetParentSize(Size.Width, Size.Height, Size.Width / Width);
        }
        void PaintSurface(SKCanvas canvas, SKSize dimension)
        {
            //Reinitialise the buffer canvas if it is undefined at all.
            if (Rescale)
            {
                var aspect      = (float)(Height / Width);
                var can_aspect  = dimension.Height / dimension.Width;

                if (aspect * 0.9 <= can_aspect && can_aspect <= aspect * 1.1)
                {
                    //Handles glitch in android.
                    Rescale = false;
                    UpdateCanvasSize(dimension);
                }
                else return;
            }

            // If the child elements are not registered with each other do that
            // before rendering
            if (RequireRegister)
                Register();

            // Let all child elements render, layers are already sorted
            canvas.Clear(BackgroundColor.ToSKColor());
            foreach (var Element in ChartElements)
                Element.Draw(canvas);
        }

        public event EventHandler Clicked;
        private void OnClicked(EventArgs e)
        {
            Clicked?.Invoke(this, e);
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
            {
                return _State;
            }
            set
            {
                _State = value;
            }
        }
        private Touch mTouch;
        private void MTouch_Press   (object sender, rMultiplatform.TouchActionEventArgs args)
        {
            State = eControlInputState.ePressed;
        }
        private void MTouch_Hover   (object sender, rMultiplatform.TouchActionEventArgs args)
        {
            State = eControlInputState.eHover;
        }
        private void MTouch_Release (object sender, rMultiplatform.TouchActionEventArgs args)
        {
            State = eControlInputState.eNone;
        }
        private void SetupTouch()
        {
            //Add the gesture recognizer 
            mTouch = new Touch();
            mTouch.Tap          += MTouch_Tap;
            mTouch.Pressed      += MTouch_Press;
            mTouch.Hover        += MTouch_Hover;
            mTouch.Released     += MTouch_Release;
            mTouch.Pinch        += MTouch_Pinch;
            mTouch.Pan          += MTouch_Pan;
            Effects.Add(mTouch);
        }
        public event EventHandler FullscreenClicked;
        private void MTouch_Tap     (object sender, Touch.TouchTapEventArgs args)
        {
            FullscreenClicked?.Invoke(sender, EventArgs.Empty);
        }
        private void MTouch_Pan     (object sender, TouchPanActionEventArgs args)
        {
            foreach (var Element in ChartElements)
                if (Element.GetType() == typeof(ChartAxis))
                {
                    var element = (Element as ChartAxis);
                    if (element.Orientation == ChartAxis.AxisOrientation.Horizontal)
                        element.Pan(args.Dx, args.Dy);
                }
        }
        private void MTouch_Pinch   (object sender, TouchPinchActionEventArgs args)
        {
            var zoomX = (float)args.Pinch.ZoomX;
            var zoomY = (float)args.Pinch.ZoomY;
            var zoomCenter = args.Pinch.Center;

            foreach (var Element in ChartElements)
                if (Element.GetType() == typeof(ChartAxis))
                {
                    var element = (Element as ChartAxis);
                    if (element.Orientation == ChartAxis.AxisOrientation.Horizontal)
                        element.Zoom(zoomX, zoomY, zoomCenter.ToSKPoint());
                }
        }
        public void InvalidateSurface()
        {
            if (mRenderer != null)
                mRenderer.InvalidateSurface();
        }

        //Initialises the object
        public Chart() : base()
        {
            Enable();

            //Setup chart elements
            mDrawPaint              = new SKPaint();
            ChartElements           = new List<AChartRenderer>();

            //Setup the padding object
            ChartElements.Add(new ChartPadding(0));

            //Default draw brush paints transparent
            var transparency        = SKColors.Transparent;
            mDrawPaint.BlendMode    = SKBlendMode.SrcOver;
            mDrawPaint.ColorFilter  = SKColorFilter.CreateBlendMode(transparency, SKBlendMode.DstOver);

            //Setup touch input
            SetupTouch();
        }
    }
}
