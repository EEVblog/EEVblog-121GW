using System;
using System.Collections.Generic;
using System.Text;
using SkiaSharp;
using Xamarin.Forms;
using SkiaSharp.Views.Forms;

namespace App_112GW
{
    public class PathLayer : ILayer
    {
        private bool mActive;
        private VariableMonitor<bool> _Changed;
        public event EventHandler OnChanged
        {
            add
            {
                _Changed.OnChanged += value;
            }
            remove
            {
                _Changed.OnChanged -= value;
            }
        }


        public Polycurve mImage;
        public string mName;

        SKPaint mDrawPaint;
        SKPaint mUndrawPaint;

        public PathLayer(Polycurve pImage, string pName, bool pActive = true)
        {
            _Changed = new VariableMonitor<bool>();
            _RenderChanged = new VariableMonitor<bool>();

            //Open the defined image
            mActive = pActive;
            mImage = pImage;
            mName = pName;

            //
            var transparency = Color.FromRgba(0, 0, 0, 0).ToSKColor();

            mDrawPaint = new SKPaint();
            mDrawPaint.BlendMode = SKBlendMode.SrcOver;
            mDrawPaint.ColorFilter = SKColorFilter.CreateBlendMode(transparency, SKBlendMode.DstOver);

            mUndrawPaint = new SKPaint();
            mUndrawPaint.BlendMode = SKBlendMode.DstOut;
            mUndrawPaint.ColorFilter = SKColorFilter.CreateBlendMode(transparency, SKBlendMode.DstOver);

            Off();
        }
        public void Set(bool pState)
        {
            bool temp = mActive;
            mActive = pState;
            _Changed.Update(ref mActive);
        }
        public void On()
        {
            Set(true);
        }
        public void Off()
        {
            Set(false);
        }
        public override string ToString()
        {
            return mName;
        }

        private VariableMonitor<bool> _RenderChanged;

        public string Name
        {
            get
            { return mName; }
            set
            { mName = value; }
        }
        public int Width
        {
            get
            {
                return (int)mImage.Width;
            }
        }
        public int Height
        {
            get
            {
                return (int)mImage.Height;
            }
        }

        public void Render(ref SKCanvas pSurface)
        {
            //This is render changed variable, don't move it to set, that is wrong
            if (_RenderChanged.Update(ref mActive))
            {
                var Pth = new SKPath();
                if (mActive)
                {
                    while (mImage.GetPath(0.1f, out Pth))
                        pSurface.DrawPath(Pth, mDrawPaint);
                }
                else
                {
                    while (mImage.GetPath(0.1f, out Pth))
                        pSurface.DrawPath(Pth, mUndrawPaint);
                }
            }
        }
    }
}



