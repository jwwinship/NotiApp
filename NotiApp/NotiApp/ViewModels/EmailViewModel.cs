using System;
using System.Collections.Generic;
using System.Text;
using GemBox.Email;
using GemBox.Email.Imap;
using GemBox.Email.Pop;
using System.Collections.ObjectModel;
using System.Diagnostics;

using System.Threading;

using NotiApp.Models;
using NotiApp.Services;
using Xamarin.Forms;
using System.Threading.Tasks;
using Java.Util;
using Android.Bluetooth;

namespace NotiApp.ViewModels
{
    public class EmailViewModel : BaseViewModel
    {
        private readonly DataModel model;

        public Command Button_Clicked { get; }

        public FreeLimitReachedAction limitAction { get; set; }
        public ImapClient myImap { get; set; }
        public PopClient pop { get; set; }
        public Dictionary<string, ImapMessageInfo> messageList;
        private readonly CustomBluetoothManager _customBluetoothManager;
        public bool silentMode { get; set; }
        public EmailViewModel()
        {
            ComponentInfo.SetLicense("EN-2021Nov28-2021Dec28-dGUplAlAwC5n/NWigS4pUF8eMtzSOOmrzPQ1lWuu8XpNwd8mYvVLw6ZzK1SeI6t/K4touorHkh47fHmozzcNOHFMXhQ==B");
            Button_Clicked = new Command(OnButtonClicked);
            model = new DataModel();
            _customBluetoothManager = (Application.Current as App)?.g_CustomBluetoothManager;

            //Authenticate IMAP connection
            try
            {
                model.Host = "imap-mail.outlook.com";
                model.Username = "thenoti@outlook.com";
                model.Password = "onoti2021k";
                myImap = new ImapClient(model.Host);
                myImap.Connect();
                myImap.Authenticate(model.Username, model.Password);
                myImap.SelectInbox();
                myImap.IdleEnable(1);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
            silentMode = false;
        }
        private async void OnButtonClicked(object obj)
        {
            //myButton.IsEnabled = false;

            //connectIMAP();

            try
            {
                UUID uuid = UUID.FromString("00001101-0000-1000-8000-00805F9B34FB");

                BluetoothDevice device = _customBluetoothManager.GetDevice("00:21:06:BE:92:BB");
                _customBluetoothManager.DoCancelDiscovery();

                await _customBluetoothManager.DoConnectionInsecure(device, uuid);



                //await DisplayAlert("Idle Status", "\nIDLE Mode is " + (myImap.IdleEnabled ? "enabled" : "disabled"), "Close");

                await PageExtensions.DisplayAlertOnUi(_customBluetoothManager.GetPage(), "Error", "Is connected: " + myImap.IsConnected, "Accept", "Close");
                await PageExtensions.DisplayAlertOnUi(_customBluetoothManager.GetPage(), "Error", "Is authenticated: " + myImap.IsAuthenticated, "Accept", "Close");
                //Debug.WriteLine("Is connected: " + myImap.IsConnected);
                //Debug.WriteLine("Is Authenticated: " + myImap.IsAuthenticated);


                Device.StartTimer(TimeSpan.FromSeconds(30), () =>
                {
                    Task.Run(async () =>
                    {
                        pollForMessages();

                    });
                    return true;
                });
            }

            catch (Exception ex)
            {
                if (ex.GetType() == typeof(FreeLimitReachedException))
                {
                    limitAction = FreeLimitReachedAction.Stop;
                }
                else
                {
                    //await DisplayAlert("Error", ex.Message, "Close");
                    Debug.WriteLine(ex.StackTrace);
                    await PageExtensions.DisplayAlertOnUi(_customBluetoothManager.GetPage(), "Error", ex.Message, "Accept", "Close");
                    await PageExtensions.DisplayAlertOnUi(_customBluetoothManager.GetPage(), "Error", ex.StackTrace, "Accept", "Close");
                }
            }
        }

        public async void Handle_SwitchToggled(object sender, ToggledEventArgs e)
        {
            silentMode = !silentMode;
        }
        private async void pollForMessages()
        {
            try
            {
                String dataToSend = silentMode ? "2" : "1";
                messageList = messageList ?? new Dictionary<string, ImapMessageInfo>(); //Initialize if null


                Console.WriteLine("Count: " + messageList.Count);

                if (myImap.SelectedFolder.Count < messageList.Count)

                {

                    var newMessages = GetMessages();

                    foreach (var key in messageList.Keys)

                        if (!newMessages.ContainsKey(key))
                        {
                            //await DisplayAlert("Deletion Alert", "Message '" + key + "' deleted.", "Close");
                            await PageExtensions.DisplayAlertOnUi(_customBluetoothManager.GetPage(), "Deletion Alert", "Message '" + key + "' deleted.", "Accept", "Close");
                            _customBluetoothManager.DoSendStream(dataToSend);
                        }

                    messageList = newMessages;
                }

                else if (myImap.SelectedFolder.Count > messageList.Count)
                {
                    //bool criticalEmailFound = true; //Default to true

                    var newMessages = GetMessages();
                    //_customBluetoothManager.DoSendStream(dataToSend);
                    foreach (var message in newMessages)
                    {

                        if (!message.Value.Flags.Contains(ImapMessageFlags.Seen)/* && !messageList.ContainsKey(message.Key)*/)
                        {
                            _customBluetoothManager.DoSendStream(dataToSend);
                            await PageExtensions.DisplayAlertOnUi(_customBluetoothManager.GetPage(), "Message alert", "Message received. \nSubject: " + myImap.PeekMessage(message.Key).Subject, "Accept", "Close");
                        }
                        //criticalEmailFound = checkKeywordsInBody(message, new string[]{"unsubscribe", "giveaway"}); //Checks for spam keywords
                        //if (criticalEmailFound)
                        //{
                            
                        //}
                        
                    }

                    messageList = newMessages;
                }

                System.Diagnostics.Debug.WriteLine("Polling Loop Completed");
                await PageExtensions.DisplayAlertOnUi(_customBluetoothManager.GetPage(), "TestAlert", "Polling Loop Completed", "Accept", "Close");

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine(ex.InnerException);
            }

            //}
        }
        public Dictionary<string, ImapMessageInfo> GetMessages()
        {
            var items = new Dictionary<string, ImapMessageInfo>();
            foreach (ImapMessageInfo info in myImap.ListMessages())
                items.Add(info.Uid, info);
            return items;
        }

        //public bool checkKeywordsInBody(KeyValuePair<string, ImapMessageInfo> messageInfo, string[] keywords)
        //{
        //    bool criticalEmailFound = true;
        //    MailMessage message = myImap.GetMessag e(messageInfo.Key);//Turn message info into message
        //    foreach (string keyword in keywords)
        //    {
        //        if (message.BodyText.Contains(keyword))
        //        {
        //            criticalEmailFound = false; //message is not important
        //        }
        //    }
        //    return criticalEmailFound;
        //}
    }
}
