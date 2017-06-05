using System;
using System.Collections.Generic;
using System.Text;

namespace rMultiplatform
{
    public class Range
    {
        private bool _Update;
        public bool Update
        {
            private set
            {
                _Update = value;
            }
            get
            {
                _Update = false;
                return _Update;
            }
        }

        private double _Maximum;
        public double Maximum
        {
            get
            {
                return _Maximum;
            }
            set
            {
                Update = true;
                _Maximum = value;
            }
        }

        private double _Minimum;
        public double Minimum
        {
            get
            {
                return _Minimum;
            }
            set
            {
                Update = true;
                _Minimum = value;
            }
        }

        public double Distance
        {
            get{return Maximum - Minimum;}
        }
        public void Set     (double ValA, double ValB)
        {
            Update = true;
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

        public      Range   (double ValA, double ValB)
        {
            Update = true;
            Set(ValA, ValB);
        }
        public bool InRange (double Val)
        {
            return (Minimum <= Val) && (Val <= Maximum);
        }

        public void AddToMaximum(double Value)
        {
            Maximum += Value;
        }
        public void AddToMinimum(double Value)
        {
            Minimum += Value;
        }
        public void ShiftRange(double Value)
        {
            AddToMaximum(Value);
            AddToMinimum(Value);
        }
        public void ShiftRangeToFitValue(double Value)
        {
            var diff = (double)0;

            if (Value > Maximum)
                diff = Value - Maximum;
            else if (Value < Minimum)
                diff = Minimum - Value;

            //Shift the range to fit the value
            ShiftRange(diff);
        }
        public void RescaleRangeToFitValue(double Value)
        {
            if (Value > Maximum)
                Maximum = (Value);
            else if (Value < Minimum)
                Minimum = (Value);
        }

        //Combines numerous ranges
        public static Range Combine(List<Range> Ranges)
        {
            Range output = null;
            foreach (var rng in Ranges)
            {
                if (rng == null)
                    continue;

                if (output == null)
                    output = new Range(rng.Minimum, rng.Maximum);

                if (rng.Minimum < output.Minimum)
                    output.Minimum = rng.Minimum;
                if (rng.Maximum > output.Maximum)
                    output.Maximum = rng.Maximum;
            }
            return output;
        }
        public static Range Combine(Range A, Range B)
        {
            var temp = new List<Range>();
            temp.Add(A); temp.Add(B);
            return Combine(temp);
        }
    }
}
