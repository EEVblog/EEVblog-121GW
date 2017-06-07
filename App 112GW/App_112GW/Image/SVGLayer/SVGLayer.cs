﻿using System;
using System.Collections.Generic;
using System.Text;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;



namespace App_112GW
{
    public class SVGLayer : ILayer
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


        public SKSvg mImage;
        public string mName;

        SKPaint mDrawPaint;
        SKPaint mUndrawPaint;

        public SVGLayer(SKSvg pImage, string pName, bool pActive = true)
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
            mDrawPaint.Color = SKColors.Red;
            mDrawPaint.IsAntialias = true;
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

        public string   Name
        {
            get
            {
                return mName;
            }
            set
            {
                mName = value;
            }
        }
        public int      Width
        {
            get
            {
                return (int)mImage.CanvasSize.Width;
            }
        }
        public int      Height
        {
            get
            {
                return (int)mImage.CanvasSize.Height;
            }
        }
        public void     Render(ref SKCanvas pSurface)
        {
            //This is render changed variable, don't move it to set, that is wrong
            if (_RenderChanged.Update(ref mActive))
            {
                if (mActive)
                    pSurface.DrawPicture(mImage.Picture, mDrawPaint);
                else
                    pSurface.DrawPicture(mImage.Picture, mUndrawPaint);
            }









            pSurface.Clear(SKColors.Black);
            var a = new SVGPathBuilder("asasd");
            foreach (var curv in a.Curves)
                pSurface.DrawPath(curv.ToPath(0.1f), mDrawPaint);




            SKMatrix aoewgin = SVGPath.BuildTransformMatrix("matrix(0.26458333,0,0,0.26458333,24.575937,125.35773)");




        }
    }
}
