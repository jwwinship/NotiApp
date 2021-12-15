using System;
using System.Collections.Generic;
using Android.Bluetooth;
using Android.Content.Res;
using Android.OS;
using Java.Util;

using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;



namespace NotiApp.Services
{

    
    interface ICustomBluetoothManager 
    {
        // methods with no return
        void DoTurnOn();
        void DoTurnOff();
        void DoCancelDiscovery();

        void DoSendStream(string data);

        Task DoConnectionInsecure(BluetoothDevice btDevice, UUID uuid);
        BluetoothDevice GetDevice(string macAddress);
        ICollection<BluetoothDevice> GetListBondedDevices();



    }

    public class CustomBluetoothManager : ICustomBluetoothManager
    {
        
        
        // ----------- Serious  Fields   ---------------------------

        private static readonly BluetoothAdapter _btAdapter = BluetoothAdapter.DefaultAdapter;  // IDisposeable
        private static BluetoothSocket _btSocket;                                               // IDisposeable
        private static Stream _outStream;                                                       // IDisposeable


        // ----------  ctor ---------------------------------------

        private static Page _page;
        public CustomBluetoothManager(Page page)
        {
            _page = page;   // To use DisplayAlert 
        }


        //--------------  Public  METHODS   -------------------------------------------------------------------------------------

        public async void DoTurnOn()
        {
            if (!_btAdapter.IsEnabled && _btAdapter != null)
            {
                await _page.DisplayAlert("Action", "Bluetooth Turned On", "OK");
                _btAdapter.Enable();
            }
        }

        public Page GetPage()
        {
            return _page;
        }

        public async void DoTurnOff()
        {
            if ( _btAdapter.IsEnabled || _btSocket.IsConnected )
            {
                await DoCloseDispose(_btAdapter);
                await DoCloseDispose(_btSocket);
                _btAdapter.Disable();
                _btSocket.Close();

                await _page.DisplayAlert("Action", "Socket and Bluetooth turned Off", "OK");
            }

        }

        public void DoCancelDiscovery()
        {
            if (_btAdapter.IsEnabled && _btAdapter.IsDiscovering) _btAdapter?.CancelDiscovery();
        }

        public async void DoSendStream(string data)
        {

           // if (_btSocket != null && _btSocket.IsConnected == true && data != null)
            {
                _btAdapter?.CancelDiscovery();

                try
                {
                    _outStream = _btSocket.OutputStream;
                }
                catch (Exception e1)
                {
                    await _page.DisplayAlert("Exception", $"CustomBluetoothMancager.DoSendStream => e1 => {e1}", "OK");
                    return;
                }


                byte[] msgBuffer = new Java.Lang.String(data).GetBytes();
                byte[] bytes = Encoding.ASCII.GetBytes(data);

                Java.Lang.String javaString = new Java.Lang.String(data);
                byte[] temp = javaString.GetBytes();

                const int offset = 0;

                try
                {
                    
                    _outStream.Write(temp,offset,temp.Length);
                }
                catch (Exception e2)
                {
                    await _page.DisplayAlert("Exception", $"CustomBluetoothMancager.DoSendStream => e2 => {e2}", "OK");
                    return;
                }

            }
        }


        /// <summary>
        /// Default uuid for Bluetoothmodule = <c>00001101-0000-1000-8000-00805F9B34FB</c>
        /// </summary>
        /// <param name="btDevice"></param>
        /// <param name="uuid"></param>
        /// <returns></returns>
        public async Task DoConnectionInsecure(BluetoothDevice btDevice, UUID uuid)
        {
            DoCancelDiscovery();

            if (btDevice == null)
            {
                await _page.DisplayAlert("Error CustomBluetoothManager.DoConnectionInsecure", "btDevice == null", "OK");
                return;
            }
            if (!_btAdapter.IsEnabled)
            {
                await _page.DisplayAlert("State", "Bluetooth is turned off", "OK");
                return;
            }

            if (_btSocket == null) goto GOTOSocketIsNull;       // _btSocket will be set inside this method. // NullreferenceException cause of calling "_btSocket.IsConnected"
            if (_btSocket.IsConnected)
            {
                await _page.DisplayAlert("State", "Device is already connected", "OK");
                return;
            }
            GOTOSocketIsNull:

            try
            {
                _btSocket = btDevice.CreateInsecureRfcommSocketToServiceRecord(uuid);
                _btSocket.Connect();

            }
            catch (Exception e1)
            {
                await _page.DisplayAlert("Exception", $"CustomBluetoothManager.DoConnectionInsecure => e1 => {e1}", "OK");

                try
                {
                    await DoCloseDispose(_btSocket);
                    await DoCloseDispose(btDevice);
                }
                catch (Exception e2)
                {
                    await _page.DisplayAlert("Excdeption", $"CustomBluetoothManager.DoConnectionInsecure => e2 => {e2}", "OK");
                    return;
                }
            }

            if (_btSocket != null && _btSocket.IsConnected) await _page.DisplayAlert("Status", "Device Connected", "OK");
            else if (_btSocket != null && _btSocket.IsConnected) await _page.DisplayAlert("Status", "Device not Connected", "OK");
        }


        public ICollection<BluetoothDevice> GetListBondedDevices() => _btAdapter.BondedDevices;

        public BluetoothDevice GetDevice(string macAddress)
        {

            if (macAddress != null && _btAdapter != null && _btAdapter.IsEnabled)
                return _btAdapter.GetRemoteDevice(macAddress);

           else
            {
                throw new NullReferenceException("CustomBluetoothManager.GetListBondedDevices => Not able to get Device. NullReferenceException");
            }
        }


        // ----------------   Private Methods   -----------------------------------------------------------------------------------------

        private async Task DoCloseDispose(IDisposable aConnectedObject)
        {
            DoCancelDiscovery();

            if (aConnectedObject == null) return;

            try
            {
                aConnectedObject.Dispose();
            }
            catch (Exception e)
            {
                await _page.DisplayAlert("Exception", $"CustomBluetoothManager.DoCloseDispose => {e}","OK");
                throw;
            }
        }
    }
    
}