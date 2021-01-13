namespace EduLocate.Services.ServiceInterfaces.School.Metadata
{
    /// <summary>Metadata about a school.</summary>
    public interface ISchoolMetadata
    {
        /// <summary>An image of the school. May be null or empty if one could not be found.</summary>
        string ImageUri { get; }

        /// <summary>A rating of the school.</summary>
        double Rating { get; }
    }
}