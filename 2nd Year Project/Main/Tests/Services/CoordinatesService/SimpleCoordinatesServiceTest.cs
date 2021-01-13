using NUnit.Framework;
using EduLocate.Core;
using EduLocate.Services.CoordinatesService;
using EduLocate.Services.ServiceInterfaces.Coordinates;

namespace EduLocate.Tests.Services.CoordinatesService
{
    [TestFixture]
    internal class SimpleCoordinatesServiceTest
    {
        [SetUp]
        public void Initialise()
        {
            _coordinatesService = new SimpleCoordinatesService();
        }

        private SimpleCoordinatesService _coordinatesService;

        /// <summary>Checks if <see cref="SimpleCoordinatesService.DistanceBetween(double, double, double, double)" /> works correctly.</summary>
        [Test]
        public void DistanceBetweenPointAndPoint()
        {
            // Expected value calculated with an online tool
            double distance = _coordinatesService.DistanceBetween(0, 0, 10, 10);
            Assert.True(EduLocate.Common.DoubleHelper.ValuesClose(distance, 1568.5, 0.05));
        }

        /// <summary>Checks if <see cref="SimpleCoordinatesService.DistanceBetween(School, double, double)" /> works correctly.</summary>
        [Test]
        public void DistanceBetweenSchoolAndPoint()
        {
            var nullLocationSchool = new School {Latitude = null, Longitude = null};
            Assert.Null(_coordinatesService.DistanceBetween(nullLocationSchool, 0, 0));

            var zeroLocationSchool = new School {Latitude = 0, Longitude = 0};
            double? zeroLocationDistance = _coordinatesService.DistanceBetween(zeroLocationSchool, 0, 0);
            Assert.NotNull(zeroLocationDistance);
            Assert.True(EduLocate.Common.DoubleHelper.ValuesClose(zeroLocationDistance, 0, 0.05));

            var tenLocationSchool = new School {Latitude = 10, Longitude = 10};
            double? tenLocationDistance = _coordinatesService.DistanceBetween(tenLocationSchool, 0, 0);
            Assert.NotNull(tenLocationDistance);
            // Expected value calculated with an online tool
            Assert.True(EduLocate.Common.DoubleHelper.ValuesClose(tenLocationDistance, 1568.5, 0.05));
        }

        /// <summary>Checks if <see cref="SimpleCoordinatesService.GridToLatitudeLongitude" /> works correctly.</summary>
        [Test]
        public void GridToLatitudeLongitude()
        {
            // Latitude of northing = 0 is ~49.766, longitude of easting = 0 is ~-7.557 (values from https://www.bgs.ac.uk/data/webservices/convertForm.cfm#bngToLatLng)
            (double northingZeroLatitude, double eastingZeroLongitude) =
                _coordinatesService.GridToLatitudeLongitude(0, 0);
            Assert.True(EduLocate.Common.DoubleHelper.ValuesClose(northingZeroLatitude, 49.766, 0.00059));
            Assert.True(EduLocate.Common.DoubleHelper.ValuesClose(eastingZeroLongitude, -7.557, 0.00059));
        }
    }
}