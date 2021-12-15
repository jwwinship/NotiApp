using NotiApp.Views;
using NotiApp.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Plugin.BluetoothLE;
using Android.Bluetooth;
using Android.Content.Res;

namespace NotiApp.ViewModels
{
    public class BluetoothViewModel : BaseViewModel
    {
        #region Commands
        public Command Button_BT_TurnOn { get; }
        public Command Button_BT_TurnOff { get; }
        public Command Button_BT_ShowPaired { get; }
        public Command Button_BT_Discover { get; }
        public Command Button_BT_Connect { get; }

        #endregion

        private readonly BluetoothAdapter _manager;
        private readonly CustomBluetoothManager _customBluetoothManager;


        public BluetoothViewModel()
        {
            Button_BT_TurnOn = new Command(OnTurnOnClicked);
            Button_BT_TurnOff = new Command(OnTurnOffClicked);
            Button_BT_ShowPaired = new Command(OnShowPairedClicked);
            Button_BT_Discover = new Command(OnDiscoverClicked);
            Button_BT_Connect = new Command(OnConnectClicked);

            _customBluetoothManager = (Application.Current as App)?.g_CustomBluetoothManager;
            _manager = BluetoothAdapter.DefaultAdapter;
            
            if (_manager != null)
            {
                //Device has no bluetooth capability
                Console.WriteLine("Device does not support bluetooth");           
            }
        }

        private async void OnTurnOnClicked(object obj)
        {
            if (!_manager.IsEnabled) _manager.Enable();
        }

        private async void OnTurnOffClicked(object obj)
        {
            if (_manager.IsEnabled) _manager.Disable();
        }

        private async void OnShowPairedClicked(object obj)
        {
            // Prefixing with `//` switches to a different navigation stack instead of pushing to the active one
            await Shell.Current.GoToAsync($"//{nameof(PairedDevicesPage)}");
            
        }

        private async void OnDiscoverClicked(object obj)
        {
            //_manager.StartDiscovery();
            
        }

        private async void OnConnectClicked(object obj)
        {
            Console.WriteLine("Connect clicked");

        }
    }
}
