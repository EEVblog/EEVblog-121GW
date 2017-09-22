using Xamarin.Forms;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using static rMultiplatform.GeneralControl;
using System;
using System.Diagnostics;

namespace rMultiplatform
{
    public abstract class ASmartElement
    {
        static public SKPaint MakeDefaultPaint(Color pColor, float pStrokeWidth, float pFontSize, SKTypeface pTypeface, bool Dotted = false, bool IsStroke = false)
        {
            var output = new SKPaint()
            {
                Color = pColor.ToSKColor(),
                StrokeWidth = pStrokeWidth,
                Typeface = pTypeface,
                TextSize = pFontSize,
                StrokeCap = SKStrokeCap.Round,
                BlendMode = SKBlendMode.Src,
                IsStroke = IsStroke,
                IsAntialias = true
            };
            if (Dotted)
            {
                output.ColorFilter = SKColorFilter.CreateBlendMode(pColor.ToSKColor(), SKBlendMode.Dst);
                output.PathEffect = SKPathEffect.CreateDash(new[] { 1f, 1f }, 0);
            }
            return output;
        }

        static private  SKPaint _MajorPaint  = MakeDefaultPaint(Globals.TextColor,       1,  Globals.MajorFontSize,  Globals.Typeface);
        static private  SKPaint _MinorPaint  = MakeDefaultPaint(Globals.TextColor,       1,  Globals.MinorFontSize,  Globals.Typeface);
        static private  SKPaint _MaskPaint   = MakeDefaultPaint(Globals.BackgroundColor, 1,  Globals.MinorFontSize,  Globals.Typeface);
        static private  SKPaint _GridPaint   = MakeDefaultPaint(Globals.TextColor,       1,  Globals.MinorFontSize,  Globals.Typeface, Dotted:true);

        static public   SKPaint MajorPaint
        {
            get
            {
                return _MajorPaint;
            }
            private set
            {
                _MajorPaint = value;
            }
        }
        static public   SKPaint MinorPaint
        {
            get
            {
                return _MinorPaint;
            }
            private set
            {
                _MinorPaint = value;
            }
        }
        static public   SKPaint MaskPaint
        {
            get
            {
                return _MaskPaint;
            }
            private set
            {
                _MaskPaint = value;
            }
        }
        static public   SKPaint GridPaint
        {
            get
            {
                return _GridPaint;
            }
        }

        static public float Scale
        {
            get; private set;
        } = 1.0f;
        static public float MinorTextSize                                           => Globals.MinorFontSize * Scale;
        static public float MajorTextSize                                           => Globals.MajorFontSize * Scale;
        static public (float x, float y) MeasureText(string Input, SKPaint Paint)   =>  (Paint.MeasureText(Input), Globals.TitleFontSize);
        static public (float x, float y) MeasureMajorText(string Input)             =>  (MajorPaint.MeasureText(Input), MajorTextSize);
        static public (float x, float y) MeasureMinorText(string Input)             =>  (MinorPaint.MeasureText(Input), MinorTextSize);
        public static SmartPadding Padding { get; private set; } = new SmartPadding(0.05f, 0, 0.1f, 0);
        public ASmartElement(){}
    }

    public class SmartChart : GeneralView
    {
        private SmartData   Data;
        private SmartTitle  _Title = new SmartTitle() { Title = "Untitled" };

        #region EVENTS
        public event EventHandler Clicked;
        #endregion
        
        #region TOUCHSCREEN
        private Touch mTouch;
        private void MTouch_Press   (object sender, TouchActionEventArgs args)  {}
        private void MTouch_Hover   (object sender, TouchActionEventArgs args)  {}
        private void MTouch_Release (object sender, TouchActionEventArgs args)  {}
        private void SetupTouch()
        {
            //Add the gesture recognizer 
            mTouch = new Touch();
            mTouch.Tap      += MTouch_Tap;
            mTouch.Pressed  += MTouch_Press;
            mTouch.Hover    += MTouch_Hover;
            mTouch.Released += MTouch_Release;
            mTouch.Pinch    += MTouch_Pinch;
            mTouch.Pan      += MTouch_Pan;
            Effects.Add(mTouch);
        }
        private void MTouch_Tap(object sender, Touch.TouchTapEventArgs args)
        {
            Clicked?.Invoke(sender, EventArgs.Empty);
        }
        private void MTouch_Pan(object sender, TouchPanActionEventArgs args)
        {
            Data.Axis.Pan((float)args.Dx, (float)args.Dy);
        }
        private void MTouch_Pinch(object sender, TouchPinchActionEventArgs args)
        {
            var zoomX = (float)args.Pinch.ZoomX;
            var zoomY = (float)args.Pinch.ZoomY;
            var Center = args.Pinch.Center;

            Debug.WriteLine(zoomX.ToString() + " " + zoomY.ToString());
            Data.Axis.Zoom(zoomX, zoomY, (float)Center.X, (float)Center.Y);
        }
        #endregion
        
        #region RENDERER
        GeneralRenderer mRenderer;
        public void Disable()
        {
            mRenderer = null;
            Content = null;
        }
        public void Enable()
        {
            mRenderer = new GeneralRenderer(Draw);
            Content = mRenderer;
        }
        public new bool IsVisible
        {
            set
            {
                if (value) Enable();
                else Disable();
                base.IsVisible = value;
            }
        }
        public void InvalidateSurface()
        {
            mRenderer?.InvalidateSurface();
        }
        #endregion

        //Told you it'd be easy
        public void SaveCSV()
        {
            Files.SaveFile(Data.GetCSV());
        }

        public string Title
        {
            get
            {
                return _Title.Title;
            }
            set
            {
                _Title.Title = value;
            }
        }

        private void Draw(SKCanvas canvas, SKSize dimension)
        {
            canvas.Clear(BackgroundColor.ToSKColor());
            (var x1, var y1, var x2, var y2) = ASmartElement.Padding.GetHorizontalLine(dimension.Width, 10);
            _Title.Draw(canvas, dimension);
            Data.Draw(canvas, dimension);
        }
        public SmartChart(SmartData pData)
        {
            Data = pData;
            Data.Parent = this;
            SetupTouch();
            IsVisible = true;
        }
    }
}
