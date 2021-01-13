using System;
using Foundation;
using EduLocate.Application.Core.Services.Alert;
using EduLocate.Application.iOS.Services;
using UIKit;

[assembly: Xamarin.Forms.Dependency(typeof(IosAlertService))]

namespace EduLocate.Application.iOS.Services
{
    public class IosAlertService : IAlertService
    {
        private const double QuickMessageSeconds = 2.5;
        private const double LongMessageSeconds = 1;

        public void Display(string message, bool quick)
        {
            Display(message, quick ? QuickMessageSeconds : LongMessageSeconds);
        }

        private static void Display(string message, double seconds)
        {
            UIAlertController alert = UIAlertController.Create(null, message, UIAlertControllerStyle.Alert);

            NSTimer.CreateScheduledTimer(seconds, obj => DismissAlert(alert, obj));

            UIApplication.SharedApplication.KeyWindow.RootViewController.PresentViewController(alert, true, null);
        }

        private static void DismissAlert(UIViewController alert, IDisposable alertDismissTimer)
        {
            alert?.DismissViewController(true, null);
            alertDismissTimer?.Dispose();
        }
    }
}