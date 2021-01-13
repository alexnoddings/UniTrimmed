using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using EduLocate.Core;
using EduLocate.Services.MockServices;
using EduLocate.Services.ServiceInterfaces.School;

namespace EduLocate.Tests.Services.MockServices
{
    /// <summary>Tests that <see cref="MockSchoolService"/> works correctly.</summary>
    /// <remarks><see cref="MockSchoolService.GetSchoolsInRadiusAsync"/> isn't tested as it would lock down what the service returns.</remarks>
    [TestFixture]
    internal class MockSchoolServiceTest
    {
        private MockSchoolService _schoolService;

        [SetUp]
        public void Initialise()
        {
            _schoolService = new MockSchoolService();
        }

        /// <summary>Checks if <see cref="MockSchoolService.GetSchoolAsync" /> works correctly.</summary>
        /// <remarks>
        ///     Unfortunately, this test cannot be too thorough as it would lock down what can be returned from the school service. It only checks that
        ///     the returned school's id is the same as what was requested.
        /// </remarks>
        [Test]
        public async Task GetSchoolAsyncTest()
        {
            const int schoolId = 100;

            School school = await _schoolService.GetSchoolAsync(schoolId);
            Assert.NotNull(school);
            Assert.AreEqual(school.Id, schoolId);
        }
    }
}