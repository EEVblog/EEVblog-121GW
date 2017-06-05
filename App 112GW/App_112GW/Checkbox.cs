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
    class Checkbox : GeneralControl
    {
        private bool _Checked;
        public bool Checked
        {
            get
            {
                return _Checked;
            }
        }

        public event EventHandler Changed
        {
            add
            {
                Clicked += value;
            }
            remove
            {
                Clicked -= value;
            }
        }


        protected virtual void CheckboxClick(object o, EventArgs e)
        {
            _Checked = !_Checked;
            if (_Checked)
                mRenderer.ShowPoints();
            else
                mRenderer.HidePoints();
        }
        public Checkbox(string label) : base(label, new SKPoint[3]{
                new SKPoint((float)(0), (float)(0)),
                new SKPoint((float)(20), (float)(0)),
                new SKPoint((float)20, (float)(10))})
        {
            Clicked += CheckboxClick;
            mRenderer.OffsetAngle = (135);
        }
    }
}
