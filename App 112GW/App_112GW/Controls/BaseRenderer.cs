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
    public abstract class BaseRenderer : ContentView
    {
        GeneralRenderer Renderer;
        public void     Disable()
        {
            Renderer = null;
            Content = null;
        }
        public void     Enable()
        {
            Renderer = new GeneralRenderer(PaintSurface);
            Content = Renderer;
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
        public void     InvalidateSurface()
        {
            if (Renderer != null)
                Renderer.InvalidateSurface();
        }

        public abstract void PaintSurface(SKCanvas canvas, SKSize dimension);
        public BaseRenderer()
        {
            Renderer.Paint += PaintSurface; ;
        }
    }
}
