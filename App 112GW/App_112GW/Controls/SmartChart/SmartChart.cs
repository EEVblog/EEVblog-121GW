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

        static private SKPaint _MajorPaint  = MakeDefaultPaint(Globals.TextColor,       1,  Globals.MajorFontSize,  Globals.Typeface);
        static private SKPaint _MinorPaint  = MakeDefaultPaint(Globals.TextColor,       1,  Globals.MinorFontSize,  Globals.Typeface);
        static private SKPaint _GridPaint   = MakeDefaultPaint(Globals.TextColor,       1,  Globals.MinorFontSize,  Globals.Typeface, Dotted: true);
        static private SKPaint _MaskPaint   = MakeDefaultPaint(Globals.BackgroundColor, 1,  Globals.MinorFontSize,  Globals.Typeface);

        static public SKPaint ScaledPaint   (float scale, SKPaint paint)
        {
            var cpy = paint.Clone();
            cpy.TextSize *= scale;
            return cpy;
        }
        static public SKPaint MajorPaint    (float scale) => ScaledPaint(scale, _MajorPaint);
        static public SKPaint MinorPaint    (float scale) => ScaledPaint(scale, _MajorPaint);

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
        static public SKPaint GridPaint(float scale)
        {
            var temp = ScaledPaint(scale, _GridPaint);
            temp.ColorFilter = SKColorFilter.CreateBlendMode(temp.Color, SKBlendMode.Dst);
            temp.PathEffect = SKPathEffect.CreateDash(new[] { 2 * scale, 2 * scale }, 0);
            return temp;
        }

        static public float MinorTextSize => Globals.MinorFontSize;
        static public float MajorTextSize => Globals.MajorFontSize;

        static public (float x, float y) MeasureText(string Input, SKPaint Paint)
        {
            SKRect temp = new SKRect(0, 0, 0, 0);
            Paint.MeasureText(Input, ref temp);
            //Compensates for cropping of pixels
            return (temp.Width + 2, temp.Height + 2);
        }
        static public float SpaceWidth(SKPaint Paint) => (Paint.MeasureText(" "));

        public static SmartPadding Padding { get; private set; } = new SmartPadding(0.05f, 0, 0.1f, 0);
		public ASmartElement(){}
	}

	public class SmartChart : GeneralView
	{
		private SmartData   Data;
		private SmartTitle  _Title = new SmartTitle() { Title = "" };

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
			mTouch.Tap	    += MTouch_Tap;
			mTouch.Pressed  += MTouch_Press;
			mTouch.Hover	+= MTouch_Hover;
			mTouch.Released += MTouch_Release;
			mTouch.Pinch	+= MTouch_Pinch;
			mTouch.Pan	    += MTouch_Pan;
            mTouch.Scroll   += MTouch_Scroll;
			Effects.Add(mTouch);
		}


        private void MTouch_Scroll(object sender, ScrollActionEventArgs args)
        {
            var dist = 1f + (float)args.Steps / (720f);

            var zoomX = dist;
            var zoomY = 0;
            var Center = args.About;

            Data.Axis.Zoom(zoomX, zoomY, (float)Center.X, (float)Center.Y);
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

        private void Draw(SKCanvas canvas, SKSize dimension, SKSize viewsize)
        {
            canvas.Clear(BackgroundColor.ToSKColor());
            (var x1, var y1, var x2, var y2) = 
                ASmartElement.Padding.GetHorizontalLine(dimension.Width, 10);

            _Title.Draw(canvas, dimension, viewsize);
            Data.Draw(canvas, dimension, viewsize);
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
