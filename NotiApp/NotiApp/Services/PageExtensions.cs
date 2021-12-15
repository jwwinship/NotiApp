using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace NotiApp.Services
{
    public static class PageExtensions
    {
        public static Task<bool> DisplayAlertOnUi(this Page source, string title, string message, string accept, string cancel)
        {
            TaskCompletionSource<bool> doneSource = new TaskCompletionSource<bool>();
            Device.BeginInvokeOnMainThread(async () =>
            {
                try
                {
                    var result = await source.DisplayAlert(title, message, accept, cancel);
                    doneSource.SetResult(result);
                }
                catch (Exception ex)
                {
                    doneSource.SetException(ex);
                }
            });

            return doneSource.Task;
        }
    }
}
