using System;
using System.Collections.Generic;
using System.Text;

namespace rMultiplatform
{
    public class SIValue
    {
        static private List<Tuple<double, string>> Units = new List<Tuple<double, string>>()
        {
            new Tuple<double, string>( 1e-12 ,"p"),
            new Tuple<double, string>( 1e-9  ,"n"),
            new Tuple<double, string>( 1e-6  ,"u"),
            new Tuple<double, string>( 1e-3  ,"m"),
            new Tuple<double, string>( 1     ," "),
            new Tuple<double, string>( 1e3   ,"k"),
            new Tuple<double, string>( 1e6   ,"M"),
            new Tuple<double, string>( 1e9   ,"G"),
            new Tuple<double, string>( 1e12  ,"T")
        };

        static public string SignificantFigure(double Value, int Figures)
        {
            int int_value = (int)Value;
            int mag_length = int_value.ToString().Length;
            Figures -= mag_length;

            int decimals = (int)((Value - (double)int_value) * Math.Pow(10, Figures));
            if (int_value == 0 && decimals == 0)
                return int_value.ToString() + "." + decimals.ToString();
            return int_value.ToString() + "." + decimals.ToString();
        }

        static public string ToString(double Value)
        {
            if (Value != 0.0)
            {
                foreach (var unit in Units)
                {
                    var range = unit.Item1 * 1000;
                    if (range > Value)
                    {
                        var label = unit.Item2;
                        var outval = (1000 * Value) / range;

                        var str = SignificantFigure(outval, 4);
                        if (str != "0.0")
                            return str + label;
                        return str;
                    }
                }
            }
            return "0.0";
        }
    }
}
