using SkiaSharp;

namespace rMultiplatform
{
    abstract class ASmartTick : ASmartElement
    {
        public ASmartAxis Parent        { get; private set; }

        public static float SpaceWidth = MajorPaint.MeasureText(" ");
        public float        Value;

        public static bool  ShowTick        { get; set; }
        public static bool  ShowMajorLabel  { get; set; }
        public static bool  ShowGridline    { get; set; }

        private static float _MajorTickLength;
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

        public enum SmartTickType
        {
            Major,
            Minor
        }
        private SmartTickType TickType;
        public float TickLength
        {
            get
            {
                return (TickType == SmartTickType.Major)? MajorTickLength : MinorTickLength;
            }
        }

        protected abstract (float x, float y)       TickStart { get; }
        protected abstract (float x, float y)       TickEnd   { get; }
        protected abstract (SKPoint x, SKPoint y)   LabelLine(string Text);

        public void Draw(SKCanvas Canvas)
        {
            (var start_x,   var start_y)    = TickStart;
            (var end_x,     var end_y)      = TickEnd;

            if (ShowTick)
            {
                Canvas.DrawLine(start_x, start_y, end_x, end_y, MajorPaint);

                if ()
                {

                }
                if (ShowMajorLabel)
                {
                    var txt = SIPrefix.ToString(Value);
                    (var pt1, var pt2) = LabelLine(txt);
                    var pts = new SKPoint[] { pt1, pt2 };
                    var pth = new SKPath();
                    pth.AddPoly(pts, false);
                    Canvas.DrawTextOnPath(txt, pth, 0, 0, MinorPaint);
                }
            }

            //if (ShowGridline)
            //    Canvas.DrawLine();
        }
    }
    class SmartTickHorizontal : ASmartTick
    {
        protected override (float x, float y) TickStart
        {
            get
            {
                var value = Parent.CoordinateFromValue(Value);
                return (Value, Parent.Location - TickLength);
            }
        }
        protected override (float x, float y) TickEnd
        {
            get
            {
                var value = Parent.CoordinateFromValue(Value);
                return (Value, Parent.Location + TickLength);
            }
        }

        protected override (SKPoint x, SKPoint y) LabelLine(string Text)
        {
            (var wid, var hei) = MeasureText(Text);
            return (new SKPoint(Position, Parent.AxisStart + SpaceWidth + wid),
                    new SKPoint(Position, Parent.AxisStart + SpaceWidth));
        }
    }
    class SmartTickVertical : ASmartTick
    {
        protected override (float x, float y) TickStart
        {
            get
            {
                var value = Parent.CoordinateFromValue(Value);
                return (Parent.Location - TickLength, Value);
            }
        }
        protected override (float x, float y) TickEnd
        {
            get
            {
                var value = Parent.CoordinateFromValue(Value);
                return (Parent.Location + TickLength, Value);
            }
        }

        protected override (SKPoint x, SKPoint y) LabelLine(string Text)
        {
            (var wid, var hei) = MeasureText(Text);
            return (new SKPoint(Parent.AxisStart - SpaceWidth - wid,    Position),
                    new SKPoint(Parent.AxisStart - SpaceWidth,          Position));
        }
    }
}
