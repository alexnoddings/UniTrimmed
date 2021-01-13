using System.Globalization;
using System.Threading;
using Java.Util;
using EduLocate.Application.Android.Services.Culture;
using EduLocate.Application.Core.Services.Translation;
using Xamarin.Forms;

[assembly: Dependency(typeof(AndroidCultureService))]

namespace EduLocate.Application.Android.Services.Culture
{
    /// <inheritdoc />
    public class AndroidCultureService : ICultureService
    {
        /// <inheritdoc />
        public CultureInfo CurrentCultureInfo
        {
            get => GetCurrentCultureInfo();
            set => SetCurrentCultureInfo(value);
        }

        private static CultureInfo GetCurrentCultureInfo()
        {
            string culture = Locale.Default.ToString().Replace("_", "-");

            try
            {
                return new CultureInfo(culture);
            }
            catch (CultureNotFoundException)
            {
                return CultureInfo.DefaultThreadCurrentCulture;
            }
        }

        private static void SetCurrentCultureInfo(CultureInfo cultureInfo)
        {
            Thread.CurrentThread.CurrentCulture = cultureInfo;
            Thread.CurrentThread.CurrentUICulture = cultureInfo;
        }
    }
}