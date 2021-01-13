using System;
using System.Collections.Generic;
using System.Globalization;
using Moq;
using NUnit.Framework;
using EduLocate.Application.Core.Services.Translation;

namespace EduLocate.Tests.Application.Core.I18N
{
    [TestFixture]
    internal class TranslationServicesTest
    {
        private const string UnitTestPlatformName = "UnitTests";

        /// <summary>Tests behaviour specific to <see cref="ResourceManagerTranslationService"/>.</summary>
        [Test]
        public void ResourceManagerTranslationServiceTest()
        {

            // An invalid platform name should throw
            Assert.Throws<InvalidOperationException>(() => new ResourceManagerTranslationService("UnsupportedRunTimePlatform45368902439", CultureService));

            // Neither argument should be null-able
            Assert.Throws<ArgumentNullException>(() => new ResourceManagerTranslationService(null, CultureService));
            Assert.Throws<ArgumentNullException>(() => new ResourceManagerTranslationService(UnitTestPlatformName, null));
        }

        /// <summary>Tests the behaviour of <see cref="ITranslationService.Translate"/> for all supported <see cref="ITranslationService"/>s.</summary>
        [Test]
        public void TranslateTest()
        {
            foreach (ITranslationService translationService in GenerateThemeServices())
            {
                // A null key should throw
                Assert.Throws<ArgumentNullException>(() => translationService.Translate(null));

                // A not-found key should just return the key as a missing translation should not crash the app
                const string unlikelyKeyName = "UnlikelyKey239320439230782430";
                Assert.AreEqual(unlikelyKeyName, translationService.Translate(unlikelyKeyName));
            }
        }

        /// <summary>Generates all theme services that are to be tested.</summary>
        private static IEnumerable<ITranslationService> GenerateThemeServices()
        {
            yield return new ResourceManagerTranslationService(UnitTestPlatformName, CultureService);
        }

        /// <summary>Backing variable for <see cref="CultureService"/>. Should not be accessed normally.</summary>
        private static ICultureService _backingCultureService;

        /// <summary>Statically-accessible mock culture service that just returns the current system's culture.</summary>
        private static ICultureService CultureService =>
            _backingCultureService ?? (_backingCultureService = CreateMockCultureService());

        /// <summary>Creates a mock <see cref="ICultureService"/> which just returns the current system's culture.</summary>
        private static ICultureService CreateMockCultureService()
        {
            // Create a mock service which just returns the current culture info of the test runner
            var cultureServiceMock = new Mock<ICultureService>();
            cultureServiceMock.Setup(s => s.CurrentCultureInfo).Returns(CultureInfo.CurrentCulture);
            return cultureServiceMock.Object;
        }
    }
}
