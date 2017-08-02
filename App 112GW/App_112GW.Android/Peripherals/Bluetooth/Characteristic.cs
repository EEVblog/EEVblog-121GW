using System;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;

using System.Text;
using System.Threading.Tasks;
using Plugin.BLE.Abstractions.Contracts;

namespace rMultiplatform.BLE
{
    public class CharacteristicBLE : ICharacteristicBLE
    {
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
        public event SetupComplete Ready;
        event ChangeEvent _ValueChanged;
        public event ChangeEvent ValueChanged
        {
            add
            {
                _ValueChanged += value;
            }
            remove
            {
                _ValueChanged -= value;
            }
        }
        public bool Send(string pInput)
        {
            byte[] bArray = Encoding.UTF8.GetBytes(pInput);
            return Send(bArray);
        }
        public bool Send(byte[] pInput)
        {
            var temp = mCharacteristic.WriteAsync(pInput).Result;
            return temp;
        }

        //Event that is called when the value of the characteristic is changed
        private void CharacteristicEvent_ValueChanged(object sender, Plugin.BLE.Abstractions.EventArgs.CharacteristicUpdatedEventArgs args)
        {
            Debug.WriteLine("CharateristicEvent : " + args.ToString());

            if (args == null)
                Debug.WriteLine("Args is null.");
            if (args.Characteristic == null)
                Debug.WriteLine("Args.Characteristic is null.");
            if (args.Characteristic.Value == null)
                Debug.WriteLine("Args.Characteristic.Value is null.");
            if (_ValueChanged == null)
                Debug.WriteLine("ValueChanged is null.");

            var buffer = args.Characteristic.Value;
            var charEvent = new CharacteristicEvent(buffer);
            _ValueChanged?.Invoke(sender, charEvent);
        }

        public CharacteristicBLE(ICharacteristic pInput, SetupComplete ready, ChangeEvent pEvent)
        {
            Ready += ready;
            ValueChanged += pEvent;

            Debug.WriteLine("Setting up characteristic");

            mCharacteristic = pInput;
            mCharacteristic.ValueUpdated += CharacteristicEvent_ValueChanged;


            Debug.WriteLine("Characteristic CanRead: " + mCharacteristic.CanRead.ToString());
            Debug.WriteLine("Characteristic CanUpdate: " + mCharacteristic.CanUpdate.ToString());
            Debug.WriteLine("Characteristic CanWrite: " + mCharacteristic.CanWrite.ToString());
            Debug.WriteLine("Characteristic Name: " + mCharacteristic.Name.ToString());

            if (mCharacteristic.CanUpdate)
                mCharacteristic.StartUpdatesAsync().ContinueWith(UpdateStarted);
            else
                Ready?.Invoke();
        }
        private void UpdateStarted(Task obj)
        {
            Debug.WriteLine("Characteristic updates started.");
            Ready?.Invoke();
        }
    }
}