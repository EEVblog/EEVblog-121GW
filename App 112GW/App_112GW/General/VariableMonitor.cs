using System;
using System.Collections.Generic;
using System.Text;

namespace rMultiplatform
{
	class VariableMonitor<T>
	{
		private T OldValue;

		private bool _UpdateOverride;
		public bool UpdateOverride
		{
			set
			{
				_UpdateOverride = value;
			}
			get
			{
				var updatebuf = new bool();
				updatebuf = _UpdateOverride;
				_UpdateOverride = false;
				return updatebuf;
			}
		}

		private bool _Changed = true;
		public event EventHandler OnChanged;
		public bool Changed
		{
			get
			{
				return _Changed;
			}
			private set
			{
				_Changed = value;
				if (_Changed)
					if (OnChanged != null)
						OnChanged(this, EventArgs.Empty);
			}
		}

		public VariableMonitor(){}
		public bool Update(ref T pValue)
		{
			//Initialise system
			if (OldValue == null)
				Changed = true;
			else //Detect change
				Changed = !OldValue.Equals(pValue);

			OldValue = pValue;
			return Changed || UpdateOverride;
		}
	}
}
