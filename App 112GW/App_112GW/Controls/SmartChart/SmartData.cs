using System.Collections.Generic;
using SkiaSharp;

namespace rMultiplatform
{
    public abstract class ASmartData : ASmartElement
    {
        public SmartChart       Parent = null;
        public ASmartAxisPair   Axis = null;

        public List<SKPoint> Points;
        private List<SKPoint> VisiblePoints
        {
            get
            {
                var DataStart   = Points.FindIndex(val => (val.X >= Axis.Horizontal.ValueStart  ));
                var DataEnd     = Points.FindIndex(val => (val.X >  Axis.Horizontal.ValueEnd    ));
                var Length = Points.Count;

                if (DataStart < 0)
                    DataStart = 0;
                if (DataEnd > Length || DataEnd < 0)
                    DataEnd = Length;
                if (DataEnd < DataStart)
                    DataStart = DataEnd;

                return Points.GetRange(DataStart, (DataEnd - DataStart));
            }
        }
        protected (SKPath, SKRect) VisiblePath
        {
            get
            {
                var path = new SKPath();
                path.AddPoly(VisiblePoints.ToArray(), false);
                return (path, path.Bounds);
            }
        }

        public abstract void Draw(SKCanvas Canvas, SKSize dimension);
        
        public ASmartData(ASmartAxisPair pAxis)
        {
            Points = new List<SKPoint>();
            Axis = pAxis;
            for (var time = 0.0f; time < 10.0f; time += 0.1f)
                Points.Add(new SKPoint(time, Globals.RandomBetween(-1.0f, 1.0f)));
        }
    }

    public class SmartData : ASmartData
    {
        public override void Draw(SKCanvas Canvas, SKSize dimension)
        {
            if (Points.Count > 0)
            {
                (var path, var bounds) = VisiblePath;

                Axis.Set        (bounds);                       //Set the axis limits
                Axis.Draw       (Canvas, dimension);            //Render the axis with limits
                path.Transform  (Axis.Transform(dimension));    //Transform the path to fit limits
                Canvas.DrawPath (path, MajorPaint);             //Render scaled and shifted path
            }
        }
        public SmartData(ASmartAxisPair pAxis) : base(pAxis)
        {
            Axis.Parent = this;
        }
    }
}