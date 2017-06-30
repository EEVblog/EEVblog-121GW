using System;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;

using System.Text;
using System.Threading.Tasks;
using Plugin.BluetoothLE;

namespace rMultiplatform.BLE
{
    public class CharacteristicBLE : ICharacteristicBLE
    {
        string _Description, _Id;
        private IDisposable snv;
        private IDisposable wnr;
        private IDisposable wr;
        private IDisposable ww;

        public string Id
        {
            get
            {
                return _Id;
            }
        }
        public string Description
        {
            get
            {
                return _Description;
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
            return false;
        }

        public CharacteristicBLE(IGattCharacteristic pInput, SetupComplete ready, ChangeEvent pEvent)
        {
            _ValueChanged += pEvent;
            /////////////////////////////////////////////////////////////////////
            Ready += ready;
            _Id = pInput.Uuid.ToString();
            _Description = pInput.Description;

            Debug.WriteLine("Setting up characteristic : " + _Id + " " + _Description);
            /////////////////////////////////////////////////////////////////////
            if (pInput.CanNotify())
            {
                wnr = pInput.RegisterAndNotify(CharacteristicConfigDescriptorValue.Indicate).Subscribe(obj =>
                {
                    Debug.WriteLine("Charateristic Indicate Event From : " + pInput.Service.Device.Uuid.ToString());
                    var buffer = obj.Data;
                    var charEvent = new CharacteristicEvent(buffer);
                    _ValueChanged?.Invoke(obj, charEvent);
                });
            }

            ///////////////////////////////////////////////////////////////////////
            //if (pInput.CanRead())
            //{
            //    Debug.WriteLine("Setting up read.");
            //    pInput.ReadInterval(new TimeSpan(0, 0, 0, 0, 500));
            //    wr = pInput.WhenRead().Subscribe(obj =>
            //    {
            //        Debug.WriteLine("Charateristic Read Event : " + _Id);
            //        byte[] buffer = obj.Data;
            //        var charEvent = new CharacteristicEvent(buffer);
            //        _ValueChanged?.Invoke(obj, charEvent);
            //    });
            //}

            ///////////////////////////////////////////////////////////////////////
            //if (pInput.CanWrite())
            //{
            //    Debug.WriteLine("Setting up write.");
            //    ww = pInput.WhenWritten().Subscribe(obj =>
            //    {
            //        Debug.WriteLine("Charateristic Write Event : " + _Id);
            //        byte[] buffer = obj.Data;
            //        var charEvent = new CharacteristicEvent(buffer);
            //        _ValueChanged?.Invoke(obj, charEvent);
            //    });
            //}

            Ready?.Invoke();
        }
        private void UpdateStarted(Task obj)
        {
            Debug.WriteLine("Characteristic updates started.");
            Ready?.Invoke();
        }
    }
}