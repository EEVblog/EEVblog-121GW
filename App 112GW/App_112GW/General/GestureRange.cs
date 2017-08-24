using System;
using System.Collections.Generic;
using System.Text;

namespace rMultiplatform
{
    class GestureRange
    {
        private enum Current
        {
            Visible,
            Boundary
        };

        private Range   Visible;
        private Range   Boundary;
        private Current Select;

        public double Minimum
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
                Boundary.Minimum = value;
                if (value < Visible.Minimum)
                    Visible.Minimum = value;
            }
        }
        public double Maximum
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
                Maximum = value;
                if (value > Visible.Maximum)
                    Visible.Maximum = value;
            }
        }
        public double Distance
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

        public void Set(Range Input)
        {
            Minimum = Input.Minimum;
            Maximum = Input.Maximum;
        }
        public void Set(double ValA, double ValB)
        {
            if (ValA > ValB)
            {
                Minimum = ValB;
                Maximum = ValA;
            }
            else
            {
                Minimum = ValA;
                Maximum = ValB;
            }
        }

        public bool InRange(double Val)
        {
            return (Minimum <= Val) && (Val <= Maximum);
        }
        public void AddToMaximum(double Value)
        {
            Maximum += Value;
            if (Maximum < Minimum)  Minimum = Minimum;
        }
        public void AddToMinimum(double Value)
        {
            Minimum += Value;
            if (Minimum > Maximum)  Maximum = Minimum;
        }

        public void ShiftRange(double Value)
        {
            Minimum += Value;
            Maximum += Value;
        }
        public void ShiftRangeToFitValue(double Value)
        {
            double diff = 0.0;
            if (Value > Maximum)
                diff = Value - Maximum;
            else if (Value < Minimum)
                diff = Minimum - Value;
            ShiftRange(diff);
        }

        public void RescaleRangeToFitValue(double Value)
        {
            if      (Value > Maximum)   Maximum = (Value);
            else if (Value < Minimum)   Minimum = (Value);
        }

        public GestureRange(double A, double B)
        {
            Set(A, B);
        }
    }
}