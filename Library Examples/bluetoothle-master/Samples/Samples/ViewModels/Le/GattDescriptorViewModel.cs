﻿using System;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Plugin.BluetoothLE;
using ReactiveUI.Fody.Helpers;


namespace Samples.ViewModels.Le
{
    public class GattDescriptorViewModel : AbstractViewModel
    {
        readonly IUserDialogs dialogs;


        public GattDescriptorViewModel(IUserDialogs dialogs, IGattDescriptor descriptor)
        {
            this.dialogs = dialogs;
            this.Descriptor = descriptor;
        }


        public IGattDescriptor Descriptor { get; }
        public string Description => this.Descriptor.Value == null ? this.Descriptor.Description : BitConverter.ToString(this.Descriptor.Value);
        public string Uuid => this.Descriptor.Uuid.ToString();
        [Reactive] public DateTime LastValue { get; private set; }
        [Reactive] public bool IsValueAvailable { get; private set; }
        [Reactive] public string Value { get; private set; }


        public void Select()
        {
            this.dialogs.ActionSheet(new ActionSheetConfig()
                .SetTitle($"Description - {this.Description} - {this.Uuid}")
                .SetCancel()
                .Add("Read", async () => await this.Read())
                //.Add("Write", async () => await this.Write())
            );
        }


        async Task Read()
        {
            try
            {
                var result = await this.Descriptor.Read();

                this.LastValue = DateTime.Now;
                this.IsValueAvailable = true;
                this.Value = result.Data == null ? "EMPTY" : Encoding.UTF8.GetString(result.Data, 0, result.Data.Length);
            }
            catch (Exception ex)
            {
                this.dialogs.Alert($"Error Reading {this.Descriptor.Uuid} - {ex}");
            }
        }


        //async Task Write()
        //{
        //    //var value = await this.Descriptor.Write(
        //}
    }
}

