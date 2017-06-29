using System;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;

using System.Text;
using System.Threading.Tasks;
using Plugin.BluetoothLE;

namespace rMultiplatform.BLE
{
    public class ServiceBLE : IServiceBLE
    {
        string _Id;
        public event SetupComplete Ready;
        private List<ICharacteristicBLE> mCharacteristics;
        private IDisposable wcd;

        public List<ICharacteristicBLE> Characteristics
        {
            get
            {
                return mCharacteristics;
            }
        }

        public string Id
        {
            get
            {
                return _Id;
            }
        }
        public override string ToString()
        {
            return Id;
        }

        private void Build(IGattService pInput, ChangeEvent pEvent)
        {
            wcd = pInput.WhenCharacteristicDiscovered().Subscribe(obj =>
            {
                mCharacteristics.Add(new CharacteristicBLE(obj, CharateristicReady, pEvent));
            });
        }
        
        private void CharateristicReady()
        {
            Debug.WriteLine("Characteristic finished setting up : " + Id);
            Ready?.Invoke();
        }

        public ServiceBLE(IGattService pInput, SetupComplete ready, ChangeEvent pEvent)
        {
            mCharacteristics = new List<ICharacteristicBLE>();
            _Id = pInput.Uuid.ToString();
            Build(pInput, pEvent);
        }
    }
}