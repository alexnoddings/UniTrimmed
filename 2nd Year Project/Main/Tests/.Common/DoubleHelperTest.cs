using NUnit.Framework;
using EduLocate.Common;

namespace EduLocate.Tests.Common
{
    [TestFixture]
    internal class DoubleHelperTest
    {
        [Test]
        public void ValuesCloseTest()
        {
            // Should pass for both null (they are the same)
            Assert.True(DoubleHelper.ValuesClose(null, null, 10));
            Assert.True(DoubleHelper.ValuesClose(null, null, 0));

            // Should always fail for only one null
            Assert.False(DoubleHelper.ValuesClose(0, null, 0));
            Assert.False(DoubleHelper.ValuesClose(0, null, int.MaxValue));
            Assert.False(DoubleHelper.ValuesClose(null, 0, 0));
            Assert.False(DoubleHelper.ValuesClose(null, 0, int.MaxValue));

            Assert.True(DoubleHelper.ValuesClose(0, 0, 0));
            Assert.True(DoubleHelper.ValuesClose(0, 1, 1));
            Assert.True(DoubleHelper.ValuesClose(-1, 1, 2));
            Assert.True(DoubleHelper.ValuesClose(-0.5, 0.5, 1));

            Assert.False(DoubleHelper.ValuesClose(0, 0.001, 0));
            Assert.False(DoubleHelper.ValuesClose(0, 1, 0.99999999));
            Assert.False(DoubleHelper.ValuesClose(-1, 1, 1.99999999));
            Assert.False(DoubleHelper.ValuesClose(-0.5, 0.5, 0.99999999));
        }
    }
}