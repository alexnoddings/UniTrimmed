using System;
using EduLocate.Application.Core.Services.Translation;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EduLocate.Application.Core.I18N
{
    /// <inheritdoc />
    /// <summary>Works as an easy way for markup to access the apps <see cref="ITranslationService"/>.</summary>
    [ContentProperty("TranslationKey")]
    public class TranslateExtension : IMarkupExtension<string>
    {
        /// <summary>The key of the translation.</summary>
        public string TranslationKey { get; set; }

        private readonly ITranslationService _translationService;

        /// <summary>Constructs the extension with the translation service provided by the dependency service.</summary>
        public TranslateExtension()
        {
            _translationService = DependencyService.Get<ITranslationService>();
        }

        /// <summary>Constructs the extension with a provided translation service.</summary>
        public TranslateExtension(ITranslationService translationService)
        {
            _translationService = translationService ?? throw new ArgumentNullException(nameof(translationService));
        }

        string IMarkupExtension<string>.ProvideValue(IServiceProvider serviceProvider)
        {
            if (TranslationKey is null) return string.Empty;

            return _translationService.Translate(TranslationKey) ?? TranslationKey;
        }

        object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
        {
            return (this as IMarkupExtension<string>).ProvideValue(serviceProvider);
        }
    }
}