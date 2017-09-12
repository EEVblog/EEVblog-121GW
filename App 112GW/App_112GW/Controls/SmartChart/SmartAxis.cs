using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace rMultiplatform
{
    public abstract class ASmartAxis : ASmartElement
    {
        public ASmartAxisPair   Parent  { get; }
        public CappedRange      Range   { get; set; }
        public string           Label   { get; set; }

        public float Distance => (float)Range.Distance;

        public ASmartTick Ticker;
        public float Position { get; set; }

        public abstract float Dimension(SKSize dimensions);
        public abstract float AxisStart(float WidthXorHeight);
        public abstract float AxisEnd(float WidthXorHeight);

        public float AxisSize(float WidthXorHeight) => Math.Abs(AxisEnd(WidthXorHeight) - AxisStart(WidthXorHeight));

        public float ValueStart => (float)Range.Minimum;
        public float ValueEnd   => (float)Range.Maximum;

        public uint MinorTicks  { get; set; } = 5;
        private uint MajorTicks { get; set; } = 4;

        private float MajorTickDistance => Distance / MajorTicks;
        private float MinorTickDistance => MajorTickDistance / MinorTicks;

        //Used in the creation of an axis pair
        public float Scaling    (float dimension)   => AxisSize (dimension) / Distance;
        public float Translation(float dimension)   => AxisStart(dimension);

        //Used to interface with touch screen
        public float ValueFromCoordinate(float dimension, float Value)
        {
            Value -= AxisStart(dimension);
            Value /= Scaling(dimension);
            Value += ValueStart;
            return Value;
        }
        public float CoordinateFromValue(float dimension, float Value)
        {
            Value -= ValueStart;
            Value *= Scaling(dimension);
            Value += AxisStart(dimension);
            return Value;
        }

        //
        public void Draw(SKCanvas canvas, SKSize dimension)
        {
            var draw_value_major_end = ValueEnd + MajorTickDistance / 2;
            for (Ticker.Value = ValueStart;
                Ticker.Value <= draw_value_major_end; 
                Ticker.Value += MajorTickDistance)
            {
                Ticker.TickType = ASmartTick.SmartTickType.Major;
                Ticker.Draw(canvas, dimension);

                Ticker.TickType = ASmartTick.SmartTickType.Minor;
                var draw_value_minor_end    = Ticker.Value + MajorTickDistance - MinorTickDistance / 2;
                var draw_value_minor_start  = Ticker.Value + MinorTickDistance;
                var value = Ticker.Value;
                if (draw_value_minor_end < draw_value_major_end)
                    for (Ticker.Value = draw_value_minor_start; 
                        Ticker.Value <= draw_value_minor_end; 
                        Ticker.Value += MinorTickDistance)

                        Ticker.Draw(canvas, dimension);


                Ticker.Value = value;
            }
        }
        public ASmartAxis(string pLabel, float pMinimum, float pMaximum)
        {
            Label = pLabel;
            Range = new CappedRange(pMinimum, pMaximum);
        }
    }
    public class SmartAxisHorizontal : ASmartAxis
    {
        public override float AxisStart (float Width)   => Padding.LeftPosition(Width);
        public override float AxisEnd   (float Width)   => Padding.RightPosition(Width);

        public override float Dimension(SKSize dimensions) => dimensions.Height;
        public SmartAxisHorizontal(string pLabel, float pMinimum, float pMaximum) : base(pLabel, pMinimum, pMaximum)
        {
            Ticker = new SmartTickHorizontal(this, ASmartTick.SmartTickType.Major);
        }
    }
    public class SmartAxisVertical : ASmartAxis
    {
        public override float AxisStart (float Height)  => Padding.TopPosition(Height);
        public override float AxisEnd   (float Height)  => Padding.BottomPosition(Height);

        public override float Dimension(SKSize dimensions) => dimensions.Width;
        public SmartAxisVertical(string pLabel, float pMinimum, float pMaximum) : base(pLabel, pMinimum, pMaximum)
        {
            Ticker = new SmartTickVertical(this, ASmartTick.SmartTickType.Major);
        }
    }
}