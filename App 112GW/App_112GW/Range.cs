using System;
using System.Collections.Generic;
using System.Text;

namespace rMultiplatform
{
    public class Range
    {
        private double _Maximum;
        public double Maximum
        {
            get
            {
                return _Maximum;
            }
            protected set
            {
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
            protected set
            {
                _Minimum = value;
            }
        }
        public double Distance
        {
            get{return Maximum - Minimum;}
        }
        public void Set     (double ValA, double ValB)
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
        public      Range   (double ValA, double ValB)
        {
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
    }
}
