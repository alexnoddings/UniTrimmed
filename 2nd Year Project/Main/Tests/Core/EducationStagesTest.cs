using System;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using EduLocate.Core;

namespace EduLocate.Tests.Core
{
    [TestFixture]
    internal class EducationStagesTest
    {
        /// <summary>Checks that <see cref="EducationStages"/> is marked with the flags attribute.</summary>
        [Test]
        public void IsMarkedWithFlagsAttribute()
        {
            Assert.True(typeof(EducationStages).GetCustomAttributes<FlagsAttribute>().Any(),
                $"{typeof(EducationStages).Name} is not marked with {typeof(FlagsAttribute).Name}.");
        }
    }
}