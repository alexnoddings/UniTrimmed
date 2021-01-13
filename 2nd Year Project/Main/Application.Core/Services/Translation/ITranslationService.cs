using System;

namespace EduLocate.Application.Core.Services.Translation
{
    /// <summary>Provides translations.</summary>
    public interface ITranslationService
    {
        /// <summary>Provides a translate string given it's key.</summary>
        /// <param name="key">The key of the translated string.</param>
        /// <returns>The translated string, or an suitable alternative value if the key was not found.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the message to display is null.</exception>
        string Translate(string key);
    }
}