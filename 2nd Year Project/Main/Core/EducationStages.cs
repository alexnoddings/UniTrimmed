using System;

namespace EduLocate.Core
{
    /// <summary>Represents which stages of education somewhere offers.</summary>
    /// <remarks>Can be used as a bit-mask as it is a flag.</remarks>
    [Flags]
    public enum EducationStages
    {
        /// <summary>No education stages provided. This usually indicates that this information is not available.</summary>
        None = 0,

        /// <summary>This establishment offers primary-school level education.</summary>
        Primary = 1,

        /// <summary>This establishment offers secondary-school level education.</summary>
        Secondary = 2,

        /// <summary>This establishment offers college level education.</summary>
        College = 4
    }
}