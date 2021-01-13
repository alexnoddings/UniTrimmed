namespace EduLocate.Application.Core.Services.Theme
{
    /// <summary>The part of a theme to access.</summary>
    public enum ThemePart
    {
        /// <inheritdoc cref="IThemeService.PrimaryColour"/>
        Primary,

        /// <inheritdoc cref="IThemeService.SecondaryColour"/>
        Secondary,

        /// <inheritdoc cref="IThemeService.TextColour"/>
        Text,

        /// <inheritdoc cref="IThemeService.BackgroundColour"/>
        Background
    }
}