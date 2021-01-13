using System.Threading.Tasks;

namespace EduLocate.Services.ServiceInterfaces.School.Metadata
{
    /// <summary>Provides metadata about a given school.</summary>
    public interface ISchoolMetadataService
    {
        /// <summary>Get the metadata for a school.</summary>
        /// <param name="school">The school.</param>
        /// <returns>The found metadata. May be incomplete.</returns>
        Task<ISchoolMetadata> GetMetadataForSchoolAsync(Core.School school);

        /// <summary>The name of the service providing this metadata.</summary>
        string ProviderName { get; }
    }
}