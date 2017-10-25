using System;
using Windows.Security.Cryptography;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using System.Diagnostics;

namespace rMultiplatform.BLE
{
	public class CharacteristicBLE : ICharacteristicBLE
	{
		private GattCharacteristic mCharacteristic;
		public event SetupComplete Ready;
		void TriggerReady()
		{
			Debug.WriteLine("Characteristic ready.");
			Ready?.Invoke();
		}

		public event ChangeEvent ValueChanged;
		void TriggerChange(GattCharacteristic sender, CharacteristicEvent ChangeEvent)
		{
			ValueChanged?.Invoke(sender, ChangeEvent);
		}

		public string Id
		{
			get
			{
				return mCharacteristic.Uuid.ToString();
			}
		}
		public string Description
		{
			get
			{
				return mCharacteristic.UserDescription;
			}
		}

		public bool Send(string pInput)
		{
			var temp = mCharacteristic.WriteValueAsync(CryptographicBuffer.ConvertStringToBinary(pInput, BinaryStringEncoding.Utf8)).AsTask().Result;
			return true;
		}
		public bool Send(byte[] pInput)
		{
			try
			{
				var temp = mCharacteristic.WriteValueAsync(CryptographicBuffer.CreateFromByteArray(pInput)).AsTask().Result;
			}
			catch { }
			return true;
		}

		//Event that is called when the value of the characteristic is changed
		private void CharacteristicEvent_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
		{
			var buffer = args.CharacteristicValue;
			byte[] data;
			CryptographicBuffer.CopyToByteArray(buffer, out data);
			TriggerChange(sender, new CharacteristicEvent(data));
		}
		async void Build()
		{
			int properties = (int)mCharacteristic.CharacteristicProperties;
			int indicate_mask = (int)GattCharacteristicProperties.Indicate;
			if ((properties & indicate_mask) != 0)
			{
				Debug.WriteLine("Setting up Indicate.");
				await mCharacteristic.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.Indicate).AsTask().ContinueWith(
				(obj2) =>
				{
					mCharacteristic.ValueChanged += CharacteristicEvent_ValueChanged;
					TriggerReady();
				});
			}
			else TriggerReady();
		}
		public void Unregister()
		{
			if (mCharacteristic != null)
				mCharacteristic.ValueChanged -= CharacteristicEvent_ValueChanged;
		}

		public CharacteristicBLE(GattCharacteristic pInput, SetupComplete pReady, ChangeEvent pEvent)
		{
			Ready = pReady;
			ValueChanged = pEvent;
			mCharacteristic = pInput;
			Build();
		}
		~CharacteristicBLE()
		{
			Debug.WriteLine("Destructing Characteristic.");
			Unregister();
			Ready = null;
			ValueChanged = null;
			mCharacteristic = null;
		}
	}

}
