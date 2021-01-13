using System.Globalization;

namespace EduLocate.Application.Core.Services.Translation
{
    /// <summary>Provides information about the current globalisation culture on a device.</summary>
    public interface ICultureService
    {
        /// <summary>The current culture being used.</summary>
        CultureInfo CurrentCultureInfo { get; set; }
    }
}