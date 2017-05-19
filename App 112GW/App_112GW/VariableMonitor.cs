using System;
using System.Collections.Generic;
using System.Text;

namespace App_112GW
{
    class VariableMonitor<T>
    {
        private T OldValue;

        private bool _Changed = true;
        public bool Changed
        {
            get
            {
                return _Changed;
            }
        }

        public VariableMonitor(){}
        public bool Update(ref T pValue)
        {
            //Initialise system
            if (OldValue == null)
            {
                OldValue = pValue;
                _Changed = true;
            }
            else
                //Detect change
                _Changed = true;

            return _Changed;
        }
    }
}
