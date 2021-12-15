using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using NotiApp.ViewModels;

namespace NotiApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EmailPage : ContentPage
    {
        public EmailPage()
        {
            InitializeComponent();
            this.BindingContext = new EmailViewModel();
        }

        private void Switch_Toggled(object sender, ToggledEventArgs e)
        {
            EmailViewModel vm = this.BindingContext as EmailViewModel;
            vm.Handle_SwitchToggled(sender, e);
        }
    }
}