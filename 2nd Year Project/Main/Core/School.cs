using System;

namespace EduLocate.Core
{
    /// <summary>Represents a school.</summary>
    public class School
    {
        #region General Data

        /// <summary>The schools ID (sometimes referred to as the URN or Unique Reference Number).</summary>
        public int Id { get; set; }

        /// <summary>Whether the school is open.</summary>
        public bool IsOpen { get; set; }

        /// <summary>The schools name.</summary>
        public string Name { get; set; }

        /// <summary>The contact telephone number.</summary>
        public string Telephone { get; set; }

        /// <summary>The stages of education offered.</summary>
        public EducationStages EducationStages { get; set; }

        /// <summary>The gender of the school.</summary>
        public SchoolGender Gender { get; set; }

        /// <summary>The religion the school has specified.</summary>
        public string Religion { get; set; }

        /// <summary>The schools last Ofsted rating. Null if no Ofsted data was found for the school.</summary>
        public int? OfstedRating { get; set; }

        /// <summary>The date of the schools last Ofsted rating. Does not have a time component. Is equal to <code>default(DateTime)</code> if no Ofsted data was found for the school.</summary>
        public DateTime LastOfstedInspection { get; set; }

        /// <summary>The facebook social link for the school. Null if one was not found from their website.</summary>
        public string FacebookLink { get; set; }

        /// <summary>The facebook twitter link for the school. Null if one was not found from their website.</summary>
        public string TwitterLink { get; set; }

        #endregion

        #region Pupil Stats

        /// <summary>How much money is spent on each pupil on average. Null if no spending data was found for the school.</summary>
        public int? MoneySpentPerStudent { get; set; }

        /// <summary>How many pupils are currently enrolled at the school. Null if no pupil data was found for the school.</summary>
        public int? NumberOfPupils { get; set; }

        /// <summary>What percentage of pupils are female. Null if no pupil data was found for the school.</summary>
        /// <remarks>See also <see cref="PercentBoys"/>.</remarks>
        public double? PercentGirls { get; set; }

        #endregion

        #region Location

        /// <summary>The latitude of the school. Null if no position data was found for the school.</summary>
        public double? Latitude { get; set; }

        /// <summary>The longitude of the school. Null if no position data was found for the school.</summary>
        public double? Longitude { get; set; }

        /// <summary>The address of the school. Null if no location data was found for the school.</summary>
        public string Address { get; set; }

        /// <summary>The town of the school. Null if no location data was found for the school.</summary>
        public string Town { get; set; }

        /// <summary>The postcode of the school. Null if no location data was found for the school.</summary>
        public string Postcode { get; set; }

        #endregion

        #region Expression-bodied members

        /// <summary>Whether this school offers primary-school level education.</summary>
        public bool OffersPrimaryEducation => EducationStages.HasFlag(EducationStages.Primary);

        /// <summary>Whether this school offers secondary-school level education.</summary>
        public bool OffersSecondaryEducation => EducationStages.HasFlag(EducationStages.Secondary);

        /// <summary>Whether this school offers college level education.</summary>
        public bool OffersCollegeEducation => EducationStages.HasFlag(EducationStages.College);

        /// <summary>Whether this school identifies as religious.</summary>
        public bool IsReligious => Religion != null &&
                                   !(Religion.ToLower().Contains("none") ||
                                     Religion.ToLower().Contains("does not apply"));

        /// <summary>What percentage of pupils are male. Null if <see cref="PercentGirls"/> is null.</summary>
        public double? PercentBoys => 100 - PercentGirls;

        #endregion
    }
}