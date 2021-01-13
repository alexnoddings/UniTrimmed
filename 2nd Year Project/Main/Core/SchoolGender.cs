namespace EduLocate.Core
{
    /// <summary>Represents which genders a school is open to.</summary>
    /// <remarks>This is NOT a flag and as such should NOT be used as a bit-mask.</remarks>
    public enum SchoolGender
    {
        /// <summary>No gender specified. This usually indicates that this information is not available.</summary>
        None,

        /// <summary>This establishment is male only.</summary>
        BoysOnly,

        /// <summary>This establishment is female only.</summary>
        GirlsOnly,

        /// <summary>This establishment is open to both genders.</summary>
        Mixed
    }
}