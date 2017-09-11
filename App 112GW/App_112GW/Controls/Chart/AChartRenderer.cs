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
    public abstract class AChartRenderer : IComparable
    {
        public abstract int Layer { get; }

        public ChartPadding ParentPadding { get; private set; }

        public delegate bool RegisterFilter(object o);
        private RegisterFilter _RegFilter = null;
        public RegisterFilter RegistrationFilter
        {
            get
            {
                return _RegFilter;
            }
            set
            {
                _RegFilter = value;
            }
        }

        public void Register(Object o)
        {
            if (o.GetType() == GetType())
            {
                if (o.GetType() == typeof(ChartPadding))
                {
                    ParentPadding = o as ChartPadding;
                    return;
                }

                if (RegistrationFilter == null)
                    RequireRegistration?.Add(o.GetType());
                else if (RegistrationFilter(o))
                    RequireRegistration?.Add(o.GetType());
            }
        }
        public Chart Parent { get; private set; }

        public void RegisterParent(Object c)
        {
            Parent = c as Chart;
        }
        private List<Type> _RequiredRegistration;
        public List<Type> RequireRegistration
        {
            get
            {
                return _RequiredRegistration;
            }
            private set
            {
                _RequiredRegistration = value;
            }
        }
        public void InvalidateParent()
        {
            Parent?.InvalidateSurface();
        }

        //Return true when redraw is required
        public abstract bool Draw(SKCanvas c);

        public double ParentWidth  { get; private set; }
        public double ParentHeight { get; private set; }

        public virtual void ParentSizeAdjusted() { }
        public void SetParentSize ( double w, double h, double scale = 1.0 )
        {
            ParentWidth = w;
            ParentHeight = h;

            //Must be set first!
            Scale = (float)scale;
            MinorPaint.TextSize = MinorTextSize;
            MajorPaint.TextSize = MajorTextSize;
            ParentSizeAdjusted();
        }

        public SKSize MeasureText(string Input)
        {
            return new SKSize(MajorPaint.MeasureText(Input), MajorTextSize);
        }
 
        public int CompareTo(object obj)
        {
            if (obj is AChartRenderer)
            {
                var ob = obj as AChartRenderer;
                var layer = ob.Layer;
                if (layer > Layer)
                    return -1;
                else if (layer < Layer)
                    return 1;
            }
            return 0;
        }

        public float MinorTextSize
        {
            get
            {
                return Globals.MinorFontSize * Scale;
            }
        }
        public float MajorTextSize
        {
            get
            {
                return Globals.MajorFontSize * Scale;
            }
        }

        static private SKPaint MakeDefaultPaint(Color pColor, float pStrokeWidth, float pFontSize, SKTypeface pTypeface)
        {
            return new SKPaint()
            {
                Color           = pColor.ToSKColor(),
                StrokeWidth     = pStrokeWidth,
                Typeface        = pTypeface,
                TextSize        = pFontSize,
                StrokeCap       = SKStrokeCap.Round,
                BlendMode       = SKBlendMode.Src,
                IsAntialias     = true
            };
        }
        static private SKPaint _MajorPaint = MakeDefaultPaint(Globals.TextColor, 2, Globals.MajorFontSize, Globals.Typeface);
        static private SKPaint _MinorPaint = MakeDefaultPaint(Globals.TextColor, 1, Globals.MinorFontSize, Globals.Typeface);

        private float _Scale;
        public float Scale
        {
            set
            {
                _Scale = value;
            }
            get
            {
                return _Scale;
            }
        }
        public SKPaint MajorPaint
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
        public SKPaint MinorPaint
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

        public AChartRenderer(List<Type> pRegisteredTypes = null)
        {
            RequireRegistration = pRegisteredTypes;
            RegistrationFilter = null;
        }
    };
}
