using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Java.Util;
using Android.Bluetooth;

using NotiApp.Views;
using NotiApp.Models;
using NotiApp.Services;


namespace NotiApp.ViewModels
{
    public class PairedDevicesViewModel : BaseViewModel
    {
        public ObservableCollection<Contact> Items { get; set; }

        private readonly CustomBluetoothManager _customBluetoothManager;

        public Command BackButtonPressed { get; }
        public PairedDevicesViewModel()
        {
            BackButtonPressed = new Command(OnBackPressed);

            _customBluetoothManager = (Application.Current as App)?.g_CustomBluetoothManager;
            Items = new ObservableCollection<Contact>();

            foreach (BluetoothDevice device in _customBluetoothManager.GetListBondedDevices())
            {
                Items.Add(new Contact(device.Name, device.Address));
            }
            
        }
        public async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {

            if (e.Item == null)
                return;

            var contact = e.Item as Contact;

            if (contact != null)
            {
                UUID uuid = UUID.FromString("00001101-0000-1000-8000-00805F9B34FB");

                BluetoothDevice device = _customBluetoothManager.GetDevice(contact.Mac);
                _customBluetoothManager.DoCancelDiscovery();

                await _customBluetoothManager.DoConnectionInsecure(device, uuid);
                _customBluetoothManager.DoSendStream("1");
                

                //Deselect Item
                ((ListView)sender).SelectedItem = null;

            }

        }

        private async void OnBackPressed(object obj)
        {
            await Shell.Current.GoToAsync($"//{nameof(BluetoothPage)}");
        }
    }
}
