using NotiApp.Services;
using NotiApp.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NotiApp
{
    public partial class App : Application
    {
        public readonly CustomBluetoothManager g_CustomBluetoothManager;
        public App()
        {
            InitializeComponent();

            DependencyService.Register<MockDataStore>();
            MainPage = new AppShell();

            g_CustomBluetoothManager = new CustomBluetoothManager(MainPage);
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
