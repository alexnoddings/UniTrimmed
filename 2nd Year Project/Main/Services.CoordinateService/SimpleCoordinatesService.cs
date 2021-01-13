using System;
using System.Diagnostics.CodeAnalysis;
using EduLocate.Core;
using EduLocate.Services.ServiceInterfaces.Coordinates;

namespace EduLocate.Services.CoordinatesService
{
    /// <inheritdoc />
    /// <remarks>It STRONGLY is advised to read <see href="https://www.ordnancesurvey.co.uk/documents/resources/guide-coordinate-systems-great-britain.pdf">this supporting document (from p49)</see> alongside this code.</remarks>
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Naming follows that of the supporting doccument.")]
    [SuppressMessage("ReSharper", "IdentifierTypo", Justification = "Naming follows that of the supporting doccument.")]
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "It is instantiated by the Dependency Injection service at run-time.")]
    public class SimpleCoordinatesService : ICoordinatesService
    {
        #region Mathematical constants

        // Using National Grid projection, Airy 1830 Ellipsoid
        /// <summary>Uses semi-major axis A.</summary>
        private const double a = 6377563.396;

        /// <summary>Uses semi-major axis B.</summary>
        private const double b = 6356256.909;

        private const double n = (a - b) / (a + b);
        private const double e2 = (a * a - b * b) / (a * a);

        /// <summary>True-Origin northing.</summary>
        private const double N0 = -100000;

        /// <summary>True-Origin easting.</summary>
        private const double E0 = 400000;

        /// <summary>True-Origin latitude.</summary>
        private const double φ0rads = 49 * (Math.PI / 180);

        /// <summary>True-Origin longitude.</summary>
        private const double λ0rads = -2 * (Math.PI / 180);

        /// <summary>Central Meridian scale factor.</summary>
        private const double F0 = 0.9996012717;

        private const double EarthRadiusKm = 6371;

        #endregion

        /// <inheritdoc />
        public (double, double) GridToLatitudeLongitude(double northing, double easting)
        {
            double N = northing;
            double E = easting;

            // initial φ calculation is different from subsequent (iterative function)
            double φcurrent = (n - N0) / (a * F0) + φ0rads;
            double M = CalculateM(φcurrent);

            while (N - N0 - M >= 0.01)
            {
                M = CalculateM(φcurrent);
                φcurrent = CalculateφCurrent(N, M, φcurrent);
            }

            double v = CalculateV(φcurrent);
            double p = CalculateP(φcurrent);
            double n2 = Calculaten2(v, p);

            double VII = Tan(φcurrent) / (2 * p * v);
            double VIII = Tan(φcurrent) / (24 * p * Cub(v)) * (5 + 3 * Tan2(φcurrent) + n2 - 9 * Tan2(φcurrent) * n2);
            double IX = Tan(φcurrent) / (720 * p * Math.Pow(v, 5)) * (61 + 90 * Tan2(φcurrent) + 45 * Tan4(φcurrent));
            double X = Sec(φcurrent) / v;
            double XI = Sec(φcurrent) / (6 * Cub(v)) * (v / p + 2 * Tan2(φcurrent));
            double XII = Sec(φcurrent) / (120 * Math.Pow(v, 5)) * (5 + 28 * Tan2(φcurrent) + 24 * Tan4(φcurrent));
            double XIIA = Sec(φcurrent) / (5040 * Math.Pow(v, 7)) * (61 + 662 * Tan2(φcurrent) + 1320 * Tan4(φcurrent) + 720 * Tan6(φcurrent));

            double EΔ = E - E0;
            double φrads = φcurrent - VII * Sqr(EΔ) + VIII * Math.Pow(EΔ, 4) - IX * Math.Pow(EΔ, 6);
            double λrads = λ0rads + X * EΔ - XI * Cub(EΔ) + XII * Math.Pow(EΔ, 5) - XIIA * Math.Pow(EΔ, 7);

            // Convert rads back to degrees
            double φ = RadsToDegs(φrads);
            double λ = RadsToDegs(λrads);

            return (φ, λ);
        }

        /// <inheritdoc />
        /// <remarks>See <see cref="DistanceBetween(double,double,double,double)"/> for how calculations are done.</remarks>
        public double? DistanceBetween(School school, double latitude, double longitude)
        {
            if (school.Latitude == null || school.Longitude == null)
                return null;
            return DistanceBetween(school.Latitude.Value, school.Longitude.Value, latitude, longitude);
        }

        /// <inheritdoc />
        /// <remarks>The <see href="https://www.movable-type.co.uk/scripts/latlong.html">Haversine formula</see> is used to calculate distances.</remarks>
        public double DistanceBetween(double lat1, double lon1, double lat2, double lon2)
        {
            lat1 = DegsToRads(lat1);
            lon1 = DegsToRads(lon1);
            lat2 = DegsToRads(lat2);
            lon2 = DegsToRads(lon2);
            double deltaLat = Math.Abs(lat1 - lat2);
            double deltaLon = Math.Abs(lon1 - lon2);
            double d = Sqr(Sin(deltaLat / 2)) + Cos(lat1) * Cos(lat2) * Sqr(Sin(deltaLon / 2));
            double c = 2 * Math.Atan2(Math.Sqrt(d), Math.Sqrt(1 - d));
            return EarthRadiusKm * c;
        }

        #region Mathematical calculations

        private static double DegsToRads(double degrees) => degrees * (Math.PI / 180);

        private static double RadsToDegs(double radians)            => radians * (180 / Math.PI);

        private static double CalculateφCurrent(double N, double M, double φcurrent)            => (N - N0 - M) / (a * F0) + φcurrent;

        private static double CalculateV(double φcurrent)            => a * F0 * Math.Pow(1 - e2 * Sin2(φcurrent), -0.5);

        private static double CalculateP(double φcurrent)            => a * F0 * (1 - e2) * Math.Pow(1 - e2 * Sin2(φcurrent), -1.5);

        private static double Calculaten2(double v, double p)            => v / p - 1;

        private static double CalculateM(double φcurrent)
        {
            double top = (1 + n + 5 / 4f * Sqr(n) + 5 / 4f * Cub(n)) * (φcurrent - φ0rads) -
                         (3 * n + 3 * Sqr(n) + 21 / 8f * Cub(n)) * Sin(φcurrent - φ0rads) * Cos(φcurrent + φ0rads);
            double bottom =
                (15 / 8f * Sqr(n) + 15 / 8f * Cub(n)) * Sin(2 * (φcurrent - φ0rads)) * Cos(2 * (φcurrent + φ0rads)) -
                35 / 24f * Cub(n) * Sin(3 * (φcurrent - φ0rads)) * Cos(3 * (φcurrent + φ0rads));
            return b * F0 * (top + bottom);
        }

        #endregion

        #region Mathematical helper methods to clean up code

        private static double Sqr(double x)
            => Math.Pow(x, 2);

        private static double Cub(double x)
            => Math.Pow(x, 3);

        private static double Sin(double a)
            => Math.Sin(a);

        private static double Cos(double a)
            => Math.Cos(a);

        private static double Tan(double a)
            => Math.Tan(a);

        private static double Sin2(double a)
            => Sqr(Math.Sin(a));

        private static double Tan2(double a)
            => Sqr(Math.Tan(a));

        private static double Tan4(double a)
            => Math.Pow(Math.Tan(a), 4);

        private static double Tan6(double a)
            => Math.Pow(Math.Tan(a), 6);

        private static double Sec(double a)
            => Math.Pow(Math.Cos(a), -1);

        #endregion
    }
}