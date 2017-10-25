using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

using Xamarin;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml.Internals;

using SkiaSharp;

namespace rMultiplatform
{
	public class SVGPath
	{
		SKMatrix LocalTransform;

		public static SKMatrix BuildTransformMatrix(string Transform)
		{
			var Output = SKMatrix.MakeIdentity();

			char[] delim = {',',' ', '(', ')'};
			var v_s = Transform.Split(delim, StringSplitOptions.RemoveEmptyEntries);

			if (v_s.Length == 0)
				throw (new Exception("Malformed transform."));

			//Get the name of the transform type
			int i = 0;
			var Type = v_s[i++].ToLower();

			//Switch to correct transform type
			switch (Type)
			{
				case "matrix":
					var a = float.Parse(v_s[i++]);
					var b = float.Parse(v_s[i++]);
					var c = float.Parse(v_s[i++]);
					var d = float.Parse(v_s[i++]);
					var e = float.Parse(v_s[i++]);
					var f = float.Parse(v_s[i++]);

				   
					Output.ScaleX   = a;
					Output.ScaleY   = d;
					Output.SkewX	= c;
					Output.SkewY	= b;
					Output.TransX   = e;
					Output.TransY   = f;

					break;
				case "translate":
					var tx = float.Parse(v_s[i++]);

					var ty = 0.0f;
					if (i < v_s.Length)
						ty = float.Parse(v_s[i++]);

					//
					Output = SKMatrix.MakeTranslation(tx, ty);
					break;
				case "scale":
					var sx = float.Parse(v_s[i++]);

					var sy = 0.0f;
					if (i < v_s.Length)
						sy = float.Parse(v_s[i++]);

					//
					Output = SKMatrix.MakeScale(sx, sy);
					break;
				case "rotate":
					//
					var angle = float.Parse(v_s[i++]);

					//
					var cx = 0.0f;
					if (i < v_s.Length)
						cx = float.Parse(v_s[i++]);
					
					//
					var cy = 0.0f;
					if (i < v_s.Length)
						cy = float.Parse(v_s[i++]);

					//
					Output = SKMatrix.MakeRotationDegrees(angle, cx, cy);
					break;
				case "skewX":
					var sk_x_angle = float.Parse(v_s[i++]);
					var anglx_radians = ((float)Math.PI/180.0f) * sk_x_angle;
					Output = SKMatrix.MakeSkew((float)Math.Tan(anglx_radians), 0);
					break;
				case "skewY":
					var sk_y_angle = float.Parse(v_s[i++]);
					var angly_radians = ((float)Math.PI / 180.0f) * sk_y_angle;
					Output = SKMatrix.MakeSkew(0, (float)Math.Tan(angly_radians));
					break;
			};

			// SVG always have these settings
			Output.Persp0   = 0;
			Output.Persp1   = 0;
			Output.Persp2   = 1;
			return Output;
		}

		public SVGPath(string pPath, string Transform)
		{
			//Identity matrix must always be default
			LocalTransform = SKMatrix.MakeIdentity();
		}
	}
}
