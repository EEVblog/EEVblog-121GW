using System;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;

using System.Text;
using System.Threading.Tasks;
using Plugin.BLE.Abstractions.Contracts;
using rMultiplatform.BLE;

namespace rMultiplatform.BLEs
{
	public class CharacteristicBLE : ICharacteristicBLE
	{
		public event SetupComplete Ready;
		public event ChangeEvent ValueChanged;
		volatile public ICharacteristic mCharacteristic;

		public string Id
		{
			get
			{
				return mCharacteristic.Id.ToString();
			}
		}
		public string Description
		{
			get
			{
				return mCharacteristic.Name;
			}
		}
		public bool Send(string pInput)
		{
			return Send(Encoding.UTF8.GetBytes(pInput));
		}
		public bool Send(byte[] pInput)
		{
			try
			{
				mCharacteristic.WriteAsync(pInput);
				return true;
			}
			catch (Exception e)
			{
				Debug.WriteLine("Failed to Send.");
				Debug.WriteLine(e);
			}
			return false;
		}

		//Event that is called when the value of the characteristic is changed
		private void CharacteristicEvent_ValueChanged(object sender, Plugin.BLE.Abstractions.EventArgs.CharacteristicUpdatedEventArgs args)
		{
			var buffer = args.Characteristic.Value;
			var charEvent = new CharacteristicEvent(buffer);
			ValueChanged?.Invoke(sender, charEvent);
		}
		public void Remake()
		{
			throw new NotImplementedException();
		}

		public void Unregister()
		{
			throw new NotImplementedException();
		}

		public CharacteristicBLE(ICharacteristic pInput, SetupComplete ready, ChangeEvent pEvent)
		{
			Ready						   +=  ready;
			ValueChanged					+=  pEvent;
			mCharacteristic				 =   pInput;
			mCharacteristic.ValueUpdated	+=  CharacteristicEvent_ValueChanged;


			if (mCharacteristic.CanUpdate)
				mCharacteristic.StartUpdatesAsync().ContinueWith((obj) => { Ready?.Invoke(); });
			else
				Ready?.Invoke();
		}
	}
}