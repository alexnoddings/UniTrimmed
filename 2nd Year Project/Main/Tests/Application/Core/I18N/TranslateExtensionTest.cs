using Moq;
using NUnit.Framework;
using EduLocate.Application.Core.I18N;
using EduLocate.Application.Core.Services.Theme;
using EduLocate.Application.Core.Services.Translation;
using Xamarin.Forms.Xaml;

namespace EduLocate.Tests.Application.Core.I18N
{
    [TestFixture]
    internal class TranslateExtensionTest
    {
        /// <summary>Ensures that ProvideValue works correctly.</summary>
        [Test]
        public void ProvideValueTest()
        {
            // Values to use for translation
            const string translationKey = "ExampleKey";
            const string translatedSuffix = ".translated";
            const string expectedTranslationValue = translationKey + translatedSuffix;

            // Create a mock translation service, which when Translate(string) is called, it returns the string with translatedSuffix appended
            var translationServiceMock = new Mock<ITranslationService>();
            translationServiceMock.Setup(s => s.Translate(It.IsAny<string>()))
                .Returns((string key) => key + translatedSuffix);

            // Create the extension using the mocked translation service
            var translateExtension = new TranslateExtension(translationServiceMock.Object)
            {
                TranslationKey = translationKey
            };

            // Get the value provided when translateExtension is cast as the generic IMarkupExtension. This just returns an object.
            object extensionTranslationObject = ((IMarkupExtension) translateExtension).ProvideValue(null);
            // Ensure the object is not null
            Assert.NotNull(extensionTranslationObject);
            // Ensure the object is a string
            Assert.IsInstanceOf<string>(extensionTranslationObject);
            // Ensure the string is what we expect it to be
            Assert.AreEqual(extensionTranslationObject, expectedTranslationValue);
            // Get the value provided when translateExtension is cast as IMarkupExtension<string>.
            string extensionTranslation = ((IMarkupExtension<string>) translateExtension).ProvideValue(null);
            // Ensure the string is not null
            Assert.NotNull(extensionTranslation);
            // Ensure the string is what we expect it to be
            Assert.AreEqual(extensionTranslation, expectedTranslationValue);
        }
    }
}