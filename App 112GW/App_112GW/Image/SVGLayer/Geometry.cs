using System;
using System.Collections.Generic;
using System.Text;

using SkiaSharp;
using SkiaSharp.Views.Forms;
using System.Diagnostics;

namespace rMultiplatform
{
	using Vector = SKPoint;
	using Path = SKPath;

	interface			   ICurve
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
		int	 Count
		{
			get;
		}
	}
	public abstract class   Curve
	{
		protected Vector[] Points;

		public enum eType
		{
			eLine,
			eBezier,
			eStart
		}
		public abstract eType   Type { get; }
		//time value is a value between 0 and 1
		public abstract Vector  GetPoint(float pTime);
		public virtual Vector   Start
		{
			get
			{
				return Points[0];
			}
		}
		public virtual Vector   End
		{
			get
			{
				return Points[Count - 1];
			}
		}
		public virtual int	  Count
		{
			get
			{
				return Points.Length;
			}
		}
		public static float	 DistanceBetween(Vector P1, Vector P2)
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
	public class			Bezier : Curve
	{
		public override eType Type
		{
			get
			{
				return eType.eBezier;
			}
		}

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
	public class			Cubic : Bezier
	{
		public Cubic(Vector P1, Vector P2, Vector P3, Vector P4) : base(new List<Vector> { P1, P2, P3, P4 })
		{ }
	}
	public class			Quadratic : Bezier
	{
		public Quadratic(Vector P1, Vector P2, Vector P3) : base(new List<Vector> { P1, P2, P3 })
		{ }
	}
	public class			Line : Curve
	{
		public override eType Type
		{
			get
			{
				return eType.eLine;
			}
		}
		public Line(Vector P1, Vector P2)
		{
			Points = new Vector[] { P1, P2 };
		}
		public override Vector GetPoint(float pTime)
		{
			return WeightedPoint(Start, End, pTime);
		}
	}
	public class			Start : Curve
	{
		public override eType Type
		{
			get
			{
				return eType.eStart;
			}
		}
		public override Vector End
		{
			get
			{
				return Points[0];
			}
		}
		public Start(Vector P1)
		{
			Points = new Vector[] { P1 };
		}
		public override Vector GetPoint(float pTime)
		{
			return End;
		}
	}


	public class Polycurve : ICurve
	{
		public class EnhancedPath
		{
			private SKPath _Path;
			public SKPath Path
			{
				get
				{
					return _Path;
				}
			}

			private bool _Negative;
			public bool Negative
			{
				get
				{
					return _Negative;
				}
				set
				{
					_Negative = value;
				}
			}

			public EnhancedPath()
			{
				_Path = null;
				Negative = false;
			}
			public EnhancedPath(SKPath pPath, bool pNegative)
			{
				_Path = pPath;
				Negative = pNegative;
			}
		}

		List<EnhancedPath> mPath;

		private SKSize mBoundary;
		private SKSize? _CanvasSize;
		public SKSize CanvasSize
		{
			get
			{
				return _CanvasSize.Value;
			}
			set
			{
				_CanvasSize = value;
			}
		}
		bool CanvasSizeSet
		{
			get
			{
				return (_CanvasSize.HasValue);
			}
		}

		public float Width
		{
			get
			{
				return CanvasSize.Width;
			}
		}
		public float Height
		{
			get
			{
				return CanvasSize.Height;
			}
		}

		private string mName;
		private List<Curve> mCurves;

		//Interface functions
		public Vector Start
		{
			get
			{
				return mCurves[0].Start;
			}
		}

		public Vector LastStart
		{
			get
			{
				for ( var i = Count - 1; i >= 0; --i)
				{
					var curvs = mCurves[i];
					if (mCurves[i].GetType() == typeof(Start))
						return curvs.Start;
				}
				return Start;
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
		public int	  Count
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
		public void	 AddLine(Vector pPoint)
		{
			mCurves.Add(new Line(End, pPoint));
		}
		public void	 AddQuadratic(Vector pControl, Vector pEnd)
		{
			mCurves.Add(new Quadratic(End, pControl, pEnd));
		}
		public void	 AddCubic(Vector pControl1, Vector pControl2, Vector pEnd)
		{
			mCurves.Add(new Cubic(End, pControl1, pControl2, pEnd));
		}
		public void	 AddBezier(Vector[] pPoints)
		{
			var pts = new List<Vector>(pPoints);
			pts.Insert(0, End);
			mCurves.Add(new Bezier(pts));
		}
		public void	 AddStart(Vector pPoint)
		{
			mCurves.Add(new Start(pPoint));
		}
		public void	 CloseCurve()
		{
			if (LastStart.Equals(End))
				return;
			AddLine(LastStart);
		}

		//Constructor
		public		  Polycurve(string name)
		{
			mPath = new List<EnhancedPath>();
			mName = name;
			mCurves = new List<Curve>();
			mBoundary = new SKSize(0, 0);
		}
		public		  Polycurve(string name, Vector pStart)
		{
			mPath = new List<EnhancedPath>();
			mName = name;
			mCurves = new List<Curve>();
			mCurves.Add(new Start(pStart));
			mBoundary = new SKSize(0, 0);
		}

		//Update routines to setup things like width, height, minimum, maximum
		public void Update ()
		{
			var TRect = new SKRect();
			var Pth = new SKPath();

			bool Set = true;

			var xmax = 0.0f;
			var ymax = 0.0f;

			GenerateCache (0.5f);
			foreach (var pth in mPath)
			{
				if (pth.Path.GetBounds(out TRect))
				{
					var xmax_n = TRect.Right;
					var ymax_n = TRect.Bottom;

					if (Set)
					{
						Set = false;
						ymax = ymax_n;
						xmax = xmax_n;
					}
					if (xmax_n > xmax)
						xmax = xmax_n;
					if (ymax_n > ymax)
						ymax = ymax_n;
				}
			}
			mBoundary = new SKSize(xmax, ymax);
		}

		//
		public SKMatrix Transformation = SKMatrix.MakeIdentity();

		//
		private SKRectI ToIntRect(SKRect value)
		{
			int x, r, y, b;
			//Horozontal scaling.
			if (value.Right > value.Left)
			{
				//Standard rectangle
				x = (int)Math.Floor(value.Left);
				r = (int)Math.Ceiling(value.Right);
			}
			else
			{
				//Inverted rectangle
				x = (int)Math.Ceiling(value.Left);
				r = (int)Math.Floor(value.Right);

			}

			//Vertical scaling
			if (value.Bottom > value.Top)
			{
				//Standard rectangle
				y = (int)Math.Floor(value.Top);
				b = (int)Math.Ceiling(value.Bottom);
			}
			else
			{
				//Inverted rectangle
				y = (int)Math.Ceiling(value.Top);
				b = (int)Math.Floor(value.Bottom);
			}

			return new SKRectI(x, y, r, b);
		}

		//Default resolution makes 10 points per line segment
		private bool IsInside(SKPath pInput, SKPath pBoundary)
		{
			var input = new SKRegion();
			var boundary = new SKRegion();

			input.SetRect   (ToIntRect(pInput.Bounds));
			boundary.SetRect(ToIntRect(pBoundary.Bounds));
			input.SetPath(pInput);
			boundary.SetPath(pBoundary);

			var contained = boundary.Contains(input);
			return contained;
		}

		bool MakeCache = true;
		public bool GenerateCache(float Resolution)
		{
			if (!CanvasSizeSet)
				throw (new Exception("Geometry type is a vector format must have canvassize set!"));

			if (Resolution == 0f)
				return false;

			var Pts = new List<SKPoint>();
			var Limit = 1.0f - Resolution;
			
			//
			bool Skip = true;
			foreach(var curv in mCurves)
			{ 
				if (Skip)
					Pts.Add(curv.Start);

				switch (curv.Type)
				{
					case Curve.eType.eBezier:
						for (float time = Resolution; time <= Limit; time += Resolution)
							Pts.Add(curv.GetPoint(time));

						//More accurate than using reosolution
						Pts.Add(curv.End);
						break;

					case Curve.eType.eLine:
						//Start is already there
						Pts.Add(curv.End);
						break;

					case Curve.eType.eStart:
						if (Skip)
						{
							Skip = false;
							continue;
						}
						var pth = new SKPath();
						pth.AddPoly(Pts.ToArray(), false);
						pth.Transform(Transformation);

						//Add path to cache
						mPath.Add(new EnhancedPath(pth, false));
						Pts.Clear();
						Pts.Add(curv.Start);
						break;
				}
			}

			//Add the last curve to the system
			var Path = new SKPath();
			Path.AddPoly(Pts.ToArray(), false);
			Path.Transform(Transformation);

			//Add path to cache
			mPath.Add(new EnhancedPath(Path, false));
			MakeCache = false;

			//Setup whether each path is inside or outside each other, first sort by area, largest area should be first.
			//Assuming largest is drawn first as per the evenodd/not-zero styling dictates in svg
			for ( var i = 0; i < mPath.Count - 1; ++i )
			{
				var parent = mPath[i];
				for ( var j = i + 1; j < mPath.Count; ++j )
				{
					var child = mPath[j];
					if (parent == child)
						continue;

					//If it is contained it is negative
					if ( IsInside(child.Path, parent.Path ) )
						child.Negative = !parent.Negative;
				}
			}
			return true;
		}

		public void Draw (ref SKCanvas pSurface, SKMatrix pTransform, ref SKPaint pDrawPaint, ref SKPaint pUndrawPaint)
		{
			if ( MakeCache )
				GenerateCache ( 0.1f );

			foreach ( var pth in mPath )
			{
				var path = new SKPath( pth.Path );
				path.Transform(pTransform);

				if (pth.Negative)
					pSurface.DrawPath(path, pUndrawPaint);
				else
					pSurface.DrawPath(path, pDrawPaint);
			}
		}
	}
}
