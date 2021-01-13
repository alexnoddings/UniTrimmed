using System;
using System.Collections.Generic;
using NUnit.Framework;
using EduLocate.Common;
using EduLocate.Core;

namespace EduLocate.Tests.Core
{
    [TestFixture]
    internal class SchoolTest
    {
        /// <summary>Generates all possible values for the EducationStages enumerable.</summary>
        private static IEnumerable<EducationStages> GenerateAllEducationStages()
        {
            Type educationStagesUnderlyingType = Enum.GetUnderlyingType(typeof(EducationStages));
            if (educationStagesUnderlyingType != typeof(int))
                Assert.Inconclusive(
                    $"Can only calculate possible enumerations of enums with an underlying type of int. {typeof(EducationStages).Name} has an underlying type of {educationStagesUnderlyingType.Name}.");

            const int highestValue =
                (int) (EducationStages.Primary | EducationStages.Secondary | EducationStages.College);
            for (var i = 0; i <= highestValue; i++) yield return (EducationStages) i;
        }

        /// <summary>Generates a series of strings that indicate a school does not follow a religion.</summary>
        private static IEnumerable<string> GenerateNonReligiousStrings()
        {
            yield return null;
            yield return "Does not apply";
            yield return "None";
        }

        /// <summary>Generates a series of strings that indicate a school follows a religion.</summary>
        private static IEnumerable<string> GenerateReligiousStrings()
        {
            yield return "Church Of England";
            yield return "Catholic";
            yield return "Islam";
            yield return "Moravian";
            yield return "Buddhist";
            yield return "Protestant";
        }

        /// <summary>Checks if <see cref="School.IsReligious" /> works correctly.</summary>
        [Test]
        public void IsReligiousTest()
        {
            foreach (string religiousString in GenerateReligiousStrings())
            {
                var school = new School {Religion = religiousString};
                Assert.True(school.IsReligious);
            }

            foreach (string nonReligiousString in GenerateNonReligiousStrings())
            {
                var school = new School {Religion = nonReligiousString};
                Assert.False(school.IsReligious);
            }
        }

        /// <summary>Checks if <see cref="School.OffersCollegeEducation" /> works correctly.</summary>
        [Test]
        public void OffersCollegeEducationTest()
        {
            foreach (EducationStages educationStages in GenerateAllEducationStages())
            {
                var school = new School {EducationStages = educationStages};
                if ((educationStages & EducationStages.College) == 0)
                    // School should not offer college education if the bit-mask is 0
                    Assert.False(school.OffersCollegeEducation);
                else
                    Assert.True(school.OffersCollegeEducation);
            }
        }

        /// <summary>Checks if <see cref="School.OffersPrimaryEducation" /> works correctly.</summary>
        [Test]
        public void OffersPrimaryEducationTest()
        {
            foreach (EducationStages educationStages in GenerateAllEducationStages())
            {
                var school = new School {EducationStages = educationStages};
                if ((educationStages & EducationStages.Primary) == 0)
                    // School should not offer primary education if the bit-mask is 0
                    Assert.False(school.OffersPrimaryEducation);
                else
                    Assert.True(school.OffersPrimaryEducation);
            }
        }

        /// <summary>Checks if <see cref="School.OffersSecondaryEducation" /> works correctly.</summary>
        [Test]
        public void OffersSecondaryEducationTest()
        {
            foreach (EducationStages educationStages in GenerateAllEducationStages())
            {
                var school = new School {EducationStages = educationStages};
                if ((educationStages & EducationStages.Secondary) == 0)
                    // School should not offer secondary education if the bit-mask is 0
                    Assert.False(school.OffersSecondaryEducation);
                else
                    Assert.True(school.OffersSecondaryEducation);
            }
        }

        /// <summary>Checks if <see cref="School.PercentBoys" /> works correctly.</summary>
        [Test]
        public void PercentBoysTest()
        {
            var nullGirlsSchool = new School {PercentGirls = null};
            Assert.True(nullGirlsSchool.PercentBoys == null);

            var allGirls = new School {PercentGirls = 100};
            Assert.True(DoubleHelper.ValuesClose(allGirls.PercentGirls, 100 - allGirls.PercentBoys, 0.01));

            var allBoys = new School {PercentGirls = 0};
            Assert.True(DoubleHelper.ValuesClose(allBoys.PercentGirls, 100 - allBoys.PercentBoys, 0.01));

            var thirdGirls = new School {PercentGirls = 33.3};
            Assert.True(DoubleHelper.ValuesClose(thirdGirls.PercentGirls, 100 - thirdGirls.PercentBoys, 0.01));
        }
    }
}