using System;
using Moq;
using NUnit.Framework;
using EduLocate.Application.Core.Services.Theme;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EduLocate.Tests.Application.Core.Services.Theme
{
    [TestFixture]
    internal class ThemeExtensionTest
    {
        /// <summary>Ensures that ProvideValue works correctly.</summary>
        [Test]
        public void ProvideValueTest()
        {
            // The colours to use for the test
            Color primaryColour = Color.Red;
            Color secondaryColour = Color.Green;
            Color textColour = Color.Yellow;
            Color backgroundColour = Color.Blue;

            // Create a mock theme service which just returns the colours above
            var themeServiceMock = new Mock<IThemeService>();
            themeServiceMock.Setup(s => s.PrimaryColour).Returns(primaryColour);
            themeServiceMock.Setup(s => s.SecondaryColour).Returns(secondaryColour);
            themeServiceMock.Setup(s => s.TextColour).Returns(textColour);
            themeServiceMock.Setup(s => s.BackgroundColour).Returns(backgroundColour);
            themeServiceMock.Setup(s => s.ColorFor(ThemePart.Primary)).Returns(primaryColour);
            themeServiceMock.Setup(s => s.ColorFor(ThemePart.Secondary)).Returns(secondaryColour);
            themeServiceMock.Setup(s => s.ColorFor(ThemePart.Text)).Returns(textColour);
            themeServiceMock.Setup(s => s.ColorFor(ThemePart.Background)).Returns(backgroundColour);

            // Create the extension using the mocked theme service
            var themeExtension = new ThemeExtension(themeServiceMock.Object);

            // Set the extension to be for the primary part
            themeExtension.Part = ThemePart.Primary;
            // Get the value provided when themeExtension is cast as the generic IMarkupExtension. This just returns an object.
            object extensionPrimaryColourObject = ((IMarkupExtension) themeExtension).ProvideValue(null);
            // Ensure the object is not null
            Assert.NotNull(extensionPrimaryColourObject);
            // Ensure the object is a color
            Assert.IsInstanceOf<Color>(extensionPrimaryColourObject);
            // Ensure the Color is what we expect it to be
            Assert.AreEqual(extensionPrimaryColourObject, primaryColour);
            // Get the value provided when themeExtension is cast as IMarkupExtension<Color>.
            Color extensionPrimaryColour = ((IMarkupExtension<Color>) themeExtension).ProvideValue(null);
            // As Color is a struct, it cannot be null. Only need to check for equality.
            Assert.AreEqual(extensionPrimaryColour, primaryColour);

            // The logic in this block is identical to that of the first one, except for a different Color.
            themeExtension.Part = ThemePart.Secondary;
            object extensionSecondaryColourObject = ((IMarkupExtension) themeExtension).ProvideValue(null);
            Assert.NotNull(extensionSecondaryColourObject);
            Assert.IsInstanceOf<Color>(extensionSecondaryColourObject);
            Assert.AreEqual(extensionSecondaryColourObject, secondaryColour);
            Color extensionSecondaryColour = ((IMarkupExtension<Color>) themeExtension).ProvideValue(null);
            Assert.AreEqual(extensionSecondaryColour, secondaryColour);

            // The logic in this block is identical to that of the first one, except for a different Color.
            themeExtension.Part = ThemePart.Text;
            object extensionTextColourObject = ((IMarkupExtension) themeExtension).ProvideValue(null);
            Assert.NotNull(extensionTextColourObject);
            Assert.IsInstanceOf<Color>(extensionTextColourObject);
            Assert.AreEqual(extensionTextColourObject, textColour);
            Color extensionTextColour = ((IMarkupExtension<Color>) themeExtension).ProvideValue(null);
            Assert.AreEqual(extensionTextColour, textColour);

            // The logic in this block is identical to that of the first one, except for a different Color.
            themeExtension.Part = ThemePart.Background;
            object extensionBackgroundColourObject = ((IMarkupExtension) themeExtension).ProvideValue(null);
            Assert.NotNull(extensionBackgroundColourObject);
            Assert.IsInstanceOf<Color>(extensionBackgroundColourObject);
            Assert.AreEqual(extensionBackgroundColourObject, backgroundColour);
            Color extensionBackgroundColour = ((IMarkupExtension<Color>) themeExtension).ProvideValue(null);
            Assert.AreEqual(extensionBackgroundColour, backgroundColour);

            // Should throw InvalidOperationException when an invalid theme part (-1) is used
            themeExtension.Part = (ThemePart) (-1);
            Assert.Throws<InvalidOperationException>(() => ((IMarkupExtension) themeExtension).ProvideValue(null));
            Assert.Throws<InvalidOperationException>(() => ((IMarkupExtension<Color>)themeExtension).ProvideValue(null));
        }
    }
}