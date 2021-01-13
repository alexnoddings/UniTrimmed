using System;
using System.Linq;
using System.Threading.Tasks;
using NLog;
using EduLocate.Application.Core.Services.Alert;
using EduLocate.Core;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace EduLocate.Application.Core.Views
{
    /// <inheritdoc />
    /// <summary>The first page the users see.</summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WelcomePage : ContentPage
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>Constructs the page.</summary>
        public WelcomePage()
        {
            InitializeComponent();
        }

        private async void UseDeviceLocation_OnClicked_Async(object sender, EventArgs e)
        {
            double? latitude;
            double? longitude;
            try
            {
                Location location = await Geolocation.GetLastKnownLocationAsync() ??
                                    await Geolocation.GetLocationAsync(new GeolocationRequest(GeolocationAccuracy.High));
                latitude = location?.Latitude;
                longitude = location?.Longitude;
            }
            catch (FeatureNotSupportedException ex)
            {
                Logger.Warn(ex, "{0} while fetching last known location", nameof(FeatureNotSupportedException));
                DependencyService.Get<IAlertService>().DisplayTranslated("LocationFeaturesNotSupportedOnDevice", false);
                return;
            }
            catch (FeatureNotEnabledException ex)
            {
                Logger.Warn(ex, "{0} while fetching last known location", nameof(FeatureNotEnabledException));
                DependencyService.Get<IAlertService>().DisplayTranslated("LocationFeaturesDisabledOnDevice", false);
                return;
            }
            catch (PermissionException ex)
            {
                Logger.Warn(ex, "{0} while fetching last known location", nameof(PermissionException));
                DependencyService.Get<IAlertService>().DisplayTranslated("LocationFeatureMissingPermission", false);
                return;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "{0} while fetching last known location", nameof(Exception));
                DependencyService.Get<IAlertService>().DisplayTranslated("LocationUnknownError", false);
                return;
            }

            if (latitude == null)
            {
                Logger.Error("No exception while fetching last known location, but returned values were null");
                DependencyService.Get<IAlertService>().DisplayTranslated("LocationUnknownError", false);
                return;
            }

            await ShowMapForAsync(latitude.Value, longitude.Value);
        }

        private async void EnterLocationManually_OnClicked_Async(object sender, EventArgs e)
        {
            string address = ManualAddressEditor.Text;
            if (string.IsNullOrWhiteSpace(address))
            {
                DependencyService.Get<IAlertService>().DisplayTranslated("AddressPostcodeNotFound", false);
                return;
            }

            Position position = (await new Geocoder().GetPositionsForAddressAsync(address)).FirstOrDefault();
            if (position == default)
            {
                DependencyService.Get<IAlertService>().DisplayTranslated("AddressPostcodeNotFound", false);
                return;
            }

            await ShowMapForAsync(position.Latitude, position.Longitude);
        }

        private async Task ShowMapForAsync(double latitude, double longitude)
        {
            var educationStages = EducationStages.None;
            if (PrimarySchoolSwitch.IsToggled)
                educationStages |= EducationStages.Primary;
            if (SecondarySchoolSwitch.IsToggled)
                educationStages |= EducationStages.Secondary;
            if (CollegeSchoolSwitch.IsToggled)
                educationStages |= EducationStages.College;

            if (educationStages == EducationStages.None)
            {
                DependencyService.Get<IAlertService>().DisplayTranslated("MustSelectAtLeastOneStageError", false);
            }
            else
            {
                var mapPage = new MapPage(latitude, longitude, DistanceSlider.Value, educationStages);
                await Navigation.PushModalAsync(mapPage);
            }
        }
    }
}