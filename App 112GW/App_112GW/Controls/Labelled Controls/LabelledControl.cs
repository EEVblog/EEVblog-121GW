using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System.Runtime.CompilerServices;
using App_112GW;

namespace rMultiplatform
{
    public class LabelledControl : Grid
    {
        Label LabelView;

        private void AddView(View pInput, int pX, int pY, int pXSpan = 1, int pYSpan = 1)
        {
            Children.Add(pInput);
            SetColumn(pInput, pX);
            SetRow(pInput, pY);

            SetColumnSpan(pInput, pXSpan);
            SetRowSpan(pInput, pYSpan);
        }

        private string _Label;
        public string   Label
        {
            set
            {
                _Label = value;
            }
            get
            {
                return _Label;
            }
        }

        public enum eLabelSide
        {
            Left,
            Right
        }
        private     eLabelSide _LabelSide;
        public      eLabelSide  LabelSide
        {
            set
            {
                _LabelSide = value;

                //Setup the sides of the views
                Children.Clear();

                //Setup the views on the correct side
                switch(_LabelSide)
                {
                    case eLabelSide.Left:
                        AddView(LabelView, 0, 0);
                        AddView(ControlView, 1, 0);
                        LabelView.HorizontalOptions = LayoutOptions.EndAndExpand;
                        LabelView.HorizontalTextAlignment = TextAlignment.End;
                        break;
                    case eLabelSide.Right:
                        AddView(LabelView, 1, 0);
                        AddView(ControlView, 0, 0);
                        LabelView.HorizontalOptions = LayoutOptions.StartAndExpand;
                        LabelView.HorizontalTextAlignment = TextAlignment.Start;
                        break;
                }
            }
            get
            {
                return _LabelSide;
            }
        }

        private GeneralControl _ControlView;
        public  GeneralControl  ControlView
        {
            set
            {
                _ControlView = value;
            }
            get
            {
                return _ControlView;
            }
        }

        public SKPaint IdleStyle
        {
            get
            {
                return ControlView.IdleStyle;
            }
            set
            {
                ControlView.IdleStyle = value;
            }
        }
        public SKPaint PressStyle
        {
            get
            {
                return ControlView.PressStyle;
            }
            set
            {
                ControlView.PressStyle = value;
            }
        }
        public SKPaint HoverStyle
        {
            get
            {
                return ControlView.HoverStyle;
            }
            set
            {
                ControlView.HoverStyle = value;
            }
        }
        public new Color BackgroundColor
        {
            get
            {
                return ControlView.BackgroundColor;
            }
            set
            {
                ControlView.BackgroundColor = value;
            }
        }

        public int BorderWidth
        {
            set
            {
                IdleStyle.StrokeWidth = value;
                PressStyle.StrokeWidth = value;
                HoverStyle.StrokeWidth = value;
            }
        }
        public Color IdleColor
        {
            set
            {
                IdleStyle.Color = value.ToSKColor();
            }
        }
        public Color PressColor
        {
            set
            {
                PressStyle.Color = value.ToSKColor();
            }
        }
        public Color HoverColor
        {
            set
            {
                HoverStyle.Color = value.ToSKColor();
            }
        }

        public LabelledControl(GeneralControl Control, string Label)
        {
            _Label = Label;
            ControlView = Control;
            LabelView = new Label()
            {
                Text = Label,
                VerticalOptions = LayoutOptions.Fill,
                VerticalTextAlignment = TextAlignment.Center
            };

            //Setup the default options
            HorizontalOptions = LayoutOptions.Fill;
            VerticalOptions = LayoutOptions.StartAndExpand;
            Padding = 10;

            //Setup grid definitions
            RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });

            //Setup Grid columns
            ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });

            //Setup views
            LabelSide = eLabelSide.Left;
        }
    }
}
