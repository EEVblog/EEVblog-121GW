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
        public event SetupComplete          Ready;
        private ChangeEvent                 mEvent;
        volatile private IService           mService;
        private List<ICharacteristicBLE>    mCharacteristics;
        public  List<ICharacteristicBLE>    Characteristics
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

        private void Build()
        {
            mService.GetCharacteristicsAsync().ContinueWith((obj) => { AddCharacteristics(obj); });
        }
        private int UninitialisedServices = 0;
        private void CharateristicReady()
        {
            --UninitialisedServices;
            if (UninitialisedServices == 0)
                Ready?.Invoke();
        }
        private void AddCharacteristics(Task<IList<ICharacteristic>> obj)
        {
            try
            {
                UninitialisedServices = obj.Result.Count;
                foreach (var item in obj.Result)
                {
                    Debug.WriteLine("Characteristic adding : " + item.Name);
                    var temp = new CharacteristicBLE(item, CharateristicReady, mEvent);
                    mCharacteristics.Add(temp);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("ERROR: " + e.Message);
            }
        }

        public ServiceBLE(IService pInput, SetupComplete ready, ChangeEvent pEvent)
        {
            mCharacteristics = new List<ICharacteristicBLE>();
            Ready       +=  ready;
            mService    =   pInput;
            mEvent      =   pEvent;

            Build();
        }
    }
}