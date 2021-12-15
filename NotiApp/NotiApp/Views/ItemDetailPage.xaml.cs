using NotiApp.ViewModels;
using System.ComponentModel;
using Xamarin.Forms;

namespace NotiApp.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}