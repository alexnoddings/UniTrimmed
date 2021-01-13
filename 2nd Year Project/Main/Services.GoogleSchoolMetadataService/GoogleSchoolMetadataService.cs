using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using NLog;
using EduLocate.Common;
using EduLocate.Core;
using EduLocate.Services.ServiceInterfaces.School.Metadata;

namespace EduLocate.Services.GoogleSchoolMetadataService
{
    /// <inheritdoc />
    /// <summary>Provides metadata for schools from Google's Places API.</summary>
    /// <remarks>See <see href="https://developers.google.com/places/web-service/intro">Google's Places API</see> documentation.</remarks>
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification =
        "It is instantiated by the Dependency Injection service at run-time.")]
    public class GoogleSchoolMetadataService : ISchoolMetadataService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static string ApiKey => Keys.GoogleApi;
        private const string PlaceSearchEndPoint = "https://maps.googleapis.com/maps/api/place/findplacefromtext/json";
        private const string PhotoSearchEndPoint = "https://maps.googleapis.com/maps/api/place/photo";

        /// <inheritdoc />
        public async Task<ISchoolMetadata> GetMetadataForSchoolAsync(School school)
        {
            Candidate schoolCandidate = await GetCandidateForSchoolAsync(school);

            if (schoolCandidate == null) return new SimpleSchoolMetadata(string.Empty, 0);

            string photoReference = schoolCandidate.Photos?.First()?.Photo_Reference;
            if (photoReference == null) return new SimpleSchoolMetadata(string.Empty, schoolCandidate.Rating);

            string imageUri = await GetImageUriFromReferenceAsync(photoReference) ?? string.Empty;
            double rating = schoolCandidate.Rating;

            return new SimpleSchoolMetadata(imageUri, rating);
        }

        /// <inheritdoc />
        public string ProviderName => "Google";

        /// <summary>Finds the Places APIs first Candidate for a school, or null if none were found.</summary>
        /// <param name="school">The school.</param>
        /// <returns>The first Candidate found, or null if none were found.</returns>
        private static async Task<Candidate> GetCandidateForSchoolAsync(School school)
        {
            Response response = await SimpleWebRequest.GetResponseFromEndpointAsync<Response>(PlaceSearchEndPoint,
                new Dictionary<string, object>
                {
                    {"input", school.Name ?? "School"},
                    {"inputtype", "textquery"},
                    {"fields", "photos,rating"},
                    {"locationbias", $"circle:20@{school.Latitude},{school.Longitude}"},
                    {"key", ApiKey}
                });

            Logger.Trace("{0} response: {1}", PlaceSearchEndPoint, response.Status);

            return response.Candidates?.First();
        }

        /// <summary>Finds an image uri given its reference, or <code>string.Empty</code> if the reference was not valid.</summary>
        /// <param name="photoReference">The reference to the photo.</param>
        /// <returns>A uri pointing to the photo resource, or <code>string.Empty</code> if the reference was not valid.</returns>
        private static async Task<string> GetImageUriFromReferenceAsync(string photoReference)
        {
            string photoUri = await SimpleWebRequest.GetRedirectFromEndpointAsync(PhotoSearchEndPoint,
                new Dictionary<string, object>
                {
                    {"maxwidth", "1600"},
                    {"photoreference", photoReference},
                    {"key", ApiKey}
                });

            Logger.Trace("{0} redirected uri: {1}", PhotoSearchEndPoint, photoUri);

            return photoUri ?? string.Empty;
        }

        /// <summary>See <see href="https://developers.google.com/places/web-service/search">Google's Places API</see> for how this is structured.</summary>
        [SuppressMessage("ReSharper", "CollectionNeverUpdated.Local", Justification =
            "Class is used to get the response, so updating collections is unneccessary.")]
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local", Justification =
            "Setter is required for proper deserialisation.")]
        [SuppressMessage("ReSharper", "ClassNeverInstantiated.Local", Justification =
            "It is instantiated by " + nameof(SimpleWebRequest.GetResponseFromEndpointAsync) + " at runtime.")]
        private class Response
        {
            public List<Candidate> Candidates { get; set; }
            public string Status { get; set; }
        }

        /// <summary>See <see href="https://developers.google.com/places/web-service/search">Google's Places API</see> for how this is structured.</summary>
        [SuppressMessage("ReSharper", "CollectionNeverUpdated.Local", Justification =
            "Class is used to get the response, so updating collections is unneccessary.")]
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local", Justification =
            "Setter is required for proper deserialisation.")]
        [SuppressMessage("ReSharper", "ClassNeverInstantiated.Local", Justification =
            "It is instantiated by " + nameof(SimpleWebRequest.GetResponseFromEndpointAsync) + " at runtime.")]
        private class Candidate
        {
            public List<Photo> Photos { get; set; }
            public double Rating { get; set; }
        }

        /// <summary>See <see href="https://developers.google.com/places/web-service/search">Google's Places API</see> for how this is structured.</summary>
        [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Naming is dictated by the Google API.")]
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local", Justification =
            "Setter is required for proper deserialisation.")]
        [SuppressMessage("ReSharper", "ClassNeverInstantiated.Local", Justification =
            "It is instantiated by " + nameof(SimpleWebRequest.GetResponseFromEndpointAsync) + " at runtime.")]
        private class Photo
        {
            public int Height { get; set; }
            public int Width { get; set; }
            public string Photo_Reference { get; set; }
        }
    }
}