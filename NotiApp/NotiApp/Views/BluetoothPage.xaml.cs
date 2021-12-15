using NotiApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NotiApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BluetoothPage : ContentPage
    {
        public BluetoothPage()
        {
            InitializeComponent();
            this.BindingContext = new BluetoothViewModel();
        }
    }
}