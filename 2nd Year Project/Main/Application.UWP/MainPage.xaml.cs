using EduLocate.Common;

namespace EduLocate.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();

            Xamarin.FormsMaps.Init();
            Windows.Services.Maps.MapService.ServiceToken = Keys.BingMapsApi;

            LoadApplication(new EduLocate.Application.Core.App());
        }
    }
}