using System;
using System.Collections.Generic;
using System.Text;

namespace rMultiplatform
{
    public class CappedRange
    {
        private enum Current
        {
            Visible,
            Boundary
        };

        private Range   Visible;
        private Range   Boundary;
        private Current Select;
        private double  Dist(double A, double B)
        {
            if (A > B)
                return A - B;
            return B - A;
        }

        public double   Minimum
        {
            get
            {
                switch (Select)
                {
                    case Current.Visible:
                        return Visible.Minimum;
                    case Current.Boundary:
                        return Boundary.Minimum;
                    default:
                        throw new Exception("Not possible.");
                }
            }
            set
            {
                switch (Select)
                {
                    case Current.Visible:
                        if (value >= Boundary.Minimum)
                            Visible.Minimum = value;
                        break;
                    case Current.Boundary:
                        Boundary.Minimum = value;
                        if (value > Visible.Minimum)
                            Visible.Minimum = value;
                        break;
                    default:
                        throw new Exception("Not possible.");
                }
            }
        }
        public double   Maximum
        {
            get
            {
                switch (Select)
                {
                    case Current.Visible:
                        return Visible.Maximum;
                    case Current.Boundary:
                        return Boundary.Maximum;
                    default:
                        throw new Exception("Not possible.");
                }
            }
            set
            {
                switch (Select)
                {
                    case Current.Visible:
                        if (value <= Boundary.Maximum)
                            Visible.Maximum = value;
                        break;
                    case Current.Boundary:
                        Boundary.Maximum = value;
                        if (value < Visible.Maximum)
                            Visible.Maximum = value;
                        break;
                    default:
                        throw new Exception("Not possible.");
                }
            }
        }
        public double   Distance
        {
            get
            {
                switch (Select)
                {
                    case Current.Visible:
                        return Visible.Distance;
                    case Current.Boundary:
                        return Boundary.Distance;
                    default:
                        throw new Exception("Not possible.");
                }
            }
        }

        public void     Set(Range Input)
        {
            Select = Current.Boundary;
            Boundary.Minimum = Input.Minimum;
            Boundary.Maximum = Input.Maximum;
            Visible.Minimum = Input.Minimum;
            Visible.Maximum = Input.Maximum;
        }
        public void     Set(double ValA, double ValB)
        {
            if (ValA > ValB)    Set(new Range(ValB, ValA));
            else                Set(new Range(ValA, ValB));
        }

        public bool InRange(double Value)
        {
            return (Minimum <= Value) && (Value <= Maximum);
        }

        public void AddToMaximum(double Value)
        {
            Maximum += Value;
            if (Maximum < Minimum) Minimum = Minimum;
        }
        public void AddToMinimum(double Value)
        {
            Minimum += Value;
            if (Minimum > Maximum) Maximum = Minimum;
        }

        public void ShiftRange(double Value)
        {
            if (Value == 0.0)
                return;

            Minimum = Minimum + Value;
            Maximum = Maximum + Value;
        }
        public void ShiftFit(double Value)
        {
            double diff = 0.0;
            if      (Value > Maximum)
                diff = Value - Maximum;
            else if (Value < Minimum)
                diff = Minimum - Value;
            ShiftRange(diff);
        }
        public void ExpandFit(double Value)
        {
            if      (Value > Maximum) Maximum = Value;
            else if (Value < Minimum) Minimum = Value;
        }

        public void Pan(double Amount)
        {
            if (Select != Current.Visible)  return;

            var min = Visible.Minimum + Amount;
            var max = Visible.Maximum + Amount;

            if ( Boundary.InRange(min) && Boundary.InRange(max) )
            {
                Visible.Minimum = min;
                Visible.Maximum = max;
            }
        }
        public void Zoom(double Amount, double About)
        {
            if (Amount == 1.0)
                return;

            if (InRange(About))
            {

                var l = Dist(About, Minimum) / Amount;
                var h = Dist(About, Maximum) / Amount;
                var lower = About - l;
                var upper = About + h;
                int c = 0;

                //Clip upper and lower bounds
                //If zoomed out to full range re-enable normal mode.
                if (lower <= Minimum)
                {
                    ++c;
                    lower = Minimum;
                }
                if (upper >= Maximum)
                {
                    ++c;
                    upper = Maximum;
                }
                if (c == 2)
                    Select = Current.Boundary;
                else
                {
                    Select = Current.Visible;
                    Minimum = lower;
                    Maximum = upper;
                }
            }
        }

        public Range GetRange()
        {
            switch (Select)
            {
                case Current.Visible:
                    return Visible;
                case Current.Boundary:
                    return Boundary;
                default:
                    throw new Exception("Not possible.");
            }
        }

        public Range Combine(List<Range> mRanges)
        {
            var output = new Range(Boundary.Minimum, Boundary.Maximum);
            foreach(var item in mRanges)
            {
                if (output.Distance == 0)
                {
                    output.Minimum = item.Minimum;
                    output.Maximum = item.Maximum;
                }
                else
                {
                    var min = item.Minimum;
                    var max = item.Maximum;
                    output.Minimum = Math.Min(min, output.Minimum);
                    output.Maximum = Math.Max(max, output.Maximum);
                }
            }
            return output;
        }

        public CappedRange(double A, double B)
        {
            Boundary = new Range(A, B);
            Visible = new Range(A, B);
            Set(A, B);
        }
    }
}