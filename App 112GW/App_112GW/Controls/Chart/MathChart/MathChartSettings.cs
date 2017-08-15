using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using System.Diagnostics;

namespace rMultiplatform
{
    class MathChartSettings : Grid
    {
        static double SqrA_x_B      (double A, double B)
        {
            return (A * A) * B;
        }
        static double SqrA_Div_B    (double A, double B)
        {
            return (A * A) / B;
        }
        static double A_Div_SqrB    (double A, double B)
        {
            return A / (B * B);
        }

        static double Division      (double A, double B)
        {
            return A / B;
        }
        static double Addition      (double A, double B)
        {
            return A + B;
        }
        static double Subtraction   (double A, double B)
        {
            return A - B;
        }
        static double Multiplication(double A, double B)
        {
            return A * B;
        }

        delegate double Operation(double A, double B);
        Dictionary<string, Operation> Operations = new Dictionary<string, Operation>
        {
            { "A + B",              Addition        },
            { "A - B",              Subtraction     },
            { "A x B",              Multiplication  },
            { "A / B",              Division        },
            { "\u221AA x B",        SqrA_x_B        },
            { "\u221AA / B",        SqrA_Div_B      },
            { "A / \u221AB",        A_Div_SqrB      }
        };

        public MathChartSettings()
        {

        }
    }
}
