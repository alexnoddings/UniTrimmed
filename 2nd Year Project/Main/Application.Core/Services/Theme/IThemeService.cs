using System;
using Xamarin.Forms;

namespace EduLocate.Application.Core.Services.Theme
{
    /// <summary>Provides colours for an application theme.</summary>
    public interface IThemeService
    {
        /// <summary>The primary/accent colour.</summary>
        Color PrimaryColour { get; }

        /// <summary>The secondary colour to be used over the primary one.</summary>
        Color SecondaryColour { get; }

        /// <summary>The colour to be used to regular text.</summary>
        Color TextColour { get; }

        /// <summary>The colour to be used for backgrounds.</summary>
        Color BackgroundColour { get; }

        /// <summary>Provides a colour given a theme part.</summary>
        /// <param name="part">The part of the theme to provide the colour for.</param>
        /// <returns>The colour of the given theme part.</returns>
        /// <exception cref="ArgumentException">Thrown when an invalid/unexpected <see cref="ThemePart"/> is passed.</exception>
        Color ColorFor(ThemePart part);
    }
}