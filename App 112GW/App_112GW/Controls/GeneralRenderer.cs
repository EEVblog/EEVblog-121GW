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
    public class GeneralRenderer :
#if __ANDROID__ && ! SOFTWARE_DRAW || true
        SKGLView
#elif __IOS__ && ! SOFTWARE_DRAW
        SKGLView
#else
        SKCanvasView
#endif
    {
        public delegate void PaintCanvas(SKCanvas c, SKSize s);
        public event PaintCanvas Paint;
        public GeneralRenderer(PaintCanvas PaintEvent)
        {
            Paint               +=  PaintEvent;
            HorizontalOptions   =   LayoutOptions.Fill;
            VerticalOptions     =   LayoutOptions.Fill;
            BackgroundColor     =   Globals.BackgroundColor;
        }

#if __ANDROID__ && !SOFTWARE_DRAW || true
        protected override void OnPaintSurface(SKPaintGLSurfaceEventArgs e)
#elif __IOS__ && ! SOFTWARE_DRAW
        protected override void OnPaintSurface(SKPaintGLSurfaceEventArgs e)
#else
        protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
#endif
        {
            Paint?.Invoke(e.Surface.Canvas, CanvasSize);
        }
    }
}
