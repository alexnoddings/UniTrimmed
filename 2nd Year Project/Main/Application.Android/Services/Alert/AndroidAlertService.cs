using System;
using Android.Widget;
using EduLocate.Application.Android.Services.Alert;
using EduLocate.Application.Core.Services.Alert;

[assembly: Xamarin.Forms.Dependency(typeof(AndroidAlertService))]

namespace EduLocate.Application.Android.Services.Alert
{
    public class AndroidAlertService : IAlertService
    {
        public void Display(string message, bool quick)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            Toast.MakeText(global::Android.App.Application.Context, message,
                quick ? ToastLength.Short : ToastLength.Long).Show();
        }
    }
}