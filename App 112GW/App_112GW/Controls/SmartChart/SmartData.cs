using SkiaSharp;
using SkiaSharp.Views.Forms;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System;
using System.Threading;

namespace rMultiplatform
{
    public abstract class ASmartData : ASmartElement
    {
        public SmartChart       Parent  = null;
        public ASmartAxisPair   Axis    = null;

        protected SKPaint DataPaint = MakeDefaultPaint(Globals.TextColor, 1, Globals.MajorFontSize, Globals.Typeface, IsStroke:true);

        public TSObservableCollection<SKPoint>    Points;

        public void Reset()
        {
            Points.Clear();
        }

        private List<SKPoint> PointsList
        {
            get
            {
                return Points.ToList();
            }
        }
        private SKPoint[] PointsArray
        {
            get
            {
                return Points.ToList().ToArray();
            }
        }
        protected (SKPath, SKRect) Path
        {
            get
            {
                var path = new SKPath();
                path.AddPoly(PointsArray, false);
                var bounds = path.Bounds;
                return (path, bounds);
            }
        }

        public string GetCSV()
        {
            var points = PointsList;
            if (points.Count > 1)
            {
                //The fallback values of axis labels are X, Y
                string horozontal_label = "time (s)";
                string vertical_label = Parent.Title;

                //The header row of the CSV
                string output = horozontal_label + ", " + vertical_label + "\r\n";

                //Print the rows of the CSV to the string.
                foreach (var item in points)
                    output += item.X.ToString() + ", " + item.Y.ToString() + "\r\n";

                //Return output ;) troll comment
                return output;
            }
            return "";
        }

        public abstract void Draw(SKCanvas Canvas, SKSize dimension);
        
        public ASmartData(ASmartAxisPair pAxis)
        {
            Points = new TSObservableCollection<SKPoint>();
            Axis = pAxis;

            pAxis.Horizontal.Range.Set(0f, 0.1f);
            pAxis.Vertical.Range.Set(-0.1f, 0.1f);
        }
    }
    public class SmartData : ASmartData
    {
        public override void Draw(SKCanvas Canvas, SKSize dimension)
        {
            if (Points.Count == 0)
                return;

            (var path, var bounds) = Path;

            Axis.Set        (bounds);                       //Set the axis limits
            Axis.Draw       (Canvas, dimension);            //Render the axis with limits
            path.Transform  (Axis.Transform(dimension));    //Transform the path to fit limits

            //This only draws the path in the render region (between axis)
            Canvas.ClipRect (Axis.AxisClip(dimension));
            Canvas.DrawPath (path, DataPaint);              //Render scaled and shifted path
        }
        public SmartData(ASmartAxisPair pAxis, TSObservableCollection<SKPoint> pData) : base(pAxis)
        {
            DataPaint.IsStroke  = true;
            DataPaint.Color     = Globals.UniqueColor().ToSKColor();
            Axis.Parent         = this;
            Points = pData;
            Points.CollectionChanged += Points_CollectionChanged;
        }
        private void Points_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Parent.InvalidateSurface();
        }
    }
}