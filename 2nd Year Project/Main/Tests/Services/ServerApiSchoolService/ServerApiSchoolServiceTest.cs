using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using EduLocate.Core;
using EduLocate.Services.ServiceInterfaces.School;

namespace EduLocate.Tests.Services.ServerApiSchoolService
{
    [TestFixture]
    internal class ServerApiSchoolServiceTest
    {
        [SetUp]
        public void Initialise()
        {
            _schoolService = new EduLocate.Services.ServerApiSchoolService.ServerApiSchoolService();
        }

        private EduLocate.Services.ServerApiSchoolService.ServerApiSchoolService _schoolService;

        /// <summary>Checks if <see cref="EduLocate.Services.ServerApiSchoolService.ServerApiSchoolService.GetSchoolAsync" /> works correctly.
        /// </summary>
        /// <remarks>
        ///     Unfortunately, this test cannot check too much of what is returned as school information can change easily.
        ///     If this test fails, first ensure that the Server API is in-fact working.
        /// </remarks>
        [Test]
        public async Task GetSchoolAsync()
        {
            const int etonId = 110158;
            School eton = await _schoolService.GetSchoolAsync(etonId);
            Assert.NotNull(eton);
            Assert.AreEqual(eton.Id, etonId);
        }

        /// <summary>Checks if <see cref="EduLocate.Services.ServerApiSchoolService.ServerApiSchoolService.GetSchoolsInRadiusAsync" /> works correctly.</summary>
        /// <remarks>
        ///     Unfortunately, this test cannot check too much of what is returned as school information can change easily.
        ///     It does, however, check that some schools are returned as it is likely that there will always be a school within
        ///     5km of Newcastle.
        ///     If this test fails, first ensure that the Server API is in-fact working.
        /// </remarks>
        [Test]
        public async Task GetSchoolsInRadiusAsync()
        {
            // Lat/long used is around the Urban Sciences Building, Newcastle University
            IEnumerable<School> schoolsInNewcastle =
                await _schoolService.GetSchoolsInRadiusAsync(54.9735594, -1.6251933, 5);
            Assert.True(schoolsInNewcastle.Any());
        }
    }
}