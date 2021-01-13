using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using EduLocate.Application.Core.Services.Theme;

namespace EduLocate.Tests.Application.Core.Services.Theme
{
    [TestFixture]
    internal class ThemeServicesTest
    {
        private IList<IThemeService> _themeServices;

        [SetUp]
        public void Initialise()
        {
            _themeServices = GenerateThemeServices().ToList();
        }

        /// <summary>Ensures that <see cref="IThemeService.ColorFor"/> works correctly on all theme services being tested. ColorFor should return the same value as the property being requested.</summary>
        [Test]
        public void ColorForTest()
        {
            foreach (IThemeService themeService in _themeServices)
            {
                Assert.AreEqual(themeService.PrimaryColour, themeService.ColorFor(ThemePart.Primary));
                Assert.AreEqual(themeService.SecondaryColour, themeService.ColorFor(ThemePart.Secondary));
                Assert.AreEqual(themeService.TextColour, themeService.ColorFor(ThemePart.Text));
                Assert.AreEqual(themeService.BackgroundColour, themeService.ColorFor(ThemePart.Background));
                // Should throw InvalidOperationException when an invalid theme part (-1) is passed
                Assert.Throws<ArgumentException>(() => themeService.ColorFor((ThemePart)(-1)));
            }
        }

        /// <summary>Generates all theme services that are to be tested.</summary>
        private static IEnumerable<IThemeService> GenerateThemeServices()
        {
            yield return new SimpleThemeService();
        }
    }
}