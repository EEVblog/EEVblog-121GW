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
    class CheckboxRenderer:
#if __ANDROID__
        SKGLView
#elif __IOS__
        SKGLView
#else
        SKCanvasView
#endif
    {
        public event EventHandler Changed;
        protected virtual void OnChanged(EventArgs e)
        {
            EventHandler handler = Changed;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public bool                     Checked;
        public int                      CornerRadius;
        public Color                    BorderColor;
        public Color                    TextColor;

        private SKPaint                 mPaintStyle;
        private SKPaint                 mClearStyle;

        //rMultiplatform.Touch mTouch;
        TapGestureRecognizer mTapRecogniser;

        public CheckboxRenderer()
        {
            HorizontalOptions = LayoutOptions.End;
            VerticalOptions = LayoutOptions.Fill;

            //Setup responses to gestures
            mTapRecogniser = new TapGestureRecognizer();
            mTapRecogniser.Tapped += TapCallback;
            GestureRecognizers.Add(mTapRecogniser);

            //Setup defaults
            Checked = false;

            var temp = new Label();
            temp.BackgroundColor = Color.Default;
            temp.TextColor = Color.Default;

            TextColor = Color.Transparent;
            BackgroundColor = Color.FromRgb(127,127,127);

            CornerRadius = 0;
            mPaintStyle = new SKPaint()
            {
                Color = TextColor.ToSKColor(),
                Style = SKPaintStyle.Stroke
            };

            VerticalOptions = LayoutOptions.Fill;
            HorizontalOptions = LayoutOptions.End;

            
        }
        protected override SizeRequest OnMeasure(double width, double height)
        {
            return new SizeRequest(new Size(10, 10));
        }
        private void TapCallback(object sender, EventArgs args)
        {
            Checked = !Checked;
            OnChanged(EventArgs.Empty);
            InvalidateSurface();
        }
        private double Larger(double v1, double v2)
        {
            if (v1 > v2) return v1;
            if (v2 > v1) return v2;
            return v1;
        }
        private float Larger(float v1, float v2)
        {
            if (v1 > v2) return v1;
            if (v2 > v1) return v2;
            return v1;
        }
        private float Larger(SKRect pInput)
        {
            return Larger(pInput.Size.Width, pInput.Size.Height);
        }
        private float Larger(SKSize pInput)
        {
            return Larger(pInput.Width, pInput.Height);
        }
        bool IsSquare(SKRect pInput)
        {
            return pInput.Width == pInput.Height;
        }
        bool IsSquare(SKSize pInput)
        {
            return pInput.Width == pInput.Height;
        }

        bool CheckSquareOnce = true;
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
                if (!IsSquare(CanvasSize) && CheckSquareOnce)
                {
                    double larger = Larger(CanvasSize);
                    HeightRequest = larger;
                    WidthRequest = larger;
                    CheckSquareOnce = false;
                }

                SKRect temp = can.ClipBounds;
                mPaintStyle.StrokeWidth = (float)WidthRequest / 16.0f;

                temp.Left += mPaintStyle.StrokeWidth;
                temp.Top += mPaintStyle.StrokeWidth;
                temp.Right -= mPaintStyle.StrokeWidth;
                temp.Bottom -= mPaintStyle.StrokeWidth;

                can.Clear(BackgroundColor.ToSKColor());
                can.DrawRect(temp, mPaintStyle);
                
                if (Checked)
                {
                    var Pah = new SKPath();
                    SKPoint[] Pts = new SKPoint[]
                        {
                            new SKPoint((float)(Width * 330/1332), (float)(Height * 600/1332)),
                            new SKPoint((float)(Width * 600/1332), (float)(Height * 863/1332)),
                            new SKPoint((float)(Width * 1070/1332), (float)(Height * 390/1332))
                        };
                    Pah.AddPoly(Pts, false);
                    can.DrawPath(Pah, mPaintStyle);
                }
            }
        }
    }

    class Checkbox : Grid
    {
        public event EventHandler Changed;
        protected virtual void OnChanged(object o, EventArgs e)
        {
            EventHandler handler = Changed;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private CheckboxRenderer    mRenderer;
        private Label               mLabel;

        private void AddView(View pInput, int pX, int pY, int pXSpan = 1, int pYSpan = 1)
        {
            Children.Add(pInput);
            SetColumn(pInput, pX);
            SetRow(pInput, pY);

            SetColumnSpan(pInput, pXSpan);
            SetRowSpan(pInput, pYSpan);
        }
        public Checkbox(string pLabel)
        {
            mLabel = new Label(){
                Text = pLabel,
                HorizontalTextAlignment = TextAlignment.End,
                HorizontalOptions = LayoutOptions.Fill,
                VerticalTextAlignment = TextAlignment.Center,
                VerticalOptions = LayoutOptions.Fill
            };

            mRenderer = new CheckboxRenderer();
            mRenderer.Changed += OnChanged;
            //
            ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });

            RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

            AddView(mLabel, 0, 0);
            AddView(mRenderer, 1, 0);
        }
    }
}
