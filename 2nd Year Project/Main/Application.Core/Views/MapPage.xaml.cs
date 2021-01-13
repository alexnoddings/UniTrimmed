using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EduLocate.Application.Core.Services.Alert;
using EduLocate.Application.Core.Services.Translation;
using EduLocate.Core;
using EduLocate.Services.ServiceInterfaces.School;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace EduLocate.Application.Core.Views
{
    /// <inheritdoc />
    /// <summary>Page to display schools on a map.</summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MapPage : ContentPage
    {
        private readonly double _latitude;
        private readonly double _longitude;
        private readonly double _radiusKm;

        private bool _showReligious = true;
        private bool _showNonReligious = true;
        private bool _showGirlsOnly = true;
        private bool _showBoysOnly = true;
        private bool _showMixed = true;

        private readonly EducationStages _educationStages;

        private IList<School> _schoolsInAreaBacking;
        private IEnumerable<School> SchoolsInArea => _schoolsInAreaBacking ?? (_schoolsInAreaBacking = Task.Run<IEnumerable<School>>(async () => await DependencyService.Get<ISchoolService>().GetSchoolsInRadiusAsync(_latitude, _longitude, _radiusKm)).Result.ToList());

        /// <summary>Constructs the map at a given point.</summary>
        /// <param name="latitude">The latitude of the point centre on.</param>
        /// <param name="longitude">The longitude of the point centre on.</param>
        /// <param name="radiusKm">The radius in kilometres to show schools in.</param>
        public MapPage(double latitude, double longitude, double radiusKm, EducationStages educationStages)
        {
            InitializeComponent();

            _latitude = latitude;
            _longitude = longitude;
            _radiusKm = radiusKm;

            _educationStages = educationStages;

            UpdateSchoolPins();
            UpdateFilterMenu();

            Map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(latitude, longitude), Distance.FromKilometers(radiusKm)));
        }

        private void UpdateSchoolPins()
        {
            // Call ToList to create a copy of the pins so that the collection is not modified during iteration
            foreach (Pin pin in Map.Pins.ToList())
                Map.Pins.Remove(pin);

            var foundSchools = 0;
            foreach (School school in SchoolsInArea)
            {
                if (!school.IsOpen) continue;
                if (string.IsNullOrWhiteSpace(school.Name)) continue;

                if ((school.EducationStages & _educationStages) == 0) continue;

                if (!_showReligious && school.IsReligious) continue;
                if (!_showNonReligious && !school.IsReligious) continue;

                if (!_showBoysOnly && school.Gender == SchoolGender.BoysOnly) continue;
                if (!_showGirlsOnly && school.Gender == SchoolGender.GirlsOnly) continue;
                if (!_showMixed && school.Gender == SchoolGender.Mixed) continue;

                var schoolPin = new Pin
                {
                    Label = school.Name,
                    Address = school.Address,
                    Position = new Position(school.Latitude ?? 0.0, school.Longitude ?? 0.0),
                };

                schoolPin.Clicked += async (_, __) => await LoadSchoolPageAsync(school);
                Map.Pins.Add(schoolPin);
                foundSchools++;
            }

            if (foundSchools > 0)
            {
                string message = DependencyService.Get<ITranslationService>().Translate("NumberOfSchoolsFound").Replace("%found%", foundSchools.ToString());
                DependencyService.Get<IAlertService>().Display(message, false);
            }
            else
            {
                DependencyService.Get<IAlertService>().DisplayTranslated("NoSchoolsFoundError", false);
            }
        }

        private void UpdateFilterMenu()
        {
            ShowReligiousSwitch.IsToggled = _showReligious;
            ShowNonReligiousSwitch.IsToggled = _showNonReligious;
            ShowGirlsSwitch.IsToggled = _showGirlsOnly;
            ShowBoysSwitch.IsToggled = _showBoysOnly;
            ShowMixedSwitch.IsToggled = _showMixed;
        }

        private async Task LoadSchoolPageAsync(School school)
        {
            var schoolInfoPage = new SchoolPage(school);
            await Navigation.PushModalAsync(new NavigationPage(schoolInfoPage));
        }

        private void OpenFiltersButton_OnClicked(object sender, EventArgs e)
        {
            ExtraFiltersGrid.IsEnabled = true;
            ExtraFiltersGrid.IsVisible = true;
        }

        private void SaveFiltersButton_OnClicked(object sender, EventArgs e)
        {
            bool religionFiltersValid = AreReligionFiltersValid();
            bool genderFiltersValid = AreGenderFiltersValid();

            if (religionFiltersValid && genderFiltersValid)
            {
                ExtraFiltersGrid.IsEnabled = false;
                ExtraFiltersGrid.IsVisible = false;
                _showReligious = ShowReligiousSwitch.IsToggled;
                _showNonReligious = ShowNonReligiousSwitch.IsToggled;
                _showGirlsOnly = ShowGirlsSwitch.IsToggled;
                _showBoysOnly = ShowBoysSwitch.IsToggled;
                _showMixed = ShowMixedSwitch.IsToggled;

                UpdateSchoolPins();
            }
            else
            {
                if (!religionFiltersValid)
                {
                    DependencyService.Get<IAlertService>().DisplayTranslated("MustSelectAtLeastOneReligionError", false);
                }
                
                if (!genderFiltersValid)
                {
                    DependencyService.Get<IAlertService>().DisplayTranslated("MustSelectAtLeastOneGenderError", false);
                }
            }
        }

        private bool AreReligionFiltersValid()
        {
            bool showReligious = ShowReligiousSwitch.IsToggled;
            bool showNonReligious = ShowNonReligiousSwitch.IsToggled;

            // At least one of these must be selected, else no schools will be displayed
            return showReligious || showNonReligious;
        }

        private bool AreGenderFiltersValid()
        {
            bool showBoysOnly = ShowBoysSwitch.IsToggled;
            bool showGirlsOnly = ShowGirlsSwitch.IsToggled;
            bool showMixed = ShowMixedSwitch.IsToggled;

            // At least one of these must be selected, else no schools will be displayed
            return showBoysOnly || showGirlsOnly || showMixed;
        }
    }
}