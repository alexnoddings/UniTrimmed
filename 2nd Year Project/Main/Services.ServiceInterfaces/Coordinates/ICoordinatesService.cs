namespace EduLocate.Services.ServiceInterfaces.Coordinates
{
    /// <summary>Converts between northing/easting and calculates the distance between points.</summary>
    public interface ICoordinatesService
    {
        /// <summary>Convert a northing/easting pair to a latitude/longitude pair.</summary>
        /// <param name="northing">The northing part.</param>
        /// <param name="easting">The easting part.</param>
        /// <returns>A tuple of the (latitude, longitude).</returns>
        (double, double) GridToLatitudeLongitude(double northing, double easting);

        /// <summary>Calculates the distance between a school and a latitude/longitude.</summary>
        /// <param name="school">The source school.</param>
        /// <param name="latitude">The target latitude.</param>
        /// <param name="longitude">The target longitude.</param>
        /// <returns>The distance between the two points in kilometres. Can be null if the schools latitude/longitude is invalid/null.</returns>
        double? DistanceBetween(Core.School school, double latitude, double longitude);

        /// <summary>Calculates the distance between one latitude/longitude and another.</summary>
        /// <param name="latitude1">The source latitude.</param>
        /// <param name="longitude1">The source longitude.</param>
        /// <param name="latitude2">The target latitude.</param>
        /// <param name="longitude2">The target longitude.</param>
        /// <returns>The distance between the two points in kilometres.</returns>
        double DistanceBetween(double latitude1, double longitude1, double latitude2, double longitude2);
    }
}