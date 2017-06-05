using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace App_112GW
{
    class BackButtonRenderer :
#if __ANDROID__
        SKGLView
#elif __IOS__
        SKGLView
#else
        SKCanvasView
#endif
    {
        private bool _Pressed;
        public bool Pressed
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
        public  SKPaint      IdleStyle
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
        public  SKPaint      PressStyle
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

        public  new SKColor BackgroundColor
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

        public  BackButtonRenderer()
        {
            IdleStyle = new SKPaint();
            PressStyle = new SKPaint();

            IdleStyle.StrokeJoin = SKStrokeJoin.Round;
            PressStyle.StrokeJoin = SKStrokeJoin.Round;

            IdleStyle.Style = SKPaintStyle.Stroke;
            PressStyle.Style = SKPaintStyle.Stroke;

            IdleStyle.StrokeCap = SKStrokeCap.Round;
            PressStyle.StrokeCap = SKStrokeCap.Round;


            points = new SKPoint[5]
            {
                new SKPoint((float)(0.5), (float)(0)),
                new SKPoint((float)(0), (float)(0.5)),
                new SKPoint((float)(1), (float)(0.5)),
                new SKPoint((float)(0), (float)(0.5)),
                new SKPoint((float)(0.5), (float)(1))
            };
            ShiftPoints(-0.5f, -0.5f);
            scaledpoints = new SKPoint[5];
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
            temp = PaddRectangle(temp, cwidth/2);
            return temp;
        }
        private SKRect FitRectange(SKRect Input)
        {
            return FitRectange(Input.Width, Input.Height);
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

                //Add points to paths
                var path = new SKPath();
                path.AddPoly(ScalePoints(PaddRectangle(can.ClipDeviceBounds,(float) Width/5)), false);

                //Draw path
                can.DrawPath(path, curStyle);
            }
        }
    }
    class BackButton : ContentView
    {
        private rMultiplatform.Touch mTouch;

        private enum eControlInputState
        {
            eNone,
            ePressed,
            eHover
        }
        private eControlInputState InputState = eControlInputState.eNone;

        private void MTouch_Press(object sender, rMultiplatform.TouchActionEventArgs args)
        {
            mRenderer.Pressed = true;
            InputState = eControlInputState.ePressed;
        }
        private void MTouch_Hover(object sender, rMultiplatform.TouchActionEventArgs args)
        {
            mRenderer.Pressed = true;
            InputState = eControlInputState.eHover;
        }
        private void MTouch_Release(object sender, rMultiplatform.TouchActionEventArgs args)
        {
            mRenderer.Pressed = false;

            if (InputState == eControlInputState.ePressed)
                OnClicked(this, EventArgs.Empty);

            InputState = eControlInputState.eNone;
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

        private BackButtonRenderer  mRenderer;
        public  SKPaint             IdleStyle
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
        public  SKPaint             PressStyle
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

        public int                  BorderWidth
        {
            set
            {
                IdleStyle.StrokeWidth = value;
                PressStyle.StrokeWidth = value;
            }
        }
        public Color                IdleColor
        {
            set
            {
                IdleStyle.Color = value.ToSKColor();
            }
        }
        public new Color            BackgroundColor
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
        public Color                PressColor
        {
            set
            {
                PressStyle.Color = value.ToSKColor();
            }
        }

        public event EventHandler   Clicked;
        private async void          OnClickedAsync()
        {
            mRenderer.Pressed = false;
            await System.Threading.Tasks.Task.Delay(100);
            if (Clicked != null)
                Clicked(this, EventArgs.Empty);
        }
        protected virtual void      OnClicked(object o, EventArgs e)
        {
            OnClickedAsync();
        }
        public BackButton           ()
        {
            HorizontalOptions = LayoutOptions.Start;
            VerticalOptions = LayoutOptions.Fill;

            mRenderer = new BackButtonRenderer();
            Content = mRenderer;

            SetupTouch();
        }
        
        //Keep control square
        protected override void     OnSizeAllocated(double width, double height)
        {
            width = height;
            base.OnSizeAllocated(width, height);
        }
    }
}
