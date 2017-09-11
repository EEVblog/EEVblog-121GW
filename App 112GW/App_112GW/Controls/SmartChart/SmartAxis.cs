using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace rMultiplatform
{
    abstract class ASmartAxis : ASmartElement
    {
        public ASmartAxisPair Parent  { get; }
        public string Label   { get; set; }
        public CappedRange Range   { get; set; }
        public float Distance
        {
            get
            {
                return (float)Range.Distance;
            }
        }

        public ASmartTick Ticker;

        public float Position
        {
            get; set;
        }

        public abstract float AxisStart
        {
            get;
        }
        public abstract float AxisEnd
        {
            get;
        }
        public float AxisSize
        {
            get
            {
                return Math.Abs(AxisEnd - AxisStart);
            }
        }

        public float ValueStart
        {
            get
            {
                return (float)Range.Minimum;
            }
        }
        public float ValueEnd
        {
            get
            {
                return (float)Range.Maximum;
            }
        }

        private float MinorTickDistance;
        private float MajorTickDistance;

        private uint _MinorTicks;
        public uint MinorTicks
        {
            get
            {
                return _MinorTicks;
            }
            set
            {
                _MinorTicks = value;
                MinorTickDistance = MajorTickDistance / value;
            }
        }

        private uint _MajorTicks;
        private uint MajorTicks
        {
            get
            {
                return _MajorTicks;
            }
            set
            {
                _MajorTicks = value;
                MajorTickDistance = AxisSize / value;
                MinorTickDistance = MajorTickDistance / value;
            }
        }

        //Used in the creation of an axis pair
        public float Scaling
        {
            get
            {
                return (AxisSize / Distance);
            }
        }
        public float Translation
        {
            get
            {
                return AxisStart - Scaling * ValueStart;
            }
        }

        //Used to interface with touch screen
        public float ValueFromCoordinate(float Value)
        {
            Value -= AxisStart;
            Value /= Scaling;
            Value += ValueStart;
            return Value;
        }
        public float CoordinateFromValue(float Value)
        {
            Value -= ValueStart;
            Value *= Scaling;
            Value += AxisStart;
            return Value;
        }

        public void Draw(SKCanvas Canvas, SKSize dimension)
        {

            for (Ticker.Value = ValueStart; Ticker.Value <= ValueEnd; Ticker.Value += MinorTickDistance)
                Ticker.Draw(Canvas);
        }
    }

    class SmartAxisHorizontal : ASmartAxis
    {
        public override float AxisStart => throw new NotImplementedException();
        public override float AxisEnd   => throw new NotImplementedException();
    }
    class SmartAxisVertical : ASmartAxis
    {
        public override float AxisStart => throw new NotImplementedException();
        public override float AxisEnd   => throw new NotImplementedException();
    }
}
