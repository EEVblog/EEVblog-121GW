using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace rMultiplatform.BLE
{
    public class ClientBLE : IClientBLE
    {
        private string _Name;
        public string Name
        {
            private set
            {
                _Name = value;
            }
            get
            {
                return _Name;
            }
        }

        public List<IDeviceBLE> ListDevices()
        {
            return null;
        }
        public List<IServiceBLE> ListServices()
        {
            return null;
        }
        public List<ICharacteristicBLE> ListCharacteristics()
        {
            return null;
        }
        public  Task<IDeviceBLE> Connect(IDeviceBLE pInput)
        {
            return null;
        }
        public bool SendData(string pInput)
        {
            return false;
        }

        public bool Initialise()
        {
            return false;
        }
    }
}
