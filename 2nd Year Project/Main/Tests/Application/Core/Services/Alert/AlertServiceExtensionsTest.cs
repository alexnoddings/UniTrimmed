using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Moq;
using NUnit.Framework;
using EduLocate.Application.Core.Services.Alert;
using EduLocate.Application.Core.Services.Translation;

namespace EduLocate.Tests.Application.Core.Services.Alert
{
    /// <remarks><see cref="AlertServiceExtensions.DisplayTranslated(IAlertService, string, bool)"/> is not testable as it relies on the DependencyService.</remarks>
    [TestFixture]
    internal class AlertServiceExtensionsTest
    {
        [Test]
        [SuppressMessage("ReSharper", "InvokeAsExtensionMethod", Justification =
            "The extension is specified explicitly to ensure the test is targeting it correctly.")]
        public void DisplayTranslatedTest()
        {
            // Values to use for translation
            const string translationKey = "ExampleKey";
            const string translatedSuffix = ".translated";
            const string expectedTranslationValue = translationKey + translatedSuffix;

            // Value that would be displayed by the alert service
            string displayedValue = null;

            // Create a mock alert service, which when Display(string, bool) is called, it sets the value of displayedValue to the string it is called with
            var alertServiceMock = new Mock<IAlertService>();
            alertServiceMock.Setup(s => s.Display(It.IsAny<string>(), It.IsAny<bool>()))
                .Callback((string message, bool quick) => displayedValue = message);

            // Create a mock translation service, which when Translate(string) is called, it returns the string with translatedSuffix appended
            var translationServiceMock = new Mock<ITranslationService>();
            translationServiceMock.Setup(s => s.Translate(It.IsAny<string>()))
                .Returns((string key) => key + translatedSuffix);

            // Call for the mock alert service display a string using the mock translation service
            AlertServiceExtensions.DisplayTranslated(alertServiceMock.Object, translationServiceMock.Object,
                translationKey, false);

            // Ensure that the translation service was called properly, and that the value was displayed by the alert service
            Assert.AreEqual(displayedValue, expectedTranslationValue);
        }
    }
}