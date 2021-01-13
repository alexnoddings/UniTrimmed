using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using EduLocate.Core;
using EduLocate.Services.ServiceInterfaces.School.Metadata;

namespace EduLocate.Services.MockServices
{
    /// <inheritdoc />
    /// <summary>Provides mock metadata about a school. Values are hard-coded.</summary>
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification =
        "It is instantiated by the Dependency Injection service at run-time.")]
    public class MockMetadataService : ISchoolMetadataService
    {
        private class MockSchoolMetadata : ISchoolMetadata
        {
            public string ImageUri => "https://www.hartlepoolmail.co.uk/webimage/1.8980998.1516903889!/image/image.jpg";
            public double Rating => 4.76;
        }

        /// <inheritdoc />
        public Task<ISchoolMetadata> GetMetadataForSchoolAsync(School school)
        {
            return Task.FromResult<ISchoolMetadata>(new MockSchoolMetadata());
        }

        /// <inheritdoc />
        public string ProviderName => "Mock Services";
    }
}