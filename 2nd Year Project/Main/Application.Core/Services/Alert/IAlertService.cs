using System;

namespace EduLocate.Application.Core.Services.Alert
{
    /// <summary>Provides the ability to display alerts to the user on a platform.</summary>
    public interface IAlertService
    {
        /// <summary>Displays a message to the user.</summary>
        /// <param name="message">The localised message to display.</param>
        /// <param name="quick">If the alert should disappear quickly.</param>
        /// <exception cref="ArgumentNullException">Thrown if the message to display is null.</exception>
        void Display(string message, bool quick);
    }
}