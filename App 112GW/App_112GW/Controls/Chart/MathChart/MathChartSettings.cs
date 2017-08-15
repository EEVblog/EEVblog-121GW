using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using System.Diagnostics;
using App_112GW;

namespace rMultiplatform
{
    class MathChartSettings : Grid
    {
        private static double SqrA_x_B      (double A, double B)
        {
            return (A * A) * B;
        }
        private static double SqrA_Div_B    (double A, double B)
        {
            return (A * A) / B;
        }
        private static double A_Div_SqrB    (double A, double B)
        {
            return A / (B * B);
        }
        private static double Division      (double A, double B)
        {
            return A / B;
        }
        private static double Addition      (double A, double B)
        {
            return A + B;
        }
        private static double Subtraction   (double A, double B)
        {
            return A - B;
        }
        private static double Multiplication(double A, double B)
        {
            return A * B;
        }
        private delegate double Operation   (double A, double B);
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

        ListView A_List = new ListView();
        ListView B_List = new ListView();
        ListView Operation_List = new ListView();

        public MathChartSettings ( )
        {
            Padding = 10;
            HorizontalOptions           = LayoutOptions.Fill;
            VerticalOptions             = LayoutOptions.Fill; 

            Operation_List.ItemsSource  = Operations.Keys;
            BackgroundColor             = Globals.BackgroundColor;

            RowDefinitions.Add      ( new RowDefinition      { Height    = new GridLength ( 1, GridUnitType.Auto) } );
            RowDefinitions.Add      ( new RowDefinition      { Height    = new GridLength ( 1, GridUnitType.Auto) } );
            ColumnDefinitions.Add   ( new ColumnDefinition   { Width     = new GridLength ( 1, GridUnitType.Star) } );
            ColumnDefinitions.Add   ( new ColumnDefinition   { Width     = new GridLength ( 1, GridUnitType.Star) } );
            ColumnDefinitions.Add   ( new ColumnDefinition   { Width     = new GridLength ( 1, GridUnitType.Star) } );

            Children.Add(new Label() { Text = "A"           },  0, 0);
            Children.Add(new Label() { Text = "Operation"   },  1, 0);
            Children.Add(new Label() { Text = "B"           },  2, 0);
            Children.Add(A_List,                                0, 1);
            Children.Add(Operation_List,                        1, 1);
            Children.Add(B_List,                                2, 1);
        }
    }
}
