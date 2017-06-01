using System;
using System.Collections.Generic;
using System.Text;
using SkiaSharp;

namespace rMultiplatform
{
    public class Padding : rMultiplatform.IChartRenderer
    {
        //Inherited parent properties
        float ParentWidth;
        float ParentHeight;

        //System boarders
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

            if (L <= x && x <= R)
                if (T <= y && y <= B)
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

        //Returns the positions of all of the lines
        public float L
        {
            get
            {
                return GetLeftPosition;
            }
        }
        public float R
        {
            get
            {
                return GetRightPosition;
            }
        }
        public float T
        {
            get
            {
                return GetTopPosition;
            }
        }
        public float B
        {
            get
            {
                return GetBottomPosition;
            }
        }

        //0'th priority
        public int Layer { get { return 0; } }

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

        //General chart renderer functions
        public bool Register(object o)
        {
            return true;
        }
        public List<Type> RequireRegistration()
        {
            return null;
        }
        public bool Draw(SKCanvas c)
        {
            c.DrawRect(Rectangle, new SKPaint() {Color = SKColors.White});
            return false;
        }
        public void SetParentSize(double w, double h)
        {
            ParentWidth = (float)w;
            ParentHeight = (float)h;
        }
        public bool RegisterParent(object c)
        {
            return false;
        }
        public void InvalidateParent()
        {}
        public int CompareTo(object obj)
        { return 0; }

        //Initialise the class
        public Padding(float V)
        {
            if (V < 0)
                throw (new Exception("Padding cannot be negative"));

            mLeft = V;
            mRight = V;
            mTop = V;
            mBottom = V;
        }
        public Padding(float L, float R, float T, float B)
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
