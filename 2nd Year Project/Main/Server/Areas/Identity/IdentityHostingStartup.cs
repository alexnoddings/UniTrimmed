using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(EduLocate.Server.Areas.Identity.IdentityHostingStartup))]

namespace EduLocate.Server.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => { });
        }
    }
}