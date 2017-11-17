using System;
using System.Collections.Generic;
using System.Text;
using SkiaSharp;

namespace rMultiplatform
{
    public class ChartPadding : AChartRenderer
    {
        //System borders
        float mLeft;
        float mRight;
        float mTop;
        float mBottom;

        //Returns the (1-Val)
        private float OtherSide(float Val)
        {
            return (float)1 - Val;
        }
        private bool WithinPadding(SKPoint pInput)
        {
            var x = pInput.X;
            var y = pInput.Y;

            if (GetLeftPosition <= x && x <= GetRightPosition)
                if (GetTopPosition <= y && y <= GetBottomPosition)
                    return true;

            return false;
        }

        //These get the pixel coordinates of the padding
        public float GetLeftPosition
        {
            get
            {
                return mLeft * (float)ParentWidth;
            }
        }
        public float GetRightPosition
        {
            get
            {
                return OtherSide(mRight) * (float)ParentWidth;
            }
        }
        public float GetTopPosition
        {
            get
            {
                return mTop * (float)ParentHeight;
            }
        }
        public float GetBottomPosition
        {
            get
            {
                return OtherSide(mBottom) * (float)ParentHeight;
            }
        }
        public float PaddedWidth
        {
            get
            {
                return (float)ParentWidth * (1 - mLeft - mRight);
            }
        }
        public float PaddedHeight
        {
            get
            {
                return (float)ParentHeight * (1 - mTop - mBottom);
            }
        }

        //Returns the positions of all of the lines
        public float L
        {
            get
            {
                return mLeft * (float)ParentWidth;
            }
            set
            {
                var ratio = value / (float)ParentWidth;
                mLeft = ratio;
                InvalidateParent();
            }
        }
        public float R
        {
            get
            {
                return mRight * (float)ParentWidth;
            }
            set
            {
                var ratio = value / ParentWidth;
                mRight = (float)ratio;
                InvalidateParent();
            }
        }
        public float T
        {
            get
            {
                return mTop * (float)ParentHeight;
            }
            set
            {
                var ratio = value / ParentHeight;
                mTop = (float)ratio;
                InvalidateParent();
            }
        }
        public float B
        {
            get
            {
                return mBottom * (float)ParentHeight;
            }
            set
            {
                var ratio = value / ParentHeight;
                mBottom = (float)ratio;
                InvalidateParent();
            }
        }
        public float W
        {
            get
            {
                return PaddedWidth;
            }
        }
        public float H
        {
            get
            {
                return PaddedHeight;
            }
        }

        //0'th priority
        public override int Layer { get { return 0; } }
        private bool _DrawBoundary;
        public bool DrawBoundary
        {
            get
            {
                return _DrawBoundary;
            }
            set
            {
                _DrawBoundary = value;
                InvalidateParent();
            }
        }

        //This returns the paddnig rectangle
        public SKRect Rectangle
        {
            get
            {
                return new SkiaSharp.SKRect(GetLeftPosition, GetTopPosition, GetRightPosition, GetBottomPosition);
            }
        }

        //Creates horozontal and vertical lines (pairs of points)
        public (SKPoint P1, SKPoint P2) GetHorozontalLine(float Position)
        {
            var x1 = GetLeftPosition;
            var x2 = GetRightPosition;
            var p1 = new SKPoint(x1, Position);
            var p2 = new SKPoint(x2, Position);

            //Make sure point is in the boundaries of the object.
            if (WithinPadding(p1))
                if (WithinPadding(p2))
                    return (p1, p2);

            return (p1, p2);
            //throw (new Exception("Point is not within padding boundaries."));
        }
        public (SKPoint P1, SKPoint P2) GetVerticalLine(float Position)
        {
            var y1 = GetTopPosition;
            var y2 = GetBottomPosition;
            var p1 = new SKPoint(Position, y1);
            var p2 = new SKPoint(Position, y2);

            //Make sure point is in the boundaries of the object.
            if (WithinPadding(p1))
                if (WithinPadding(p2))
                    return (p1, p2);

            return (p1, p2);
            //throw (new Exception("Point is not within padding boundaries."));
        }

        public override bool Draw(SKCanvas c)
        {
            if (DrawBoundary)
                c.DrawRect(Rectangle, new SKPaint() {StrokeWidth = 2, IsStroke = true, Color = SKColors.White});
            return false;
        }

        //Padding constructors
        public ChartPadding(float V)
        {
            if (V < 0)
                throw (new Exception("Padding cannot be negative"));

            mLeft   = V;
            mRight  = V;
            mTop    = V;
            mBottom = V;
        }
        public ChartPadding(float L, float R, float T, float B)
        {
            if (L < 0 || R < 0 || T < 0 || B < 0)
                throw (new Exception("Padding cannot be negative"));

            mLeft   = L;
            mRight  = R;
            mTop    = T;
            mBottom = B;
        }

        //To simplify code
        public ChartPadding(double V)
        {
            if (V < 0)
                throw (new Exception("Padding cannot be negative"));

            mLeft   = (float)V;
            mRight  = (float)V;
            mTop    = (float)V;
            mBottom = (float)V;
        }
        public ChartPadding(double L, double R, double T, double B)
        {
            if (L < 0 || R < 0 || T < 0 || B < 0)
                throw (new Exception("Padding cannot be negative"));

            mLeft   = (float)L;
            mRight  = (float)R;
            mTop    = (float)T;
            mBottom = (float)B;
        }
    }
}
