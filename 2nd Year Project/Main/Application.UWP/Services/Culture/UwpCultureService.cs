using System.Globalization;
using System.Threading;
using EduLocate.Application.Core.Services.Translation;
using EduLocate.Application.UWP.Services.Culture;
using Xamarin.Forms;

[assembly: Dependency(typeof(UwpCultureService))]

namespace EduLocate.Application.UWP.Services.Culture
{
    /// <inheritdoc />
    public class UwpCultureService : ICultureService
    {
        /// <inheritdoc />
        public CultureInfo CurrentCultureInfo
        {
            get => GetCurrentCultureInfo();
            set => SetCurrentCultureInfo(value);
        }

        private static CultureInfo GetCurrentCultureInfo()
        {
            return Thread.CurrentThread.CurrentCulture;
        }

        private static void SetCurrentCultureInfo(CultureInfo cultureInfo)
        {
            Thread.CurrentThread.CurrentCulture = cultureInfo;
            Thread.CurrentThread.CurrentUICulture = cultureInfo;
        }
    }
}