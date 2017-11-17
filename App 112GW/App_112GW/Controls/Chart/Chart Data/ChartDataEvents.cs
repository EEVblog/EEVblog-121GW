using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using System.Reflection;
using System.Resources;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System.Runtime.CompilerServices;
using System;

namespace rMultiplatform
{
    public class ChartAxisEventArgs : EventArgs
    {
        public enum ChartAxisEventType
        {
            DrawMajorTick,
            DrawMinorTick,
            DrawLabel
        };

        public AxisLabel Label;
        public SKCanvas Canvas;
        public SKColor Color;
        public double Position;
        public ChartAxis.AxisOrientation Orientation;
        public ChartAxisEventType EventType;
        public float TickLength;

        public ChartAxisEventArgs(AxisLabel Label, SKCanvas Can, Color Col, double Pos, double TickLen, ChartAxis.AxisOrientation Ori, ChartAxisEventType Typ) : base()
        {
            this.Label = Label;
            TickLength = (float)TickLen;
            Canvas = Can;
            Color = Col.ToSKColor();
            Position = Pos;
            Orientation = Ori;
            EventType = Typ;
        }
        public ChartAxisEventArgs(AxisLabel Label, SKCanvas Can, SKColor Col, double Pos, double TickLen, ChartAxis.AxisOrientation Ori, ChartAxisEventType Typ) : base()
        {
            this.Label = Label;
            TickLength = (float)TickLen;
            Canvas = Can;
            Color = Col;
            Position = Pos;
            Orientation = Ori;
            EventType = Typ;
        }
    }

    public class ChartAxisDrawEventArgs : EventArgs
    {
        public int Index;
        public int MaxIndex;
        public AxisLabel AxisLabel;
        public ChartAxis.AxisOrientation Orientation;
        public float Position;

        public ChartAxisDrawEventArgs(AxisLabel Label, ChartAxis.AxisOrientation Orientation, float Position, int Index, int MaxIndex) : base()
        {
            this.AxisLabel = Label;
            this.Orientation = Orientation;
            this.Position = Position;
            this.Index = Index;
            this.MaxIndex = MaxIndex;
        }
    };
}
