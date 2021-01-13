using System.Collections.Generic;
using System.Threading.Tasks;

namespace EduLocate.Services.ServiceInterfaces.School
{
    /// <summary>Handles getting schools from a source.</summary>
    public interface ISchoolService
    {
        /// <summary>Find a school from the given ID.</summary>
        /// <param name="id">The school's ID.</param>
        /// <returns>The school.</returns>
        Task<Core.School> GetSchoolAsync(int id);

        /// <summary>Finds all schools in a radius around a point.</summary>
        /// <param name="latitude">The point's latitude.</param>
        /// <param name="longitude">The point's longitude.</param>
        /// <param name="radiusKm">The radius to search in kilometres.</param>
        /// <returns>All of the found schools.</returns>
        Task<IEnumerable<Core.School>> GetSchoolsInRadiusAsync(double latitude, double longitude, double radiusKm);
    }
}