using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace rMultiplatform.BLE
{
    public class ServiceBLE : IServiceBLE
    {
        private GattDeviceService mService;
        public event SetupComplete Ready;
        void TriggerReady()
        {
            Debug.WriteLine("Service ready.");
            Ready?.Invoke();
        }

        private List<ICharacteristicBLE> mCharacteristics;
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
                return mService.Uuid.ToString();
            }
        }
        public override string ToString()
        {
            return Id;
        }
        void Build(ChangeEvent pEvent)
        {
            mCharacteristics = null;
            mCharacteristics = new List<ICharacteristicBLE>();
            mService.GetCharacteristicsAsync().AsTask().ContinueWith((arg)=> 
            {
                Debug.WriteLine("Found Characteristics.");
                CharacteristicsAquired(arg.Result, pEvent);
            });          
        }
        private int Uninitialised = 0;
        private void ItemReady()
        {
            --Uninitialised;
            if (Uninitialised == 0)
                TriggerReady();
        }
        private void CharacteristicsAquired(GattCharacteristicsResult result, ChangeEvent pEvent)
        {
            var characteristics = result.Characteristics;
            Uninitialised = characteristics.Count;
            foreach (var item in result.Characteristics)
                mCharacteristics.Add(new CharacteristicBLE(item, ItemReady, pEvent));
        }
        
        public void Unregister()
        {
            foreach (var characteristic in mCharacteristics)
                characteristic.Unregister();
        }
        public ServiceBLE(GattDeviceService pInput, SetupComplete ready, ChangeEvent pEvent)
        {
            Ready = ready;
            mService = pInput;
            Build(pEvent);
        }
        ~ServiceBLE()
        {
            Unregister();
            mCharacteristics = null;
        }
    }
}
