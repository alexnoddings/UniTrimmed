using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Resources;
using EduLocate.Application.Core.Services.Translation;
using Xamarin.Forms;

[assembly: Dependency(typeof(ResourceManagerTranslationService))]

namespace EduLocate.Application.Core.Services.Translation
{
    /// <inheritdoc />
    /// <summary>Provides translation services using an AppResources.resx file.</summary>
    public class ResourceManagerTranslationService : ITranslationService
    {
        /// <summary>Path to the application resource file used for translations.</summary>
        private const string ResourceId = "EduLocate.Application.Core.I18N.AppResources";

        private static readonly Lazy<ResourceManager> ResourceManager = new Lazy<ResourceManager>(() =>
            new ResourceManager(ResourceId, typeof(ResourceManagerTranslationService).Assembly));

        private readonly CultureInfo _cultureInfo;

        /// <inheritdoc />
        /// <summary>Constructs the translation service from the current run-time platform and the <see cref="T:EduLocate.Application.Core.Services.Translation.ICultureService" /> from the dependency service.</summary>
        [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "It is used when the service is instantiated by the dependency service at run-time.")]
        public ResourceManagerTranslationService() : this(Device.RuntimePlatform, DependencyService.Get<ICultureService>())
        {
        }

        /// <inheritdoc />
        /// <summary>Constructs the translation service.</summary>
        /// <param name="runTimePlatform">What platform the code is running on.</param>
        /// <param name="cultureService">The culture service used to determine translations.</param>
        public ResourceManagerTranslationService(string runTimePlatform, ICultureService cultureService)
        {
            if (cultureService == null) throw new ArgumentNullException(nameof(cultureService), @"Culture service must be provided.");

            switch (runTimePlatform)
            {
                case null:
                    throw new ArgumentNullException(nameof(runTimePlatform), @"The run-time platform must be specified.");
                case Device.Android:
                case Device.UWP:
                case "UnitTests":
                {
                    _cultureInfo = cultureService.CurrentCultureInfo;
                    break;
                }
                default:
                    throw new InvalidOperationException($"{nameof(ResourceManagerTranslationService)} does not support translations for platform {runTimePlatform}");
            }
        }

        /// <inheritdoc />
        public string Translate(string key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            return ResourceManager.Value.GetString(key, _cultureInfo) ?? key;
        }
    }
}