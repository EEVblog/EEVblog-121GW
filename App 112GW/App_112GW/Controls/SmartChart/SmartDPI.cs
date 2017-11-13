using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace rMultiplatform
{
    static class SmartDPI
    {
        static public (float, float) GetScale(SKCanvas canvas, SKSize dimension, SKSize view)
        {
            return (dimension.Width / view.Width, dimension.Height / view.Height);
        }
    };
}