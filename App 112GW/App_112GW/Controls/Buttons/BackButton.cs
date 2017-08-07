using System;
using SkiaSharp;

namespace rMultiplatform
{
    class BackButton : GeneralControl
    {
        public event EventHandler Back
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
        protected override void OnSizeAllocated(double width, double height)
        {
            width = height;
            base.OnSizeAllocated(width, height);
        }
        public BackButton () :base(new SKPoint[] {
            new SKPoint((float)(0.5), (float)(0)),
            new SKPoint((float)(0), (float)(0.5)),
            new SKPoint((float)(1), (float)(0.5)),
            new SKPoint((float)(0), (float)(0.5)),
            new SKPoint((float)(0.5), (float)(1))})
        {
            ShowPoints();
        }
    }
}