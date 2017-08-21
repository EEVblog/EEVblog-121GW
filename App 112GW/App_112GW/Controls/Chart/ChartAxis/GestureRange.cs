using System;
using System.Collections.Generic;
using System.Text;

namespace rMultiplatform
{
    class GestureRange
    {
        private Range Visible;
        private Range Boundary;
        private enum Current
        {
            Visible,
            Boundary
        }
        Current Select;

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

            FirstScaling = false;
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

        bool FirstScaling = true;
        public void RescaleRangeToFitValue(double Value)
        {
            if (FirstScaling)
            {
                Minimum = Value;
                Maximum = Value;
                FirstScaling = false;
            }
            if (Value > Maximum)
                Maximum = (Value);
            else if (Value < Minimum)
                Minimum = (Value);
        }
    }
}
