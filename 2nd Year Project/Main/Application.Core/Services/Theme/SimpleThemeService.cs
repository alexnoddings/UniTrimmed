using System;
using EduLocate.Application.Core.Services.Theme;
using Xamarin.Forms;

[assembly: Dependency(typeof(SimpleThemeService))]

namespace EduLocate.Application.Core.Services.Theme
{
    /// <inheritdoc />
    /// <summary>Provides a simple, hard-coded theme.</summary>
    public class SimpleThemeService : IThemeService
    {
        // Light Theme
        /// <inheritdoc />
        public Color PrimaryColour { get; } = Color.FromHex("#2196F3");

        /// <inheritdoc />
        public Color SecondaryColour { get; } = Color.FromHex("#FFFFFF");

        /// <inheritdoc />
        public Color TextColour { get; } = Color.FromHex("#111111");

        /// <inheritdoc />
        public Color BackgroundColour { get; } = Color.FromHex("#FFFFFF");

        // Dark Theme
        ///// <inheritdoc />
        //public Color PrimaryColour { get; } = Color.FromHex("#2196F3");

        ///// <inheritdoc />
        //public Color SecondaryColour { get; } = Color.FromHex("#FFFFFF");

        ///// <inheritdoc />
        //public Color TextColour { get; } = Color.FromHex("#D6D6D6");

        ///// <inheritdoc />
        //public Color BackgroundColour { get; } = Color.FromHex("#262626");

        /// <inheritdoc />
        public Color ColorFor(ThemePart part)
        {
            switch (part)
            {
                case ThemePart.Primary:
                    return PrimaryColour;
                case ThemePart.Secondary:
                    return SecondaryColour;
                case ThemePart.Text:
                    return TextColour;
                case ThemePart.Background:
                    return BackgroundColour;
                default:
                    throw new ArgumentException(@"Unexpected theme part", nameof(part));
            }
        }
    }
}