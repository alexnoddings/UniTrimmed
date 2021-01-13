using System;
using System.Diagnostics.CodeAnalysis;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EduLocate.Application.Core.Services.Theme
{
    /// <inheritdoc />
    /// <summary>Works as an easy way for markup to access the apps <see cref="T:EduLocate.Application.Core.Services.Theme.IThemeService" />.</summary>
    [ContentProperty("Part")]
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "It is instantiated when referenced in the markup.")]
    public class ThemeExtension : IMarkupExtension<Color>
    {
        /// <summary>The part of the theme to access.</summary>
        public ThemePart Part { get; set; }

        private readonly IThemeService _themeService;

        /// <summary>Constructs the extension with the theme service provided by the dependency service.</summary>
        public ThemeExtension()
        {
            _themeService = DependencyService.Get<IThemeService>();
        }

        /// <summary>Constructs the extension with a provided theme service.</summary>
        public ThemeExtension(IThemeService themeService)
        {
            _themeService = themeService ?? throw new ArgumentNullException(nameof(themeService));
        }

        /// <inheritdoc />
        /// <summary>Provides a value for the given <see cref="P:EduLocate.Application.Core.Services.Theme.ThemeExtension.Part" />.</summary>
        /// <param name="serviceProvider">Xamarin's service provider.</param>
        /// <returns>The colour to use for the given part.</returns>
        /// <exception cref="InvalidOperationException">Thrown when <see cref="Part"/> is not valid/expected.</exception>
        Color IMarkupExtension<Color>.ProvideValue(IServiceProvider serviceProvider)
        {
            switch (Part)
            {
                case ThemePart.Primary:
                    return _themeService.PrimaryColour;
                case ThemePart.Secondary:
                    return _themeService.SecondaryColour;
                case ThemePart.Text:
                    return _themeService.TextColour;
                case ThemePart.Background:
                    return _themeService.BackgroundColour;
                default:
                    throw new InvalidOperationException($"{nameof(Part)} is not an expected value.");
            }
        }

        /// <inheritdoc />
        /// <summary>Provides a value for the given <see cref="P:EduLocate.Application.Core.Services.Theme.ThemeExtension.Part" />.</summary>
        /// <param name="serviceProvider">Xamarin's service provider.</param>
        /// <returns>The colour to use for the given part.</returns>
        /// <exception cref="InvalidOperationException">Thrown when <see cref="Part"/> is not valid/expected.</exception>
        object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
        {
            return (this as IMarkupExtension<Color>).ProvideValue(serviceProvider);
        }
    }
}