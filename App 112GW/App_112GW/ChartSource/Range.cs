using System;
using System.Collections.Generic;
using System.Text;

namespace rMultiplatform
{
    public class Range
    {
        protected double Maximum;
        protected double Minimum;

        public double Distance
        {
            get
            {
                return Maximum - Minimum;
            }
        }

        public      Range   (double ValA, double ValB)
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
        public bool InRange (double Val)
        {
            return (Minimum <= Val) && (Val <= Maximum);
        }
    }
}
