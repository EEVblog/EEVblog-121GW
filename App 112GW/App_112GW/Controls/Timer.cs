using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace rMultiplatform
{
	public class CancellableTimer
	{
		private readonly TimeSpan   timespan;
		private readonly Action	 callback;
		private CancellationTokenSource cancel;

		public CancellableTimer(TimeSpan timespan, Action callback)
		{
			this.timespan = timespan;
			this.callback = callback;
			cancel = new CancellationTokenSource();
		}

		bool Running = false;
		public void Start()
		{
			if (!Running)
			{
				Running = true;
				Task.Delay(timespan).ContinueWith((obj) =>
				{
					Running = false;
					Debug.WriteLine("Callback called.");
					Device.BeginInvokeOnMainThread(() => { callback?.Invoke(); });
				}, cancel.Token);
			}
		}
		public void Cancel()
		{
			cancel.Cancel();
			cancel = new CancellationTokenSource();
			Running = false;
		}
	}
}
