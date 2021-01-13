using System.Collections.Generic;
using System.Threading.Tasks;

namespace EduLocate.Services.ServiceInterfaces.School
{
    /// <inheritdoc />
    /// <summary>Handles storing schools as well as finding them.</summary>
    public interface ISchoolRepository : ISchoolService
    {
        /// <summary>Gets a list of all schools.</summary>
        /// <returns>All of the schools that are known.</returns>
        /// <remarks>This lives in the repository and not regular service as this functionality is not exposed to clients, only to the backend.</remarks>
        Task<IEnumerable<Core.School>> GetAllSchoolsAsync();

        /// <summary>Updates an existing school based on their non-null values, or saves a non-existing school.</summary>
        /// <param name="school">The school to save.</param>
        Task SelectiveUpdateSchoolAsync(Core.School school);

        /// <summary>Updates existing schools based on their non-null values, or saves non-existing schools.</summary>
        /// <param name="school">The schools to save.</param>
        Task SelectiveUpdateSchoolsAsync(IEnumerable<Core.School> school);
    }
}