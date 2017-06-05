using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace rMultiplatform
{

    class BackButton : GeneralControl
    {
        public event EventHandler   Back
        {
            add
            {
                Clicked += value;
            }
            remove
            {
                Clicked -= value;
            }
        }

        public BackButton () :base("Back", new SKPoint[] {
            new SKPoint((float)(0.5), (float)(0)),
            new SKPoint((float)(0), (float)(0.5)),
            new SKPoint((float)(1), (float)(0.5)),
            new SKPoint((float)(0), (float)(0.5)),
            new SKPoint((float)(0.5), (float)(1))})
        {
            mRenderer.ShowPoints();
        }
        
        //Keep control square
        protected override void     OnSizeAllocated(double width, double height)
        {
            width = height;
            base.OnSizeAllocated(width, height);
        }
    }
}
