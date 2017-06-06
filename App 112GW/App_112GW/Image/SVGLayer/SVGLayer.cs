using System;
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
            mDrawPaint.BlendMode = SKBlendMode.SrcOver;
            mDrawPaint.ColorFilter = SKColorFilter.CreateBlendMode(transparency, SKBlendMode.DstOver);

            mUndrawPaint = new SKPaint();
            mUndrawPaint.BlendMode = SKBlendMode.DstOut;
            mUndrawPaint.ColorFilter = SKColorFilter.CreateBlendMode(transparency, SKBlendMode.DstOver);
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


            //var Curv = new Polycurve(0, 0);
            //Curv.AddLine(new SKPoint(100, 100));
            //Curv.AddBezier(new SKPoint[] { new SKPoint(0, 0), new SKPoint(0, 100), new SKPoint(100, 100) });
            //Curv.AddLine(new SKPoint(0, 100));
            //Curv.CloseCurve();

            //pSurface.DrawPath(Curv.ToPath(), mDrawPaint);

            //var a = new SVGToPath("asasd");
        }
    }
}
