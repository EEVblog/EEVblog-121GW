using Xamarin.Forms;
using SkiaSharp;
using SkiaSharp.Views.Forms;

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
        static public float MinorTextSize => Globals.MinorFontSize * Scale;
        static public float MajorTextSize => Globals.MajorFontSize * Scale;

        static public (float x, float y) MeasureMajorText(string Input)
        {
            return (MajorPaint.MeasureText(Input), MajorTextSize);
        }
        static public (float x, float y) MeasureMinorText(string Input)
        {
            return (MinorPaint.MeasureText(Input), MinorTextSize);
        }

        public static SmartPadding Padding { get; private set; } = new SmartPadding(0.25f);

        public ASmartElement(){}
    }

    public class SmartChart : GeneralView
    {
        #region GENERALVIEW_RENDER_REGION
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

        private SmartData Data;
        private void Draw(SKCanvas canvas, SKSize dimension)
        {
            Data.Draw(canvas, dimension);
        }
        public SmartChart(SmartData pData)
        {
            Data = pData;
            Data.Parent = this;

            IsVisible = true;
        }
    }
}
