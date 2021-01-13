using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EduLocate.Server.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using EduLocate.Server.Models;
using EduLocate.Services.ClosedXmlExcelDataService;
using EduLocate.Services.CoordinatesService;
using EduLocate.Services.ServiceInterfaces.Coordinates;
using EduLocate.Services.ServiceInterfaces.Excel;
using EduLocate.Services.ServiceInterfaces.School;

namespace EduLocate.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("ApplicationDatabase")));

            services.AddDefaultIdentity<IdentityUser>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequiredUniqueChars = 6;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireDigit = true;

                options.User.AllowedUserNameCharacters =
                    @"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789@+-._";
                options.User.RequireUniqueEmail = true;
            });

            services.AddAuthorization(options =>
            {
                foreach ((string policyName, Action<AuthorizationPolicyBuilder> policyAction) in Policies
                    .GenerateAuthorisationPolicies())
                    options.AddPolicy(policyName, policyAction);
            });

            services.AddMvc()
                .AddRazorPagesOptions(options =>
                {
                    options.Conventions.AuthorizeAreaFolder("Identity", "/Account/Admin",
                        Policies.UserManagerPolicyName);
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddScoped<IExcelDataService, ClosedXmlExcelDataService>();
            services.AddScoped<ISchoolRepository, EfSchoolRepository>();
            services.AddScoped<ICoordinatesService, SimpleCoordinatesService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    "default",
                    "{controller=Home}/{action=Index}/{id?}");
            });

            //Task seedTask = IdentitySeedingHelper.SeedAsync(userManager, roleManager);
            //seedTask.GetAwaiter().GetResult();
        }
    }
}