using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

using Xamarin;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml.Internals;

using SkiaSharp;

namespace App_112GW
{
    public class SVGPath
    {
        public static SKMatrix BuildTransformMatrix(string Transform)
        {
            var Output = new SKMatrix();

            char[] delim = {',',' ','-', '(', ')'};
            var v_s = Transform.Split(delim, StringSplitOptions.RemoveEmptyEntries);

            if (v_s.Length == 0)
                throw (new Exception("Malformed transform."));

            //Get the name of the transform type
            int i = 0;
            var Type = v_s[i++].ToLower();

            //Setup matrix and zero cells
            float[] matrix = new float[6];
            for (int x = 0; x < matrix.Length; x++)
                matrix[x] = 0;

            //Switch to correct transform type
            switch (Type)
            {
                case "matrix":

                    matrix[0] = float.Parse(v_s[i++]);
                    matrix[1] = float.Parse(v_s[i++]);
                    matrix[2] = float.Parse(v_s[i++]);
                    matrix[3] = float.Parse(v_s[i++]);
                    matrix[4] = float.Parse(v_s[i++]);
                    matrix[5] = float.Parse(v_s[i++]);
                    break;
                case "translate":
                    var tx = float.Parse(v_s[i++]);

                    var ty = 0.0f;
                    if (i < v_s.Length)
                        ty = float.Parse(v_s[i++]);

                    //
                    matrix[0] = 1;
                    matrix[4] = 1;
                    matrix[2] = tx;
                    matrix[5] = ty;
                    break;
                case "scale":
                    var sx = float.Parse(v_s[i++]);

                    var sy = 0.0f;
                    if (i < v_s.Length)
                        sy = float.Parse(v_s[i++]);

                    //
                    matrix[0] = sx;
                    matrix[4] = sy;
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
                    const float convt = ((float)Math.PI / 180.0f);
                    var angl_degrees = convt * angle;

                    //
                    matrix[0] =  (float)Math.Cos(angl_degrees);
                    matrix[1] = -(float)Math.Sin(angl_degrees);
                    matrix[3] =  (float)Math.Sin(angl_degrees);
                    matrix[4] =  (float)Math.Cos(angl_degrees);
                    break;
                case "skewX":
                    var sk_x_angle = float.Parse(v_s[i++]);
                    angl_degrees = convt * sk_x_angle;

                    matrix[0] = 1;
                    matrix[1] = (float)Math.Tan(angl_degrees);
                    matrix[4] = 1;
                    break;
                case "skewY":
                    var sk_y_angle = float.Parse(v_s[i++]);
                    angl_degrees = convt * sk_y_angle;

                    matrix[0] = 1;
                    matrix[3] = (float)Math.Tan(angl_degrees);
                    matrix[4] = 1;
                    break;
            };

            Output.ScaleX = matrix[0];
            Output.SkewX = matrix[1];
            Output.TransX = matrix[2];
            Output.SkewY = matrix[3];
            Output.ScaleY = matrix[4];
            Output.TransY = matrix[5];
            Output.Persp0 = 0;
            Output.Persp1 = 0;
            Output.Persp2 = 1;

            return Output;
        }

        public SVGPath(string pPath, string Transform)
        {
            //

        }
    }
}
