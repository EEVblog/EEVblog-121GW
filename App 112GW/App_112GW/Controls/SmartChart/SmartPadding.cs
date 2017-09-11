using System;
using SkiaSharp;

namespace rMultiplatform
{
    public class SmartPadding
    {
        //System borders
        float mLeft;
        float mRight;
        float mTop;
        float mBottom;

        //Returns the (1-Val)
        private float OtherSide(float Val)
        {
            return 1 - Val;
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
                return mLeft * ParentWidth;
            }
        }
        public float GetRightPosition
        {
            get
            {
                return OtherSide(mRight) * ParentWidth;
            }
        }
        public float GetTopPosition
        {
            get
            {
                return mTop * ParentHeight;
            }
        }
        public float GetBottomPosition
        {
            get
            {
                return OtherSide(mBottom) * ParentHeight;
            }
        }
        public float PaddedWidth
        {
            get
            {
                return ParentWidth * (1 - mLeft - mRight);
            }
        }
        public float PaddedHeight
        {
            get
            {
                return ParentHeight * (1 - mTop - mBottom);
            }
        }

        //Returns the positions of all of the lines
        public float L
        {
            get
            {
                return mLeft * ParentWidth;
            }
            set
            {
                var ratio = value / ParentWidth;
                mLeft = ratio;
            }
        }
        public float R
        {
            get
            {
                return mRight * ParentWidth;
            }
            set
            {
                var ratio = value / ParentWidth;
                mRight = ratio;
            }
        }
        public float T
        {
            get
            {
                return mTop * ParentHeight;
            }
            set
            {
                var ratio = value / ParentHeight;
                mTop = ratio;
            }
        }
        public float B
        {
            get
            {
                return mBottom * ParentHeight;
            }
            set
            {
                var ratio = value / ParentHeight;
                mBottom = ratio;
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

        //This returns the paddnig rectangle
        public SKRect Rectangle
        {
            get
            {
                return new SKRect(GetLeftPosition, GetTopPosition, GetRightPosition, GetBottomPosition);
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
        }

        //To simplify code
        public SmartPadding(float V)
        {
            if (V < 0)
                throw (new Exception("Padding cannot be negative"));

            mLeft = V;
            mRight = V;
            mTop = V;
            mBottom = V;
        }
        public SmartPadding(float L, float R, float T, float B)
        {
            if (L < 0 || R < 0 || T < 0 || B < 0)
                throw (new Exception("Padding cannot be negative"));

            mLeft = L;
            mRight = R;
            mTop = T;
            mBottom = B;
        }
    }
}
