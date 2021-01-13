using EduLocate.Services.ServiceInterfaces.School.Metadata;

namespace EduLocate.Services.GoogleSchoolMetadataService
{
    /// <inheritdoc />
    /// <summary>Provides a simple implementation of school metadata.</summary>
    public class SimpleSchoolMetadata : ISchoolMetadata
    {
        /// <inheritdoc />
        public string ImageUri { get; }

        /// <inheritdoc />
        public double Rating { get; }

        /// <summary>Constructs the simple metadata.</summary>
        /// <param name="imageUri">The <see cref="ImageUri"/></param>
        /// <param name="rating">The <see cref="Rating"/></param>
        public SimpleSchoolMetadata(string imageUri, double rating)
        {
            ImageUri = imageUri;
            Rating = rating;
        }
    }
}