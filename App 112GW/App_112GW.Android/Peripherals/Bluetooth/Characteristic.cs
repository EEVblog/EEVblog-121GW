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

        public event SetupComplete  Ready;
        public event ChangeEvent    ValueChanged;

        public bool Send(string pInput)
        {
            return Send(Encoding.UTF8.GetBytes(pInput));
        }
        public bool Send(byte[] pInput)
        {
            return mCharacteristic.WriteAsync(pInput).Result;
        }

        //Event that is called when the value of the characteristic is changed
        private void CharacteristicEvent_ValueChanged(object sender, Plugin.BLE.Abstractions.EventArgs.CharacteristicUpdatedEventArgs args)
        {
            if (args == null)
                Debug.WriteLine("Args is null.");
            if (args.Characteristic == null)
                Debug.WriteLine("Args.Characteristic is null.");
            if (args.Characteristic.Value == null)
                Debug.WriteLine("Args.Characteristic.Value is null.");
            if (ValueChanged == null)
                Debug.WriteLine("ValueChanged is null.");

            var buffer = args.Characteristic.Value;
            var charEvent = new CharacteristicEvent(buffer);
            ValueChanged?.Invoke(sender, charEvent);
        }

        public CharacteristicBLE(ICharacteristic pInput, SetupComplete ready, ChangeEvent pEvent)
        {
            Ready += ready;
            ValueChanged += pEvent;
            mCharacteristic = pInput;
            mCharacteristic.ValueUpdated += CharacteristicEvent_ValueChanged;

            Debug.WriteLine("Setting up characteristic");
            Debug.WriteLine("Characteristic CanRead:\t" + mCharacteristic.CanRead.ToString());
            Debug.WriteLine("Characteristic CanUpdate:\t" + mCharacteristic.CanUpdate.ToString());
            Debug.WriteLine("Characteristic CanWrite:\t" + mCharacteristic.CanWrite.ToString());
            Debug.WriteLine("Characteristic Name:\t\t" + mCharacteristic.Name.ToString());

            if (mCharacteristic.CanUpdate)
                mCharacteristic.StartUpdatesAsync().ContinueWith((obj) => { Ready?.Invoke(); });
            else
                Ready?.Invoke();
        }
    }
}