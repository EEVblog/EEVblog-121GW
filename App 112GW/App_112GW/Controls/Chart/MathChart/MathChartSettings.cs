using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using System.Diagnostics;
using App_112GW;
using System.Collections;
using SkiaSharp;

namespace rMultiplatform
{
    class MathChartSettings : Grid
    {
        private static float SqrA_x_B(float A, float B)
        {
            return (A * A) * B;
        }
        private static float SqrA_Div_B(float A, float B)
        {
            return (A * A) / B;
        }
        private static float A_Div_SqrB(float A, float B)
        {
            return A / (B * B);
        }
        private static float Division(float A, float B)
        {
            return A / B;
        }
        private static float Addition(float A, float B)
        {
            return A + B;
        }
        private static float Subtraction(float A, float B)
        {
            return A - B;
        }
        private static float Multiplication(float A, float B)
        {
            return A * B;
        }
        public delegate float Operation(float A, float B);

        public struct OperationItem
        {
            public string Label { get; set; }
            public Operation Function { get; set; }

            public OperationItem(string pLabel, Operation pFunction)
            {
                Label = pLabel;
                Function = pFunction;
            }
        }
        List<OperationItem> Operations = new List<OperationItem>()
        {
            new OperationItem( "A + B",              Addition        ),
            new OperationItem( "A - B",              Subtraction     ),
            new OperationItem( "A x B",              Multiplication  ),
            new OperationItem( "A / B",              Division        ),
            new OperationItem( "A\u00b2 x B",        SqrA_x_B        ),
            new OperationItem( "A\u00b2 / B",        SqrA_Div_B      ),
            new OperationItem( "A / B\u00b2",        A_Div_SqrB      )
        };

        ListView A_List = new ListView();
        ListView B_List = new ListView();

        Operation Current_Operation = null;
        ListView Operation_List = new ListView();

        public IEnumerable SourceA
        {
            set
            {
                A_List.ItemsSource = value;
            }
            private get
            {
                return A_List.ItemsSource;
            }
        }
        public IEnumerable SourceB
        {
            set
            {
                B_List.ItemsSource = value;
            }
            private get
            {
                return B_List.ItemsSource;
            }
        }

       
        SKPoint Interpolate(SKPoint A, SKPoint B, float X)
        {
            var mx = (B.X - A.X);
            var my = (B.Y - A.Y);
            var bx = A.X;
            var by = A.Y;

            var dx = (X - A.X);
            var rx = dx / mx;

            var valx = mx * rx + bx;
            var valy = my * rx + by;
            return new SKPoint(valx, valy);
        }
        enum Current
        {
            L1,
            L2
        };

        void Resample(List<SKPoint> L1, List<SKPoint> L2)
        {
            Data.Clear();
            var l1_count = L1.Count;
            var l2_count = L2.Count;
            if (Current_Operation != null)
            {
                if ((l1_count > 1) && (l2_count > 1))
                {
                    var i1 = 1;
                    var i2 = 1;
                    List<SKPoint> Output = new List<SKPoint>();
                    while (i1 < l1_count && i2 < l2_count)
                    {
                        var p1 = L1[i1];
                        var p2 = L2[i2];
                        var x1 = p1.X;
                        var x2 = p2.X;

                        float x_val;
                        float y_val1, y_val2;
                        if (x1 > x2)
                        {
                            x_val = x2;
                            y_val2 = p2.Y;

                            //Interpolate a point for L1
                            var P1 = L1[i1 - 1];
                            var P2 = L1[i1];
                            var pt = Interpolate(P1, P2, x2);
                            y_val1 = pt.Y;
                            ++i2;
                        }
                        else
                        {
                            x_val = x1;
                            y_val1 = p1.Y;

                            //Interpolate a point for L2
                            var P1 = L2[i2 - 1];
                            var P2 = L2[i2];
                            var pt = Interpolate(P1, P2, x1);
                            y_val2 = pt.Y;
                            ++i1;
                        }
                        Data.Add(new SKPoint(x_val, Current_Operation(y_val1, y_val2)));
                    }
                }
            }
            ChartData.Set(Data);
        }

        List<SKPoint> Data = new List<SKPoint>();
        Multimeter DeviceA = null, DeviceB = null;
        private void A_List_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                if (DeviceA != null)
                    DeviceA.Plot.DataChanged -= DataA_Changed;
                DeviceA = e.SelectedItem as Multimeter;
                DeviceA.Plot.DataChanged += DataA_Changed;
            }
        }
        private void B_List_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                if (DeviceB != null)
                    DeviceB.Plot.DataChanged -= DataB_Changed;
                DeviceB = e.SelectedItem as Multimeter;
                DeviceB.Plot.DataChanged += DataB_Changed;
            }
        }
        private void DataA_Changed(List<SKPoint> Data)
        {
            Resample(Data, DeviceB.Data.Data);
        }
        private void DataB_Changed(List<SKPoint> Data)
        {
            Resample(DeviceA.Data.Data, Data);
        }

        private void Operation_List_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var sel_item = e.SelectedItem;
            var sel_obj = sel_item as Object;
            Current_Operation = ( (OperationItem) sel_item ).Function;
        }

        public ChartData ChartData;
        public Chart Plot;
        private void AddView(View pInput, int pX, int pY, int pXSpan = 1, int pYSpan = 1)
        {
            Children.Add(pInput);
            SetColumn(pInput, pX);
            SetRow(pInput, pY);

            SetColumnSpan(pInput, pXSpan);
            SetRowSpan(pInput, pYSpan);
        }
        public MathChartSettings ( )
        {
            A_List.ItemSelected += A_List_ItemSelected;
            B_List.ItemSelected += B_List_ItemSelected;
            Operation_List.ItemSelected += Operation_List_ItemSelected;

            Padding = 10;
            HorizontalOptions   = LayoutOptions.Fill;
            VerticalOptions     = LayoutOptions.Fill; 
            BackgroundColor     = Globals.BackgroundColor;

            var operation_template = new DataTemplate(() =>
            {
                var temp = new Label
                {
                    HorizontalOptions = LayoutOptions.Fill,
                    HorizontalTextAlignment = TextAlignment.Center,
                    HeightRequest = 20
                };
                temp.SetBinding(Label.TextProperty, "Label");
                return new ViewCell { View = temp };
            });
            var item_template = new DataTemplate(() =>
            {
                var temp = new Label
                {
                    HorizontalOptions = LayoutOptions.Fill,
                    HorizontalTextAlignment = TextAlignment.Center,
                    HeightRequest = 20
                };
                temp.SetBinding(Label.TextProperty, "ShortId");
                return new ViewCell { View = temp };
            });

            A_List.ItemTemplate         = item_template;
            B_List.ItemTemplate         = item_template;
            Operation_List.ItemTemplate = operation_template;
            Operation_List.ItemsSource  = Operations;

            ChartData = new ChartData(ChartData.ChartDataMode.eRescaling, "Time (s)", "Volts (V)", 10f);
            Plot = new Chart() { Padding = new ChartPadding(0.1f) };
            Plot.AddGrid(new ChartGrid());
            Plot.AddAxis(new ChartAxis(5, 5, 0, 20) { Label = "Time (s)",   Orientation = ChartAxis.Orientation.Horizontal, LockToAxisLabel = "Volts (V)",  LockAlignment = ChartAxis.AxisLock.eEnd, ShowDataKey = false });
            Plot.AddAxis(new ChartAxis(5, 5, 0, 0)  { Label = "Volts (V)",  Orientation = ChartAxis.Orientation.Vertical,   LockToAxisLabel = "Time (s)",   LockAlignment = ChartAxis.AxisLock.eStart });
            Plot.AddData(ChartData);

            RowDefinitions.Add      ( new RowDefinition     { Height    = new GridLength    ( 1, GridUnitType.Auto) } );
            RowDefinitions.Add      ( new RowDefinition     { Height    = new GridLength    ( 1, GridUnitType.Star) } );
            RowDefinitions.Add      ( new RowDefinition     { Height    = new GridLength    ( 1, GridUnitType.Star) } );

            ColumnDefinitions.Add   ( new ColumnDefinition  { Width     = new GridLength    ( 1, GridUnitType.Star) } );
            ColumnDefinitions.Add   ( new ColumnDefinition  { Width     = new GridLength    ( 1, GridUnitType.Star) } );
            ColumnDefinitions.Add   ( new ColumnDefinition  { Width     = new GridLength    ( 1, GridUnitType.Star) } );

            Children.Add(new Label() { Text = "A",          HorizontalOptions = LayoutOptions.Fill,     HorizontalTextAlignment = TextAlignment.Center  },  0, 0);
            Children.Add(new Label() { Text = "Operation",  HorizontalOptions = LayoutOptions.Fill,     HorizontalTextAlignment = TextAlignment.Center  },  1, 0);
            Children.Add(new Label() { Text = "B",          HorizontalOptions = LayoutOptions.Fill,     HorizontalTextAlignment = TextAlignment.Center  },  2, 0);
            Children.Add(A_List,                                0, 1);
            Children.Add(Operation_List,                        1, 1);
            Children.Add(B_List,                                2, 1);

            AddView(Plot, 0, 2, 3, 1);
        }

    }
}
