using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using EduLocate.Core;
using EduLocate.Services.ServiceInterfaces.School;

namespace EduLocate.Services.MockServices
{
    /// <inheritdoc />
    /// <summary>Provides sample school data.</summary>
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification =
        "It is instantiated by the Dependency Injection service at run-time.")]
    public class MockSchoolService : ISchoolService
    {
        /// <inheritdoc />
        public Task<School> GetSchoolAsync(int id)
        {
            return Task.FromResult(new School
            {
                Id = id,
                IsOpen = true,
                Name = $"School #{id}",
                Telephone = "07450 000000",
                EducationStages = EducationStages.Primary & EducationStages.Secondary & EducationStages.College,
                Gender = SchoolGender.Mixed,
                Religion = "Does not apply",
                OfstedRating = 1,
                LastOfstedInspection = new DateTime(2019, 05, 11),
                FacebookLink = "https://facebook.com",
                TwitterLink = "https://twitter.com",

                MoneySpentPerStudent = 200,
                NumberOfPupils = 1000,
                PercentGirls = 55.5,

                Latitude = 54.9736034,
                Longitude = -1.6251494,
                Address = "1 Science Square",
                Town = "Newcastle upon Tyne",
                Postcode = "NE4 5TG"
            });
        }

        /// <inheritdoc />
        public Task<IEnumerable<School>> GetSchoolsInRadiusAsync(double latitude, double longitude, double radiusKm)
        {
            return Task.FromResult(GenerateTestSchools());
        }

        private static IEnumerable<School> GenerateTestSchools()
        {
            yield return new School
            {
                Id = 1,
                IsOpen = true,
                Name = "Newcastle School",
                Telephone = "07450 000000",
                EducationStages = EducationStages.Primary | EducationStages.Secondary | EducationStages.College,
                Gender = SchoolGender.Mixed,
                Religion = "Does not apply",
                OfstedRating = 1,
                LastOfstedInspection = new DateTime(2019, 05, 11),
                FacebookLink = "https://facebook.com",
                TwitterLink = "https://twitter.com",

                MoneySpentPerStudent = 200,
                NumberOfPupils = 1000,
                PercentGirls = 55.5,

                Latitude = 54.9736034,
                Longitude = -1.6251494,
                Address = "1 Science Square",
                Town = "Newcastle upon Tyne",
                Postcode = "NE4 5TG"
            };

            yield return new School
            {
                Id = 2,
                IsOpen = true,
                Name = "Dyke House College",
                Telephone = "01429 266377",
                EducationStages = EducationStages.Secondary | EducationStages.College,
                Gender = SchoolGender.Mixed,
                Religion = "Does not apply",
                OfstedRating = 2,
                LastOfstedInspection = new DateTime(2019, 05, 11),
                FacebookLink = "https://www.facebook.com/DykeHouseCollege/",
                TwitterLink = "https://twitter.com/DykeHouse",

                MoneySpentPerStudent = 183,
                NumberOfPupils = 1272,
                PercentGirls = 53.7,

                Latitude = 54.6960862,
                Longitude = -1.2180818,
                Address = "Mapleton Road",
                Town = "Hartlepool",
                Postcode = "TS24 8NQ"
            };
        }
    }
}