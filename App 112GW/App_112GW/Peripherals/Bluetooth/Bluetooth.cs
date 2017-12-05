﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Collections.ObjectModel;

namespace rMultiplatform.BLE
{
	public delegate void	ChangeEvent(Object o, CharacteristicEvent v);
	public delegate void	DeviceConnected(IDeviceBLE device);

	public class			CharacteristicEvent : EventArgs
	{
		public string NewValue;
		public byte[] Bytes;

		public CharacteristicEvent(byte[] pNewValue)
		{
			Bytes = pNewValue;
			NewValue = Encoding.UTF8.GetString(Bytes);
		}
	}
	public delegate void SetupComplete();
	public interface		IDeviceBLE
	{
		event DeviceSetupComplete Ready;
		event ChangeEvent Change;

		string  Id	  { get; }
		string  Name	{ get; }
		bool	Paired  { get; }
		bool	CanPair { get; }
		void	Remake  ( object o );

		void	Unregister();

		string ToString();
		List<IServiceBLE> Services { get; }
	}
	public delegate void DeviceSetupComplete(IDeviceBLE Device);
	public interface		IServiceBLE
	{
		event SetupComplete Ready;

		string Id { get; }
		string ToString();

		void Unregister();

		List<ICharacteristicBLE> Characteristics { get;}
	}
	public interface		ICharacteristicBLE
	{
		event SetupComplete Ready;

		string Id { get; }
		string Description { get; }

		bool Send( string pInput );
		bool Send( byte[] pInput );

		event ChangeEvent ValueChanged;

		void Unregister();
	}

	public delegate void VoidEvent();
	public delegate void ConnectedEvent(IDeviceBLE pDevice);
	public interface		IClientBLE
	{
		event ConnectedEvent DeviceConnected;

		//Default functions
		void Start();
		void Stop();
		void Rescan();
		void Reset();
		void RemoveAll();

		//Does not return a usable device, it must be paired first
		ObservableCollection<IDeviceBLE> ListDevices();
		void Connect(IDeviceBLE pInput);
	}

	public abstract class AClientBLE
	{
		public volatile ObservableCollection<IDeviceBLE> mVisibleDevices = new ObservableCollection<IDeviceBLE>();
		public volatile ObservableCollection<IDeviceBLE> mConnectedDevices = null;

		private Mutex mut = new Mutex();
		public event ConnectedEvent DeviceConnected;

		public void RemoveAll()
		{
			mVisibleDevices.Clear();
			if (mConnectedDevices != null)
				mConnectedDevices.Clear();
		}

		public void RemoveDevice(string pId)
		{
			foreach (var dev in mVisibleDevices)
				if (dev.Id == pId)
					mVisibleDevices.Remove(dev);
		}
		public void RemoveDevice(IDeviceBLE pInput)
		{
			RemoveDevice(pInput.Id);
		}

		public void TriggerDeviceConnected(IDeviceBLE pInput)
		{
			RunMainThread(() =>
			{
				Debug.WriteLine("Finished connecting to : " + pInput.Id);
				if (mConnectedDevices != null)
					mConnectedDevices.Add(pInput);

				DeviceConnected?.Invoke(pInput);
				RemoveDevice(pInput);
			});

		}

		public void MutexBlock(Action Function, string tag = "")
		{
			void GetMutex(string ltag = "")
			{
				Debug.WriteLine(ltag + " : Waiting");
				mut.WaitOne();
				Debug.WriteLine(ltag + " : Started");
			}
			void ReleaseMutex(string ltag = "")
			{
				Debug.WriteLine(ltag + " : Done");
				mut.ReleaseMutex();
				Debug.WriteLine(ltag + " : Released");
			}
			try
			{
				GetMutex(tag);
				Function();
				ReleaseMutex(tag);
			}
			catch
			{
				ReleaseMutex(tag);
			}
		}

		public bool AddUniqueItem ( IDeviceBLE pInput )
		{
			if (pInput != null)
			{
				try
				{
					bool add = true;
					foreach (var device in mVisibleDevices)
						if (device.Id == pInput.Id)
							add = false;

					if (add)
						if (pInput.Name != null)
							if (pInput.Name.Length != 0)
								mVisibleDevices.Add(pInput);

					return add;
				}
				catch (Exception e)
				{
					Debug.WriteLine("Error Caught : public bool AddUniqueItem(IDeviceBLE pInput)");
					Debug.WriteLine(e);
				}
			}
			return false;
		}
		public ObservableCollection<IDeviceBLE> ListDevices()
		{
			if (mVisibleDevices == null)
				return new ObservableCollection<IDeviceBLE>();
			return mVisibleDevices;
		}
		public void RunMainThread(Action input)
		{
			Device.BeginInvokeOnMainThread(() =>
			{
				try
				{
					input?.Invoke();
				}
				catch
				{
					Debug.WriteLine("Error Caught :  public void RunMainThread(Action input)");
				}
			});
		}
	}
}
