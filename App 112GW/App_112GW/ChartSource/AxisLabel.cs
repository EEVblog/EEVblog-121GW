using System;
using System.Collections.Generic;
using System.Text;

namespace rMultiplatform
{
    public class AxisLabel
    {
        private string _Label;
        public string Label
        {
            get
            {
                return _Label;
            }
        }
        private string _Units;
        public string Units
        {
            get
            {
                return _Units;
            }
        }
        public string Text
        {
            get
            {
                return _Label + " ( " + _Units + " )";
            }
            set
            {
                var scts = value.Split('(', ')');
                var txt = scts[0];

                if (scts.Length == 1)
                    throw (new Exception("Must contain units in the following format 'Label(units)'."));

                var units = scts[1].Replace(" ", "");

                if (units.Length == 0)
                    throw (new Exception("Must contain units in the following format 'Label(units)'."));

                _Units = units;
                _Label = txt;
            }
        }
        
        public AxisLabel(string Label)
        {
            this.Text = Label;
        }
    };
}