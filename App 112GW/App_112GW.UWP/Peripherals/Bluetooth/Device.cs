using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;

namespace rMultiplatform.BLE
{
	public class UnPairedDeviceBLE : IDeviceBLE
	{
		public volatile DeviceInformation Information;
		public event DeviceSetupComplete Ready;
		public event ChangeEvent Change;

		public string Id
		{
			get
			{
				return Information.Id;
			}
		}
		public string Name
		{
			get
			{
				return Information.Name;
			}
		}
		public bool Paired
		{
			get
			{
				return Information.Pairing.IsPaired;
			}
		}
		public bool CanPair
		{
			get
			{
				return Information.Pairing.CanPair;
			}
		}
		public UnPairedDeviceBLE(DeviceInformation pInput) { Information = pInput; }
		public override string ToString()
		{
			return Name + "\n" + Id;
		}

		public void Remake(object o)
		{
			throw new NotImplementedException();
		}

		public void Unregister()
		{
			throw new NotImplementedException();
		}

		public List<IServiceBLE> Services
		{
			get
			{
				return null;
			}
		}
	}
	public class PairedDeviceBLE : IDeviceBLE
	{
		private BluetoothLEDevice mDevice;
		private List<IServiceBLE> mServices;
		public event DeviceSetupComplete Ready;
		public event ChangeEvent Change;
		void TriggerReady()
		{
			mDevice.ConnectionStatusChanged += MDevice_ConnectionStatusChanged;
			Ready?.Invoke(this);
			Ready = null;
		}
		private void InvokeChange(object o, CharacteristicEvent v)
		{
			Change?.Invoke(o, v);
		}

		public string Id
		{
			get
			{
				return mDevice.DeviceId;
			}
		}
		public string Name
		{
			get
			{
				return mDevice.Name;
			}
		}
		public bool Paired
		{
			get
			{
				return mDevice.DeviceInformation.Pairing.IsPaired;
			}
		}
		public bool CanPair
		{
			get
			{
				return mDevice.DeviceInformation.Pairing.CanPair;
			}
		}
		public List<IServiceBLE> Services
		{
			get
			{
				return mServices;
			}
		}

		private int Uninitialised = 0;
		private void ItemReady()
		{
			--Uninitialised;
			Debug.WriteLine("Service count remaining = " + Uninitialised.ToString());
			if (Uninitialised == 0)
				TriggerReady();
		}
		private async void Build()
		{
			await mDevice.GetGattServicesAsync().AsTask().ContinueWith((obj) =>
			{
				Debug.WriteLine("Found Services.");
				ServicesAquired(obj.Result);
			});
		}
		private void ServicesAquired(GattDeviceServicesResult result)
		{
			Debug.WriteLine("Services Aquired.");
			var services = result.Services;
			Uninitialised = services.Count;
			foreach (var service in services)
				mServices.Add(new ServiceBLE(service, ItemReady, InvokeChange));
		}
		public override string ToString()
		{
			return Name + "\n" + Id;
		}
		public void Remake(object o)
		{
			Deregister();
			Build();
		}

		void Deregister()
		{
			if (mDevice != null)
			{
				mDevice.ConnectionStatusChanged -= MDevice_ConnectionStatusChanged;

				if (mServices != null)
					foreach (var service in mServices)
						service.Unregister();

				mServices = null;
				mServices = new List<IServiceBLE>();
			}
		}

		async void MDevice_ConnectionStatusChanged(BluetoothLEDevice sender, object args)
		{
			mDevice = sender;
			Deregister();
			if (sender.ConnectionStatus == BluetoothConnectionStatus.Disconnected)
			{
				Debug.WriteLine("Reconnecting...");
				await BluetoothLEDevice.FromIdAsync(sender.DeviceId).AsTask().ContinueWith((obj) => { ConnectionComplete(obj.Result); });
			}
			else
				ConnectionComplete(sender);
			Debug.WriteLine("MDevice_ConnectionStatusChanged end.");
		}
		void ConnectionComplete(BluetoothLEDevice result)
		{
			Debug.WriteLine("Connection complete start.");
			mDevice = result;
			Remake(result);
			Debug.WriteLine("Connection complete end.");
		}

		public void Unregister()
		{
			Deregister();
		}

		public PairedDeviceBLE(BluetoothLEDevice pInput, DeviceSetupComplete pReady)
		{
			mServices = new List<IServiceBLE>();
			Ready = pReady;
			mDevice = pInput;
			Build();
		}
		~PairedDeviceBLE()
		{
			Debug.WriteLine("Deregistering Service.");
			mDevice.Dispose();
			mDevice = null;
			Deregister();
			mServices = null;
		}
	}
}
