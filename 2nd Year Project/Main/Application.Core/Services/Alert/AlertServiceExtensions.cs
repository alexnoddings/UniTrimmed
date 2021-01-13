using EduLocate.Application.Core.Services.Translation;
using Xamarin.Forms;

namespace EduLocate.Application.Core.Services.Alert
{
    /// <summary>Extensions for <see cref="IAlertService"/>.</summary>
    public static class AlertServiceExtensions
    {
        /// <summary>Displayed a message translated using the <see cref="ITranslationService"/> from the <see cref="DependencyService"/>.</summary>
        /// <param name="alertService">The alert service to display the alert using.</param>
        /// <param name="key">The key of the translated string. See <see cref="ITranslationService.Translate"/>.</param>
        /// <param name="quick">If the alert should disappear quickly. <see cref="IAlertService.Display"/>.</param>
        public static void DisplayTranslated(this IAlertService alertService, string key, bool quick)
        {
            DisplayTranslated(alertService, DependencyService.Get<ITranslationService>(), key, quick);
        }

        /// <summary>Displayed a message translated using the <see cref="ITranslationService"/> provided.</summary>
        /// <param name="alertService">The alert service to display the alert using.</param>
        /// <param name="translationService">The translation service to translate the key into a message.</param>
        /// <param name="key">The key of the translated string. See <see cref="ITranslationService.Translate"/>.</param>
        /// <param name="quick">If the alert should disappear quickly. <see cref="IAlertService.Display"/>.</param>
        public static void DisplayTranslated(this IAlertService alertService, ITranslationService translationService, string key, bool quick)
        {
            alertService.Display(translationService.Translate(key), quick);
        }
    }
}
