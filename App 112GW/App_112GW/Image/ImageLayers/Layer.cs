using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using SkiaSharp;
using SkiaSharp.Views.Forms;

namespace App_112GW
{
    public interface ILayer
    {
        event EventHandler      OnChanged;
        void                    Set(bool pState);
        void                    Off();
        void                    On();
                

        string                  ToString();
        string                  Name
        {
            get;
            set;
        }
        void                    Render(ref SKCanvas pSurface);


        int Width
        {
            get;
        }
        int Height
        {
            get;
        }
    }

    public class LayerCompare : Comparer<App_112GW.ILayer>
    {
        // Compares by Length, Height, and Width.
        public override int Compare(App_112GW.ILayer x, App_112GW.ILayer y)
        {
            return x.Name.CompareTo(y.Name);
        }
    }
}
