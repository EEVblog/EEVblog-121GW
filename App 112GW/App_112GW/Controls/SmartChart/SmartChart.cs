using Xamarin.Forms;
using SkiaSharp;
using SkiaSharp.Views.Forms;

namespace rMultiplatform
{
    abstract class ASmartElement
    {
        static private SKPaint MakeDefaultPaint(Color pColor, float pStrokeWidth, float pFontSize, SKTypeface pTypeface)
        {
            return new SKPaint()
            {
                Color = pColor.ToSKColor(),
                StrokeWidth = pStrokeWidth,
                Typeface = pTypeface,
                TextSize = pFontSize,
                StrokeCap = SKStrokeCap.Round,
                BlendMode = SKBlendMode.Src,
                IsAntialias = true
            };
        }

        static private SKPaint _MajorPaint  = MakeDefaultPaint(Globals.TextColor,       2,  Globals.MajorFontSize,  Globals.Typeface);
        static private SKPaint _MinorPaint  = MakeDefaultPaint(Globals.TextColor,       1,  Globals.MinorFontSize,  Globals.Typeface);
        static private SKPaint _MaskPaint   = MakeDefaultPaint(Globals.BackgroundColor, 1,  Globals.MinorFontSize,  Globals.Typeface);
        static public SKPaint MajorPaint
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
        static public SKPaint MinorPaint
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
        static public SKPaint MaskPaint
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

        static public float Scale
        {
            get;private set;
        }
        static public float MinorTextSize
        {
            get
            {
                return Globals.MinorFontSize * Scale;
            }
        }
        static public float MajorTextSize
        {
            get
            {
                return Globals.MajorFontSize * Scale;
            }
        }

        static public (float x, float y) MeasureText(string Input)
        {
            return (MajorPaint.MeasureText(Input), MajorTextSize);
        }

        public double ParentWidth { get; private set; }
        public double ParentHeight { get; private set; }
    }

    public class SmartChart : GeneralView
    {
        SmartData      Data;

        #region RENDER_REGION
        GeneralRenderer mRenderer;
        public void     Disable()
        {
            mRenderer = null;
            Content = null;
        }
        public void     Enable()
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
        private void    PaintSurface(SKCanvas canvas, SKSize dimension)
        {
            (var horz, var vert) = Data.GetRange();
        }
        #endregion
    }
}
