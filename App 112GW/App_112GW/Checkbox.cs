using System;
using System.Collections.Generic;
using System.Text;

using Xamarin.Forms;

using SkiaSharp;
using SkiaSharp.Views.Forms;
using System.Runtime.CompilerServices;
using App_112GW;

namespace rMultiplatform
{
    class CheckboxRenderer :
#if __ANDROID__
        SKGLView
#elif __IOS__
        SKGLView
#else
        SKCanvasView
#endif
    {
        public bool         Checked;
        public float        Scale = 0.5f;
        private bool        _Pressed;
        public bool         Pressed
        {
            get
            {
                return _Pressed;
            }
            set
            {
                _Pressed = value;
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

        private SKPoint[] points;
        private SKPoint[] scaledpoints;

        public CheckboxRenderer()
        {
            IdleStyle = new SKPaint();
            PressStyle = new SKPaint();

            IdleStyle.StrokeJoin = SKStrokeJoin.Round;
            PressStyle.StrokeJoin = SKStrokeJoin.Round;

            IdleStyle.Style = SKPaintStyle.Stroke;
            PressStyle.Style = SKPaintStyle.Stroke;

            IdleStyle.StrokeCap = SKStrokeCap.Round;
            PressStyle.StrokeCap = SKStrokeCap.Round;

            IdleStyle.IsAntialias = true;
            PressStyle.IsAntialias = true;


            points = new SKPoint[3]
            {
                new SKPoint((float)(0), (float)(0)),
                new SKPoint((float)(20), (float)(0)),
                new SKPoint((float)20, (float)(10))
            };
            ShiftPoints(-0.5f, -0.5f);
            scaledpoints = new SKPoint[3];
        }
        private void ShiftPoints(float x, float y)
        {
            for (int i = 0; i < points.Length; i++)
                points[i].Offset(x, y);
        }
        private SKPoint[] ScalePoints(SKRect Input)
        {
            float midx = Input.MidX;
            float midy = Input.MidY;

            for (int i = 0; i < points.Length; i++)
            {
                float X = points[i].X;
                float Y = points[i].Y;

                X *= Input.Width;
                Y *= Input.Height;

                X += midx;
                Y += midy;

                scaledpoints[i].X = X;
                scaledpoints[i].Y = Y;
            }
            return scaledpoints;
        }
        private SKRect PaddRectangle(SKRect pInput, float pPadding)
        {
            pInput.Left += pPadding;
            pInput.Top += pPadding;
            pInput.Right -= pPadding;
            pInput.Bottom -= pPadding;
            return pInput;
        }
        private SKRect FitRectange(float width, float height)
        {

            SKRect temp = new SKRect(0, 0, (float)width, (float)height);
            IdleStyle.StrokeWidth = (float)WidthRequest / 16.0f;
            var cwidth = IdleStyle.StrokeWidth;
            temp = PaddRectangle(temp, cwidth / 2);
            return temp;
        }
        private SKRect FitRectange(SKRect Input)
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
        private SKRect              GetRectangle(SKPoint[] pInput)
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
#if __ANDROID__
        protected override void OnPaintSurface(SKPaintGLSurfaceEventArgs e)
#elif __IOS__
        protected override void OnPaintSurface(SKPaintGLSurfaceEventArgs e)
#else
        protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
#endif
        {
            using (var can = e.Surface.Canvas)
            {
                var curStyle = (Pressed) ? PressStyle : IdleStyle;

                //Clear button
                can.Clear(BackgroundColor);

                //Draw border
                can.DrawRect(FitRectange(can.ClipDeviceBounds), curStyle);

                if (Checked)
                {
                    //Add points to paths
                    var path = new SKPath();
                    path.AddPoly(points, false);

                    //Move to centre origin to make rotations correct
                    var rect_inital = GetRectangle(path.Points);
                    (var xshft, var yshft) = GetMinimumPoint(path.Points);
                    path.Offset(-xshft, -yshft);
                    path.Offset(-rect_inital.Width / 2, -rect_inital.Height / 2);

                    //Rotate by 45 degrees
                    path.Transform(SKMatrix.MakeRotationDegrees(135));

                    //Offset to zero
                    //Scale to fill
                    var rect_scale = GetRectangle(path.Points);
                    (xshft, yshft) = GetMinimumPoint(path.Points);
                    path.Offset(-rect_scale.Left, -rect_scale.Top);

                    var xscale = (float)Width * Scale / rect_scale.Width;
                    var yscale = (float)Height * Scale / rect_scale.Height;
                    path.Transform(SKMatrix.MakeScale(xscale, yscale));
                    path.Offset((float)Width * Scale / 2, (float)Height * Scale / 2);

                    //Draw path
                    can.DrawPath(path, curStyle);
                }
            }
        }
    }

    class Checkbox : ContentView
    {
        private rMultiplatform.Touch mTouch;

        private enum eControlInputState
        {
            eNone,
            ePressed,
            eHover
        }
        private eControlInputState InputState = eControlInputState.eNone;

        private void    MTouch_Press(object sender, rMultiplatform.TouchActionEventArgs args)
        {
            mRenderer.Pressed = true;
            InputState = eControlInputState.ePressed;
        }
        private void    MTouch_Hover(object sender, rMultiplatform.TouchActionEventArgs args)
        {
            mRenderer.Pressed = true;
            InputState = eControlInputState.eHover;
        }
        private void    MTouch_Release(object sender, rMultiplatform.TouchActionEventArgs args)
        {
            mRenderer.Pressed = false;

            if (InputState == eControlInputState.ePressed)
                OnClicked(this, EventArgs.Empty);

            InputState = eControlInputState.eNone;
        }

        private bool    _Checked;
        public bool     Checked
        {
            get
            {
                return _Checked;
            }
        }

        private void    SetupTouch()
        {
            //Add the gesture recognizer 
            mTouch = new rMultiplatform.Touch();
            mTouch.Pressed += MTouch_Press;
            mTouch.Hover += MTouch_Hover;
            mTouch.Released += MTouch_Release;
            Effects.Add(mTouch);
        }

        private CheckboxRenderer mRenderer;
        public SKPaint IdleStyle
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
        public SKPaint PressStyle
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

        public int BorderWidth
        {
            set
            {
                IdleStyle.StrokeWidth = value;
                PressStyle.StrokeWidth = value;
            }
        }
        public Color IdleColor
        {
            set
            {
                IdleStyle.Color = value.ToSKColor();
            }
        }
        public new Color BackgroundColor
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
        public Color PressColor
        {
            set
            {
                PressStyle.Color = value.ToSKColor();
            }
        }

        public event EventHandler Clicked;
        private async void OnClickedAsync()
        {
            mRenderer.Pressed = false;
            await System.Threading.Tasks.Task.Delay(100);
            if (Clicked != null)
                Clicked(this, EventArgs.Empty);
        }
        protected virtual void OnClicked(object o, EventArgs e)
        {

            _Checked = !_Checked;
            mRenderer.Checked = _Checked;
            OnClickedAsync();
        }
        public Checkbox(string label)
        {
            HorizontalOptions = LayoutOptions.Start;
            VerticalOptions = LayoutOptions.Fill;

            mRenderer = new CheckboxRenderer();
            Content = mRenderer;

            _Checked = false;

            SetupTouch();
        }

        //Keep control square
        protected override void OnSizeAllocated(double width, double height)
        {
            width = height;
            WidthRequest = height;
            base.OnSizeAllocated(width, height);
        }
    }
}
