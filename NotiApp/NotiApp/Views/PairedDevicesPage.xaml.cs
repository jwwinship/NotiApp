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
    public partial class PairedDevicesPage : ContentPage
    {
        
        public PairedDevicesPage()
        {
            InitializeComponent();
            this.BindingContext = new PairedDevicesViewModel();
        }
        private void Handle_ItemTappedView(object sender, ItemTappedEventArgs e)
        {
            PairedDevicesViewModel vm = this.BindingContext as PairedDevicesViewModel;
            vm.Handle_ItemTapped(sender, e);
        }
    }


}