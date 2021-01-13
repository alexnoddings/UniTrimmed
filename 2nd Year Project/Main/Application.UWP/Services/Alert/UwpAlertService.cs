using System;
using Windows.UI.Popups;
using EduLocate.Application.Core.Services.Alert;
using EduLocate.Application.UWP.Services.Alert;
using Xamarin.Forms;

[assembly: Dependency(typeof(UwpAlertService))]

namespace EduLocate.Application.UWP.Services.Alert
{
    public class UwpAlertService : IAlertService
    {
        public async void Display(string message, bool quick)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            await new MessageDialog(message).ShowAsync();
        }
    }
}