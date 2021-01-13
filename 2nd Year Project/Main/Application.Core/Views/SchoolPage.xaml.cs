using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using EduLocate.Application.Core.Services.Theme;
using EduLocate.Application.Core.Services.Translation;
using EduLocate.Core;
using EduLocate.Services.ServiceInterfaces.School.Metadata;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EduLocate.Application.Core.Views
{
    /// <inheritdoc />
    /// <summary>Page to display information about a given school.</summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SchoolPage : ContentPage
    {
        private readonly School _school;

        /// <summary>Constructs the page for a school.</summary>
        /// <param name="school">The school to display the information of.</param>
        public SchoolPage(School school)
        {
            InitializeComponent();

            _school = school;
            SetupPage();
        }

        private void SetupPage()
        {
            SetPageMetadataAsync().ConfigureAwait(false);

            Title = _school.Name;

            string notProvided = DependencyService.Get<ITranslationService>().Translate("NotProvided");
            var schoolMetadataService = DependencyService.Get<ISchoolMetadataService>();
            GeneralRatingTextLabel.Text = DependencyService.Get<ITranslationService>().Translate("GeneralRating")
                .Replace("%provider%", schoolMetadataService.ProviderName);
            GeneralRatingLabel.Text = DependencyService.Get<ITranslationService>().Translate("LoadingMessage");

            GeneralAddressLabel.Text = _school.Address ?? notProvided;
            GeneralTelephoneLabel.Text = _school.Telephone ?? notProvided;

            GeneralEducationStagesLabel.Text = GetEducationStagesString(_school.EducationStages);

            GeneralGenderLabel.Text = _school.Gender.ToString();
            GeneralReligionLabel.Text = _school.Religion ?? notProvided;

            PupilStatsMoneySpentLabel.Text =
                _school.MoneySpentPerStudent?.ToString(CultureInfo.CurrentCulture) ?? notProvided;
            PupilStatsTotalLabel.Text = _school.NumberOfPupils?.ToString(CultureInfo.CurrentCulture) ?? notProvided;

            if (_school.PercentBoys == null)
                PupilStatsBoysLabel.Text = notProvided;
            else
                PupilStatsBoysLabel.Text = _school.PercentBoys.Value.ToString(CultureInfo.CurrentCulture) + "%";

            if (_school.PercentGirls == null)
                PupilStatsGirlsLabel.Text = notProvided;
            else
                PupilStatsGirlsLabel.Text = _school.PercentGirls.Value.ToString(CultureInfo.CurrentCulture) + "%";

            switch (_school.OfstedRating)
            {
                case 1:
                    OfstedRatingLabel.Text = "Outstanding";
                    OfstedRatingNumber.Text = " 1 ";
                    OfstedRatingNumber.BackgroundColor = Color.DarkGreen;
                    break;
                case 2:
                    OfstedRatingLabel.Text = "Good";
                    OfstedRatingNumber.Text = " 2 ";
                    OfstedRatingNumber.BackgroundColor = Color.RoyalBlue;
                    break;
                case 3:
                    OfstedRatingLabel.Text = "Requires improvement";
                    OfstedRatingNumber.Text = " 3 ";
                    OfstedRatingNumber.BackgroundColor = Color.OrangeRed;
                    break;
                case 4:
                    OfstedRatingLabel.Text = "Inadequate";
                    OfstedRatingNumber.Text = " 4 ";
                    OfstedRatingNumber.BackgroundColor = Color.DarkRed;
                    break;
                default:
                    OfstedRatingLabel.Text = _school.OfstedRating?.ToString(CultureInfo.CurrentCulture) ?? notProvided;
                    OfstedRatingNumber.Text = " ? ";
                    OfstedRatingNumber.BackgroundColor = Color.Gray;
                    break;
            }

            OfstedRatingDateLabel.Text = _school.LastOfstedInspection == default
                ? notProvided
                : _school.LastOfstedInspection.ToShortDateString();

            if (!string.IsNullOrWhiteSpace(_school.FacebookLink))
            {
                SocialLinksFacebook.Text = _school.FacebookLink;
                SocialLinksFacebook.GestureRecognizers.Add(new TapGestureRecognizer
                    {Command = new Command(() => Device.OpenUri(new Uri(_school.FacebookLink)))});
                SocialLinksFacebook.TextColor = DependencyService.Get<IThemeService>().PrimaryColour;
            }
            else
            {
                SocialLinksFacebook.Text = notProvided;
            }

            if (!string.IsNullOrWhiteSpace(_school.TwitterLink))
            {
                SocialLinksTwitter.Text = _school.TwitterLink;
                SocialLinksTwitter.GestureRecognizers.Add(new TapGestureRecognizer
                    { Command = new Command(() => Device.OpenUri(new Uri(_school.TwitterLink))) });
                SocialLinksTwitter.TextColor = DependencyService.Get<IThemeService>().PrimaryColour;
            }
            else
            {
                SocialLinksTwitter.Text = notProvided;
            }
        }

        private static string GetEducationStagesString(EducationStages educationStages)
        {
            var stageNames = new List<string>();

            if (educationStages.HasFlag(EducationStages.Primary))
                stageNames.Add(EducationStages.Primary.ToString());
            if (educationStages.HasFlag(EducationStages.Secondary))
                stageNames.Add(EducationStages.Secondary.ToString());
            if (educationStages.HasFlag(EducationStages.College))
                stageNames.Add(EducationStages.College.ToString());

            return string.Join(", ", stageNames);
        }

        private async Task SetPageMetadataAsync()
        {
            var schoolMetadataService = DependencyService.Get<ISchoolMetadataService>();
            ISchoolMetadata metadata = await schoolMetadataService.GetMetadataForSchoolAsync(_school);
            string imageUri = metadata?.ImageUri ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(imageUri))
            {
                BannerImage.Source = imageUri;
                BannerImageGrid.IsEnabled = true;
                BannerImageGrid.IsVisible = true;
                BannerImageDisclaimer.Text = DependencyService.Get<ITranslationService>().Translate("ImageProvidedBy").Replace("%provider%", schoolMetadataService.ProviderName);
            }
            string rating = metadata?.Rating.ToString(CultureInfo.CurrentCulture) ??
                            DependencyService.Get<ITranslationService>().Translate("NotProvided");
            GeneralRatingLabel.Text = rating;
        }
    }
}