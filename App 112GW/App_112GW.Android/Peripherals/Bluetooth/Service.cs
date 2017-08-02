using System;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;

using System.Text;
using System.Threading.Tasks;
using Plugin.BLE.Abstractions.Contracts;

namespace rMultiplatform.BLE
{
    public class ServiceBLE : IServiceBLE
    {
        public event SetupComplete Ready;

        volatile private IService mService;
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
                return mService.Id.ToString();
            }
        }
        public override string ToString()
        {
            return Id;
        }

        ChangeEvent mEvent;
        private void Build()
        {
            mService.GetCharacteristicsAsync().ContinueWith((obj) => { AddCharacteristics(obj, mEvent); });
        }
        private void AddCharacteristics(Task<IList<ICharacteristic>> obj, ChangeEvent pEvent)
        {
            try
            {
                UninitialisedServices = obj.Result.Count;
                foreach (var item in obj.Result)
                {
                    Debug.WriteLine("Characteristic adding : " + item.Name);
                    var temp = new CharacteristicBLE(item, CharateristicReady, pEvent);
                    mCharacteristics.Add(temp);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("ERROR: " + e.Message);
            }
        }

        private int UninitialisedServices = 0;
        private void CharateristicReady()
        {
            --UninitialisedServices;
            if (UninitialisedServices == 0)
            {
                Debug.WriteLine("Characteristics finished setting up : " + Id);
                Ready?.Invoke();
            }
        }

        public ServiceBLE(IService pInput, SetupComplete ready, ChangeEvent pEvent)
        {
            mCharacteristics = new List<ICharacteristicBLE>();
            Ready += ready;

            mService = pInput;
            mEvent = pEvent;
            Build();
        }
    }
}