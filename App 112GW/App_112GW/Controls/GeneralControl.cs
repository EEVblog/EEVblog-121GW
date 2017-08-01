using System;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Xamarin.Forms;
using SkiaSharp;
using SkiaSharp.Views.Forms;

using App_112GW;
namespace rMultiplatform
{
    public class GeneralControlRenderer :
#if __ANDROID__ && ! SOFTWARE_DRAW
        SKGLView
#elif __IOS__ && ! SOFTWARE_DRAW
        SKGLView
#else
        SKCanvasView
#endif
    {
        public bool         ShowPoly;
        public new float    Scale = 0.5f;

        public enum eControlInputState
        {
            eNone,
            ePressed,
            eHover
        }

        SKPaint curStyle = new SKPaint();
        private     eControlInputState _State;
        public      eControlInputState State
        {
            get
            {
                return _State;
            }
            set
            {
                curStyle = IdleStyle;
                switch (value)
                {
                    case eControlInputState.ePressed:
                        curStyle = PressStyle;
                        break;
                    case eControlInputState.eHover:
                        curStyle = HoverStyle;
                        break;
                }
                _State = value;
                InvalidateSurface();
            }
        }

        private SKPaint     _IdleStyle;
        public SKPaint      IdleStyle
        {
            get
            {
                return _IdleStyle;
            }
            set
            {
                _IdleStyle = value;
                InvalidateSurface();
            }
        }
        private SKPaint     _PressStyle;
        public SKPaint      PressStyle
        {
            get
            {
                return _PressStyle;
            }
            set
            {
                _PressStyle = value;
                InvalidateSurface();
            }
        }
        private SKPaint     _HoverStyle;
        public SKPaint      HoverStyle
        {
            get
            {
                return _HoverStyle;
            }
            set
            {
                _HoverStyle = value;
                InvalidateSurface();
            }
        }
        public new SKColor  BackgroundColor
        {
            set
            {
                base.BackgroundColor = value.ToFormsColor();
                InvalidateSurface();
            }
            get
            {
                return base.BackgroundColor.ToSKColor();
            }
        }

        public SKPoint[]    mPoints;
        private SKPoint[]   scaledpoints;

        public GeneralControlRenderer(SKPoint[] pPoints)
        {
            IdleStyle = new SKPaint();
            PressStyle = new SKPaint();
            HoverStyle = new SKPaint();

            IdleStyle.StrokeJoin = SKStrokeJoin.Round;
            PressStyle.StrokeJoin = SKStrokeJoin.Round;
            HoverStyle.StrokeJoin = SKStrokeJoin.Round;

            IdleStyle.Style = SKPaintStyle.Stroke;
            PressStyle.Style = SKPaintStyle.Stroke;
            HoverStyle.Style = SKPaintStyle.Stroke;

            IdleStyle.StrokeCap = SKStrokeCap.Round;
            PressStyle.StrokeCap = SKStrokeCap.Round;
            HoverStyle.StrokeCap = SKStrokeCap.Round;

            IdleStyle.IsAntialias = true;
            PressStyle.IsAntialias = true;
            HoverStyle.IsAntialias = true;

            OffsetAngle = 0;
            mPoints = pPoints;
            ShiftPoints(-0.5f, -0.5f);
            scaledpoints = new SKPoint[3];

            State = eControlInputState.eNone;
        }
        public void HidePoints()
        {
            ShowPoly = false;
            InvalidateSurface();
        }
        public void ShowPoints()
        {
            ShowPoly = true;
            InvalidateSurface();
        }
        public void SetPoints(SKPoint[] pPoints)
        {
            mPoints = pPoints;
            ShiftPoints(-0.5f, -0.5f);
            InvalidateSurface();
        }

        private void                ShiftPoints(float x, float y)
        {
            for (int i = 0; i < mPoints.Length; i++)
                mPoints[i].Offset(x, y);
        }
        private SKPoint[]           ScalePoints(SKRect Input)
        {
            float midx = Input.MidX;
            float midy = Input.MidY;

            for (int i = 0; i < mPoints.Length; i++)
            {
                float X = mPoints[i].X;
                float Y = mPoints[i].Y;

                X *= Input.Width;
                Y *= Input.Height;

                X += midx;
                Y += midy;

                scaledpoints[i].X = X;
                scaledpoints[i].Y = Y;
            }
            return scaledpoints;
        }
        private SKRect              PaddRectangle(SKRect pInput, float pPadding)
        {
            pInput.Left += pPadding;
            pInput.Top += pPadding;
            pInput.Right -= pPadding;
            pInput.Bottom -= pPadding;
            return pInput;
        }
        private SKRect              FitRectange(float width, float height)
        {

            SKRect temp = new SKRect(0, 0, (float)width, (float)height);
            IdleStyle.StrokeWidth = (float)WidthRequest / 16.0f;
            var cwidth = IdleStyle.StrokeWidth;
            temp = PaddRectangle(temp, cwidth / 2);
            return temp;
        }
        private SKRect              FitRectange(SKRect Input)
        {
            return FitRectange(Input.Width, Input.Height);
        }

        private (float x, float y)  GetMinimumPoint(SKPoint[] pInput)
        {
            float? xm = null, ym = null;
            foreach (var pt in pInput)
            {
                var x = pt.X;
                var y = pt.Y;

                if (xm == null)
                    xm = x;
                if (ym == null)
                    ym = y;

                if (x < xm)
                    xm = x;
                if (y < ym)
                    ym = y;
            }
            return ((float)xm, (float)ym);
        }
        private (float x, float y)  GetMaximumPoint(SKPoint[] pInput)
        {
            float? xm = null, ym = null;
            foreach (var pt in pInput)
            {
                var x = pt.X;
                var y = pt.Y;

                if (xm == null)
                    xm = x;
                if (ym == null)
                    ym = y;

                if (x > xm)
                    xm = x;
                if (y > ym)
                    ym = y;
            }
            return ((float)xm, (float)ym);
        }
        private SKRect GetRectangle(SKPoint[] pInput)
        {
            (var x1, var y1) = GetMinimumPoint(pInput);
            (var x2, var y2) = GetMaximumPoint(pInput);
            return new SKRect(x1, y1, x2, y2);
        }
        private float Larger(float a, float b)
        {
            if (a > b)
                return a;
            return b;
        }
        public float OffsetAngle
        {
            get;
            set;
        }

        protected override void InvalidateMeasure()
        {
            base.InvalidateMeasure();
        }

        SKPath path = new SKPath();
        float CanvasScaling = 1.0f;
        protected override void OnSizeAllocated(double width, double height)
        {
            //Add points to paths
            path.Reset();
            path.AddPoly(mPoints, false);

            //Move to centre origin to make rotations correct
            var rect_inital = GetRectangle(path.Points);
            (var xshft, var yshft) = GetMinimumPoint(path.Points);
            path.Offset(-xshft, -yshft);
            path.Offset(-rect_inital.Width / 2, -rect_inital.Height / 2);

            //Rotate by 45 degrees
            path.Transform(SKMatrix.MakeRotationDegrees(OffsetAngle));

            //Offset to zero
            //Scale to fill
            var rect_scale = GetRectangle(path.Points);
            (xshft, yshft) = GetMinimumPoint(path.Points);
            path.Offset(-rect_scale.Left, -rect_scale.Top);

            var xscale = (float)Width * Scale / rect_scale.Width;
            var yscale = (float)Height * Scale / rect_scale.Height;


            path.Transform(SKMatrix.MakeScale(xscale, yscale));
            path.Offset((float)Width * Scale / 2, (float)Height * Scale / 2);
            base.OnSizeAllocated(width, height);
        }

#if __ANDROID__ && ! SOFTWARE_DRAW
        protected override void OnPaintSurface(SKPaintGLSurfaceEventArgs e)
#elif __IOS__ && ! SOFTWARE_DRAW
        protected override void     OnPaintSurface(SKPaintGLSurfaceEventArgs e)
#else
        protected override void     OnPaintSurface(SKPaintSurfaceEventArgs e)
#endif
        {
            var Canvas = e.Surface.Canvas;
            Canvas.Clear(BackgroundColor);
            Canvas.DrawRect(FitRectange(Canvas.DeviceClipBounds), curStyle);
            if (ShowPoly)
            {
                if (CanvasSize.Width == 0)
                {
                    InvalidateMeasure();
                    return;
                }
                CanvasScaling = CanvasSize.Width / (float)Width;
                Canvas.Scale(CanvasScaling);
                Canvas.DrawPath(path, curStyle);
            }
        }
    }

    public class GeneralControl : ContentView
    {
        public GeneralControlRenderer                       mRenderer;
        private Touch                                       mTouch;

        private GeneralControlRenderer.eControlInputState   mState;
        bool                                                ShowPoly;
        float                                               OffsetAngle;

        private void        MTouch_Press   (object sender, TouchActionEventArgs args)
        {
            if (mRenderer != null)
                mRenderer.State = GeneralControlRenderer.eControlInputState.ePressed;
        }
        private void        MTouch_Hover   (object sender, TouchActionEventArgs args)
        {
            if (mRenderer != null)
                mRenderer.State = GeneralControlRenderer.eControlInputState.eHover;
        }
        private void        MTouch_Release (object sender, TouchActionEventArgs args)
        {
            if (mRenderer == null)
                return;
            
            if (mRenderer.State == GeneralControlRenderer.eControlInputState.ePressed)
                OnClicked(this, EventArgs.Empty);
            mRenderer.State = GeneralControlRenderer.eControlInputState.eNone;
        }

        private void        SetupTouch()
        {
            //Add the gesture recognizer 
            mTouch = new rMultiplatform.Touch();
            mTouch.Pressed += MTouch_Press;
            mTouch.Hover += MTouch_Hover;
            mTouch.Released += MTouch_Release;
            Effects.Add(mTouch);
        }
        public SKPaint      IdleStyle
        {
            set
            {
                mRenderer.IdleStyle = value;
            }
            get
            {
                return mRenderer.IdleStyle;
            }
        }
        public SKPaint      PressStyle
        {
            set
            {
                mRenderer.PressStyle = value;
            }
            get
            {
                return mRenderer.PressStyle;
            }
        }
        public SKPaint      HoverStyle
        {
            set
            {
                mRenderer.HoverStyle = value;
            }
            get
            {
                return mRenderer.HoverStyle;
            }
        }

        public int          BorderWidth
        {
            set
            {
                IdleStyle.StrokeWidth = value;
                PressStyle.StrokeWidth = value;
                HoverStyle.StrokeWidth = value;
            }
        }
        public Color        IdleColor
        {
            set
            {
                IdleStyle.Color = value.ToSKColor();
            }
        }
        public Color        PressColor
        {
            set
            {
                PressStyle.Color = value.ToSKColor();
            }
        }
        public Color        HoverColor
        {
            set
            {
                HoverStyle.Color = value.ToSKColor();
            }
        }
        public new Color    BackgroundColor
        {
            set
            {
                mRenderer.BackgroundColor = value.ToSKColor();
            }
            get
            {
                return mRenderer.BackgroundColor.ToFormsColor();
            }
        }

        protected event EventHandler Clicked;
        protected void      OnClicked(object o, EventArgs e)
        {
            Task.Delay(100).ContinueWith((obj) => { Device.BeginInvokeOnMainThread(() => { Clicked?.Invoke(this, EventArgs.Empty); }); });
        }

        SKPoint[] mPoints;
        public GeneralControl(SKPoint[] pPoints)
        {
            HorizontalOptions = LayoutOptions.Start;
            VerticalOptions = LayoutOptions.Fill;
            mPoints = pPoints;
            Enable();
            SetupTouch();
        }
        public void Disable()
        {
            if (mRenderer != null)
            {
                mState      = mRenderer.State;
                OffsetAngle = mRenderer.OffsetAngle;
                ShowPoly    = mRenderer.ShowPoly;
            }
            Content = null;
            mRenderer = null;
        }
        public void Enable()
        {
            mRenderer = new GeneralControlRenderer(mPoints);
            mRenderer.State = mState;
            mRenderer.OffsetAngle = OffsetAngle;
            mRenderer.ShowPoly = ShowPoly;
            mRenderer.SetPoints(mPoints);
            Content = mRenderer;

            BackgroundColor = Globals.BackgroundColor;
            BorderWidth = Globals.BorderWidth;
            PressColor = Globals.FocusColor;
            HoverColor = Globals.HighlightColor;
            IdleColor = Globals.TextColor;
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            width = height;
            WidthRequest = height;
            base.OnSizeAllocated(width, height);
        }
    }
}