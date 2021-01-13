using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using EduLocate.Server.Models;

namespace EduLocate.Server.Data
{
    /// <summary>Helper for seeding the identity system.</summary>
    internal static class IdentitySeedingHelper
    {
        /// <summary>Seeds users and roles.</summary>
        /// <param name="userManager">The UserManager to seed.</param>
        /// <param name="roleManager">The RoleManager to seed.</param>
        internal static async Task SeedAsync(UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            await SeedUsersAsync(userManager);
        }

        private static async Task SeedUsersAsync(UserManager<IdentityUser> userManager)
        {
            string sysAdminUsername = Environment.GetEnvironmentVariable("ServerSysAdminUsername");
            string sysAdminEmail = Environment.GetEnvironmentVariable("ServerSysAdminEmail");
            string sysAdminPass = Environment.GetEnvironmentVariable("ServerSysAdminPassword");

            if (sysAdminUsername == null || sysAdminEmail == null || sysAdminPass == null)
                return;

            if (userManager.FindByNameAsync(sysAdminUsername).Result != null) return;

            var user = new IdentityUser
            {
                UserName = sysAdminUsername,
                Email = sysAdminEmail
            };
            IdentityResult result = await userManager.CreateAsync(user, sysAdminPass);
            if (result.Succeeded)
                await userManager.AddClaimAsync(user, new Claim(Policies.UserManagerClaimName, "true"));
        }
    }
}