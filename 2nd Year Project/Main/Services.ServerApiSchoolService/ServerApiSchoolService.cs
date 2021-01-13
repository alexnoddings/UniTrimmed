using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using EduLocate.Common;
using EduLocate.Core;
using EduLocate.Services.ServiceInterfaces.School;

namespace EduLocate.Services.ServerApiSchoolService
{
    /// <inheritdoc />
    /// <summary>Handles getting schools from the API provided in the Server.</summary>
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification =
        "It is instantiated by the Dependency Injection service at run-time.")]
    public class ServerApiSchoolService : ISchoolService
    {
        /// <summary>The base uri of the server.</summary>
        private static readonly Uri BaseUri = new Uri("http://home.liamjbell.com:5000/");

        /// <inheritdoc />
        public async Task<School> GetSchoolAsync(int id)
        {
            var uri = new Uri(BaseUri, $"/api/schools/{id}");
            return await SimpleWebRequest.GetResponseFromEndpointAsync<School>(uri, null);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<School>> GetSchoolsInRadiusAsync(double latitude, double longitude,
            double radiusKm)
        {
            var uri = new Uri(BaseUri, "/api/schools/radius");
            IList<School> schools = await SimpleWebRequest.GetResponseFromEndpointAsync<IList<School>>(uri,
                new Dictionary<string, object>
                {
                    {"latitude", latitude},
                    {"longitude", longitude},
                    {"radiusKm", radiusKm}
                });
            // Ignore schools with no name (usually ones with partial/unusable data)
            return schools.Where(s => !string.IsNullOrWhiteSpace(s.Name));
        }
    }
}