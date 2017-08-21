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
    public class Chart : BaseRenderer
    {
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
                foreach (AChartRenderer item in ChartElements)
                    if (item.GetType() == typeof(ChartData))
                        (item as ChartData).DataChanged += value;
            }
            remove
            {
                foreach (AChartRenderer item in ChartElements)
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
            foreach(var Element in ChartElements)
                if (Element.GetType() == typeof (ChartData))
                    (Element as ChartData).ToCSV();
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
            foreach (AChartRenderer Element in ChartElements)
                if (Element.GetType() == typeof(ChartAxis))
                {
                    var element = (Element as ChartAxis);
                    if (element.Orientation == ChartAxis.Orientation.Horizontal)
                        element.Pan(args.Dx, args.Dy);
                }
        }
        private void MTouch_Pinch   (object sender, TouchPinchActionEventArgs args)
        {
            var zoomX = (float)args.Pinch.ZoomX;
            var zoomY = (float)args.Pinch.ZoomY;
            var zoomCenter = args.Pinch.Center;

            foreach (AChartRenderer Element in ChartElements)
                if (Element.GetType() == typeof(ChartAxis))
                {
                    var element = (Element as ChartAxis);
                    if (element.Orientation == ChartAxis.Orientation.Horizontal)
                        element.Zoom(zoomX, zoomY, zoomCenter.ToSKPoint());
                }
        }

        public override void PaintSurface(SKCanvas canvas, SKSize dimension)
        {
            //Reinitialise the buffer canvas if it is undefined at all.
            if (Rescale)
            {
                var aspect = (float)(Height / Width);
                var can_aspect = dimension.Height / dimension.Width;
            }

            // Let all child elements render, layers are already sorted
            canvas.Clear(BackgroundColor.ToSKColor());
            foreach (var Element in ChartElements)
                Element.Draw(canvas);
        }

        //Initialises the object
        public Chart() : base()
        {
            Enable();

            //Must always fill parent container
            HorizontalOptions       = LayoutOptions.Fill;
            VerticalOptions         = LayoutOptions.Fill;

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
            BackgroundColor = Globals.BackgroundColor;
        }
    }
}
