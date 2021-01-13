using System;
using System.IO;
using System.Text;
using NLog;
using NLog.Config;
using NLog.Targets;
using EduLocate.Application.Core.Views;
using EduLocate.Services.GoogleSchoolMetadataService;
using EduLocate.Services.ServerApiSchoolService;
using EduLocate.Services.ServiceInterfaces.School;
using EduLocate.Services.ServiceInterfaces.School.Metadata;
using Xamarin.Forms;

namespace EduLocate.Application.Core
{
    /// <inheritdoc />
    public partial class App : Xamarin.Forms.Application
    {
        /// <summary>Constructs the application, handling dependency service registration.</summary>
        public App()
        {
            bool logValidSettings;
            string logFileName = string.Empty;
            string logArchiveFileName = string.Empty;

            switch (Device.RuntimePlatform)
            {
                case Device.Android:
                    logFileName = "/data/data/uk.ac.ncl.edulocate/logs/edulocate-latest.log";
                    logArchiveFileName = "/data/data/uk.ac.ncl.edulocate/logs/edulocate-{#}-log";
                    logValidSettings = true;
                    break;
                case Device.UWP:
                    string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                    logFileName = Path.Combine(path, "edulocate-latest.log");
                    logArchiveFileName = Path.Combine(path, "edulocate-{#}-log");
                    logValidSettings = true;
                    break;
                default:
                    logValidSettings = false;
                    break;
            }

            if (logValidSettings)
            {
                LogManager.Configuration = LogManager.Configuration ?? new LoggingConfiguration();
                LogManager.Configuration.AddTarget(new FileTarget("AndroidLogFile")
                {
                    FileName = logFileName,
                    ArchiveFileName = logArchiveFileName,
                    ArchiveEvery = FileArchivePeriod.Day,
                    ArchiveNumbering = ArchiveNumberingMode.Date,
                    MaxArchiveFiles = 31,
                    ArchiveDateFormat = "yyyy-MM-dd-HH-mm",
                    Encoding = Encoding.UTF8,
                });
            }

            InitializeComponent();

#if MOCK_SERVICES
            DependencyService.Register<ISchoolService, MockSchoolService>();
            DependencyService.Register<ISchoolMetadataService, MockMetadataService>();
#else
            DependencyService.Register<ISchoolService, ServerApiSchoolService>();
            DependencyService
                .Register<ISchoolMetadataService, GoogleSchoolMetadataService>();
#endif

            MainPage = new WelcomePage();
        }
    }
}