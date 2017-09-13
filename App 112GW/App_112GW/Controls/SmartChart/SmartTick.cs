using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace rMultiplatform
{
    public abstract class ASmartTick : ASmartElement
    {
        public ASmartAxis Parent        { get; private set; }
        public float Value;
        public enum SmartTickType
        {
            Major,
            Minor
        }
        public SmartTickType TickType;

        public float Position(float dimension) => Parent.CoordinateFromValue(dimension, Value);
        
        public static float SpaceWidth = MajorPaint.MeasureText(" ");
        public static bool  ShowTick             { get; set; } = true;
        public static bool  ShowMajorLabel       { get; set; } = true;
        public static bool  ShowMajorGridline    { get; set; } = true;
        public static bool  ShowMinorGridline    { get; set; } = false;

        public bool ShowGridline => (TickType == SmartTickType.Major) ? ShowMajorGridline : ShowMinorGridline;

        public bool IsMajorTick => TickType == SmartTickType.Major;
        public bool IsMinorTick => TickType == SmartTickType.Minor;

        private static float _MajorTickLength = 5.0f;
        private static float MajorTickLength
        {
            get
            {
                return _MajorTickLength;
            }
            set
            {
                _MajorTickLength = value;
            }
        }
        private static float MinorTickLength
        {
            get
            {
                return MajorTickLength / 2;
            }
        }

        public float            TickLength
        {
            get
            {
                return (TickType == SmartTickType.Major)? MajorTickLength : MinorTickLength;
            }
        }

        protected abstract (float x, float y) TickStart(SKSize dimensoin);
        protected abstract (float x, float y) TickEnd(SKSize dimensoin);
        protected abstract (float x, float y) TickCentre(SKSize dimension);
        private (float x1, float y1, float x2, float y2) TickLine(SKSize dimension)
        {
            (var start_x, var start_y) = TickStart(dimension);
            (var end_x, var end_y) = TickEnd(dimension);
            return (start_x, start_y, end_x, end_y);
        }

        protected float GridLineDimension(SKSize dimensions) => Parent.Dimension(dimensions);
        protected abstract (float x1, float y1, float x2, float y2) GridLine(SKSize dimension);

        protected abstract (SKPoint x, SKPoint y) LabelLine(SKSize dimension, string Text);
        private (string, SKPath) LabelPath(SKSize dimension)
        {
            var txt = SIPrefix.ToString(Value);
            (var pt1, var pt2) = LabelLine(dimension, txt);
            var pts = new SKPoint[] { pt1, pt2 };
            var pth = new SKPath();
            pth.AddPoly(pts, false);
            return (txt, pth);
        }

        public void Draw ( SKCanvas canvas, SKSize dimension)
        {
            if (ShowTick)
            {
                (var start_x, var start_y, var end_x, var end_y) = TickLine(dimension);
                canvas.DrawLine(start_x, start_y, end_x, end_y, MajorPaint);

                if (IsMajorTick)
                {
                    if (ShowMajorLabel)
                    {
                        (var txt, var pth) = LabelPath(dimension);
                        canvas.DrawTextOnPath(txt, pth, 0, 0, MajorPaint);
                    }
                }
            }
            if (ShowGridline)
            {
                (var x1, var y1, var x2, var y2) = GridLine(dimension);
                canvas.DrawLine(x1, y1, x2, y2, GridPaint);
            }
        }

        public ASmartTick(ASmartAxis pParent, SmartTickType Type)
        {
            Parent = pParent;
            TickType = Type;
        }
    }
    public class SmartTickHorizontal : ASmartTick
    {
        protected override (float x, float y)   TickStart   (SKSize dimension) => (Position(dimension.Width), Parent.Position - TickLength);
        protected override (float x, float y)   TickEnd     (SKSize dimension) => (Position(dimension.Width), Parent.Position + TickLength);
        protected override (float x, float y)   TickCentre  (SKSize dimension) => (Position(dimension.Width), Parent.Position);

        protected override (float x1, float y1, float x2, float y2) GridLine    (SKSize dimension) => Padding.GetVerticalLine(dimension.Height, Position(dimension.Width));
        protected override (SKPoint x, SKPoint y)                   LabelLine   (SKSize dimension, string Text)
        {
            (var wid,   var hei)    = MeasureMajorText(Text);
            (var tx,    var ty)     = TickEnd(dimension);
            var pt1 = new SKPoint(tx, ty + SpaceWidth + wid);
            var pt2 = new SKPoint(tx, ty + SpaceWidth);
            return (pt1, pt2);
        }

        public SmartTickHorizontal(ASmartAxis Parent, SmartTickType Type) : base(Parent, Type) { }
    }
    public class SmartTickVertical : ASmartTick
    {
        protected override (float x, float y)   TickStart   (SKSize dimension) => (Parent.Position - TickLength,    dimension.Height - Position(dimension.Height));
        protected override (float x, float y)   TickEnd     (SKSize dimension) => (Parent.Position + TickLength,    dimension.Height - Position(dimension.Height));
        protected override (float x, float y)   TickCentre  (SKSize dimension) => (Parent.Position,                 dimension.Height - Position(dimension.Height));

        protected override (float x1, float y1, float x2, float y2) GridLine    (SKSize dimension) => Padding.GetHorizontalLine(dimension.Width, Position(dimension.Height));
        protected override (SKPoint x, SKPoint y)                   LabelLine   (SKSize dimension, string Text)
        {
            (var wid,   var hei)    = MeasureMajorText(Text);
            (var tx,    var ty)     = TickStart(dimension);
            var pt1 = new SKPoint(tx - SpaceWidth - wid, ty);
            var pt2 = new SKPoint(tx - SpaceWidth, ty);
            return (pt1, pt2);
        }

        public SmartTickVertical(ASmartAxis Parent, SmartTickType Type) : base(Parent, Type) { }
    }
}