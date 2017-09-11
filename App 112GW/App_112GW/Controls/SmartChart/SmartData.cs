using System.Collections.Generic;
using SkiaSharp;

namespace rMultiplatform
{
    abstract class ASmartData : ASmartElement
    {
        public SmartChart       Parent;
        public ASmartAxisPair   Axis;

        public List<ChartAxis>  Registrants;
        public List<SKPoint>    Points;

        public abstract void Draw(SKCanvas Canvas, SKSize dimension);
    }

    class SmartData : ASmartData
    {
        public override void Draw(SKCanvas Canvas, SKSize dimension)
        {
            if (Points.Count == 0)
                return;
            
            var DataStart   = Points.FindIndex(val => (val.X >= Axis.Horizontal.ValueStart));
            var DataEnd     = Points.FindIndex(val => (val.X >  Axis.Horizontal.ValueEnd));
            var Length      = Points.Count;

            if (DataStart < 0)
                DataStart = 0;
            if (DataEnd > Length || DataEnd < 0)
                DataEnd = Length;
            
            if (DataEnd > DataStart)
            {
                var path = new SKPath();
                var output = Points.GetRange(DataStart, (DataEnd - DataStart)).ToArray();
                path.AddPoly(output, false);
                var bounds = path.Bounds;
                Axis.Set(bounds);
                Axis.Draw(Canvas, dimension);

                path.Transform(Axis.Transform);
                Canvas.DrawPath(path, MajorPaint);
            }
        }
    }
}
