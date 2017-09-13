using SkiaSharp;
using SkiaSharp.Views.Forms;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System;

namespace rMultiplatform
{
    public abstract class ASmartData : ASmartElement
    {
        public SmartChart       Parent = null;
        public ASmartAxisPair   Axis = null;

        protected SKPaint DataPaint = MakeDefaultPaint(Globals.TextColor, 1, Globals.MajorFontSize, Globals.Typeface, IsStroke:true);


        public ObservableCollection<SKPoint> Points;
        private List<SKPoint> VisiblePoints
        {
            get
            {
                int DataStart = -1;
                int DataEnd = -1;
                bool IsDataStart(SKPoint arg, int index)
                {
                    var result = arg.X >= Axis.Horizontal.ValueStart;
                    if (DataStart == -1)
                        if (result)
                            DataStart = index;
                    return result;
                }
                bool IsDataEnd(SKPoint arg, int index)
                {
                    var result = arg.X > Axis.Horizontal.ValueEnd;
                    if (DataEnd == -1)
                        if (result)
                            DataStart = index;
                    return result;
                }
                Points.Where(IsDataStart);
                Points.Where(IsDataEnd);

                var Length = Points.Count;

                if (DataStart < 0)
                    DataStart = 0;

                if (DataEnd > Length || DataEnd < 0)
                    DataEnd = Length;

                if (DataEnd < DataStart)
                    DataStart = DataEnd;

                return Points.ToList().GetRange(DataStart, (DataEnd - DataStart));
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
            Points = new ObservableCollection<SKPoint>();
            Axis = pAxis;

            pAxis.Horizontal.Range.Set(-1, 10);
            pAxis.Vertical.Range.Set(-1, 1);
            for (var time = 0.0f; time < 10999.0f; time += 0.1f)
                Points.Add(new SKPoint(time, Globals.RandomBetween(-1.0f, 1.0f)));
        }
    }

    public class SmartData : ASmartData
    {
        public override void Draw(SKCanvas Canvas, SKSize dimension)
        {
            if (Points.Count == 0)
                return;

            (var path, var bounds) = VisiblePath;

            Axis.Set        (bounds);                       //Set the axis limits
            Axis.Draw       (Canvas, dimension);            //Render the axis with limits
            path.Transform  (Axis.Transform(dimension));    //Transform the path to fit limits
            Canvas.DrawPath (path, DataPaint);              //Render scaled and shifted path
        }
        public SmartData(ASmartAxisPair pAxis, ObservableCollection<SKPoint> pData) : base(pAxis)
        {
            DataPaint.IsStroke  = true;
            DataPaint.Color     = Globals.UniqueColor().ToSKColor();
            Axis.Parent         = this;
            Points = pData;
            Points.CollectionChanged += Points_CollectionChanged;
        }
        private void Points_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //Parent.InvalidateSurface();
        }
    }
}