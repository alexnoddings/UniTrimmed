using System.Threading.Tasks;
using NUnit.Framework;
using EduLocate.Core;
using EduLocate.Services.GoogleSchoolMetadataService;
using EduLocate.Services.ServiceInterfaces.School.Metadata;

namespace EduLocate.Tests.Services.GoogleSchoolMetadataService
{
    [TestFixture]
    [Ignore(
        "Running this test every session racks up unnecessary costs on the API. This test should only be enabled when working on " +
        nameof(EduLocate.Services.GoogleSchoolMetadataService.GoogleSchoolMetadataService))]
    internal class GoogleSchoolMetadataServiceTest
    {
        [SetUp]
        public void Initialise()
        {
            _metadataService = new EduLocate.Services.GoogleSchoolMetadataService.GoogleSchoolMetadataService();
        }

        private EduLocate.Services.GoogleSchoolMetadataService.GoogleSchoolMetadataService _metadataService;

        /// <summary>Checks if <see cref="GoogleSchoolMetadataService.GetMetadataForSchoolAsync" /> works correctly.</summary>
        /// <remarks>
        ///     Unfortunately, this test cannot be too accurate as the data received from Google cannot be guaranteed.
        ///     This test failing may not indicate an issue with the <see cref="GoogleSchoolMetadataService" />, but rather with
        ///     Google missing some metadata or the keys no longer being valid.
        /// </remarks>
        [Test]
        public async Task GetMetadataForSchoolAsync()
        {
            // Eton is used as it is a well-known school with a higher likelihood of having metadata available.
            var eton = new School
            {
                Id = 110158,
                IsOpen = true,
                Name = "Eton College",
                Telephone = "01753 370100",
                EducationStages = EducationStages.Secondary | EducationStages.College,
                Gender = SchoolGender.BoysOnly,
                Religion = "Church of England",
                Latitude = 51.492221990269961,
                Longitude = -0.60812356900352926,
                Address = "Eton",
                Town = "Windsor",
                Postcode = "SL4 6DW"
            };

            ISchoolMetadata metadata = await _metadataService.GetMetadataForSchoolAsync(eton);
            if (metadata?.ImageUri == null)
                // Set to inconclusive as a lack of metadata does not definitively prove that the service has failed.
                Assert.Inconclusive(
                    "Some or all of the metadata for provided school was null. This could indicate a failing test, that Google is just missing metadata for this place, or that keys are out of data.");
        }
    }
}