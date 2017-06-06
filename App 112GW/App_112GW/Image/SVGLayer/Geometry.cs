using System;
using System.Collections.Generic;
using System.Text;

using SkiaSharp;
using SkiaSharp.Views.Forms;

namespace App_112GW
{
    using Vector = SKPoint;
    using Path = SKPath;

    interface               ICurve
    {
        //time value is a value between 0 and 1
        Vector  GetPoint(float pTime);

        //Properties
        Vector  Start
        {
            get;
        }
        Vector  End
        {
            get;
        }
        int     Count
        {
            get;
        }
    }
    public abstract class   Curve
    {
        protected Vector[] Points;

        //time value is a value between 0 and 1
        public abstract Vector GetPoint(float pTime);
        public virtual Vector Start
        {
            get
            {
                return Points[0];
            }
        }
        public virtual Vector End
        {
            get
            {
                return Points[Count - 1];
            }
        }
        public virtual int Count
        {
            get
            {
                return Points.Length;
            }
        }
        public static float DistanceBetween(Vector P1, Vector P2)
        {
            var dx = P2.X - P1.X;
            var dy = P2.Y - P1.Y;
            return (float)Math.Sqrt(dx * dx + dy * dy);
        }

        /// <summary>
        /// Calculates the binomial coefficient (nCk) (N items, choose k)
        /// </summary>
        /// <param name="n">the number items</param>
        /// <param name="k">the number to choose</param>
        /// <returns>the binomial coefficient</returns>
        public static long Binomial(long n, long k)
        {
            if (k > n) return 0;
            if (n == k) return 1;
            if (k > n - k) k = n - k; // Everything is symmetric around n-k, so it is quicker to iterate over a smaller k than a larger one.

            long c = 1;
            for (long i = 1; i <= k; i++)
            {
                c *= n--;
                c /= i;
            }
            return c;
        }
        public static Vector WeightedPoint(Vector P1, Vector P2, float Weight)
        {
            var x = P2.X - P1.X;
            var y = P2.Y - P1.Y;
            x *= Weight;
            y *= Weight;

            x += P1.X;
            y += P1.Y;

            return new Vector(x, y);
        }
    }
    public class            Bezier : Curve
    {
        public Bezier(Vector[] pPoints)
        {
            Points = pPoints;
        }
        public Bezier(List<Vector> pPoints)
        {
            Points = pPoints.ToArray();
        }
        public override Vector GetPoint(float pTime)
        {
            var t = pTime;
            var v = Count;
            var n = v - 1;
            var xresult = (float)0;
            var yresult = (float)0;

            //Calculate X components
            for (int k = 0; k < v; k++)
            {
                var binom = Binomial(n, k);
                var tpow = (float)Math.Pow(t, k);
                var t_1pow = (float)Math.Pow(1.0f - t, n - k);
                xresult += Points[k].X * binom * t_1pow * tpow;
                yresult += Points[k].Y * binom * t_1pow * tpow;
            }
            return new Vector(xresult, yresult);
        }
    }
    public class            Cubic : Bezier
    {
        public Cubic(Vector P1, Vector P2, Vector P3, Vector P4) : base(new List<Vector> { P1, P2, P3, P4 })
        { }
    }
    public class            Quadratic : Bezier
    {
        public Quadratic(Vector P1, Vector P2, Vector P3) : base(new List<Vector> { P1, P2, P3 })
        { }
    }
    public class            Line : Curve
    {
        public Line(Vector P1, Vector P2)
        {
            Points = new Vector[] { P1, P2 };
        }
        public override Vector GetPoint(float pTime)
        {
            return WeightedPoint(Start, End, pTime);
        }
    }
    class                   Polycurve : ICurve
    {
        private Vector      mStart;
        private List<Curve> mCurves;

        //Interface functions
        public Vector   Start
        {
            get
            {
                var i = 0;
                return mCurves[i].Start;
            }
        }
        public Vector   End
        {
            get
            {
                var i = Count - 1;
                return mCurves[i].End;
            }
        }
        public int      Count
        {
            get
            {
                if (mCurves == null)
                    return 0;
                return mCurves.Count;
            }
        }
        public Vector   GetPoint(float pTime)
        {
            pTime *= Count;
            var i = (int)pTime;
            pTime -= (float)i;
            return mCurves[i].GetPoint(pTime);
        }

        //Polycurve functions
        public void AddLine(Vector pPoint)
        {
            Vector start;
            if (Count > 0)
                start = End;
            else
                start = mStart;
            
            mCurves.Add(new Line(start, pPoint));
        }
        public void AddQuadratic(Vector pControl, Vector pEnd)
        {
            Vector start;
            if (Count > 0)
                start = End;
            else
                start = mStart;

            mCurves.Add(new Quadratic(start, pControl, pEnd));
        }
        public void AddCubic(Vector pControl1, Vector pControl2, Vector pEnd)
        {
            Vector start;
            if (Count > 0)
                start = End;
            else
                start = mStart;

            mCurves.Add(new Cubic(start, pControl1, pControl1, pEnd));
        }
        public void AddBezier(Vector[] pPoints)
        {
            Vector start;
            if (Count > 0)
                start = End;
            else
                start = mStart;
            var pts = new List<Vector>(pPoints);
            pts.Insert(0, start);
            mCurves.Add(new Bezier(pts));
        }
        public void CloseCurve()
        {
            if (Start.Equals(End))
                return;
            AddLine(Start);
        }

        //Constructor
        public Polycurve(float x, float y)
        {
            mCurves = new List<Curve>();
            mStart = new Vector(x, y);
        }
        public Polycurve(Vector pStart)
        {
            mCurves = new List<Curve>();
            mStart = new Vector();
            mStart = pStart;
        }

        //Default resolution makes 10 points per line segment
        public SKPath ToPath(float Resolution = 0.1f)
        {
            var Pts = new List<SKPoint>();
            var Limit = 1.0f;

            Resolution /= (float)Count;

            for (float time = 0; time <= Limit; time += Resolution)
                Pts.Add(GetPoint(time));

            var Path = new SKPath();
            Path.AddPoly(Pts.ToArray(), false);
            return Path;
        }

        //TEST CODE
        //var Curv = new Polycurve(0, 0);
        //Curv.AddLine(new SKPoint(100, 100));
        //Curv.AddBezier(new SKPoint[] { new SKPoint(0, 0), new SKPoint(0, 100), new SKPoint(100, 100) });
        //Curv.AddLine(new SKPoint(0, 100));

        //Curv.CloseCurve();

        //
        //for (float time = 0; time <= 1; time += 0.01f)
        //{
        //Pts.Add(Curv.GetPoint(time));
        //}
        //var Path = new SKPath();
        //Path.AddPoly(Pts.ToArray(),false);
        //mDrawPaint.IsStroke = false;
        //pSurface.DrawPath(Path, mDrawPaint);
    }
}
