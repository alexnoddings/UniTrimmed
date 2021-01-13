using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using EduLocate.Server.Models;

namespace EduLocate.Server.Areas.Identity.Pages.Account.Admin
{
    public partial class EditModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public EditModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public string Id { get; set; }

        public string Username { get; set; }

        [TempData] public string StatusMessage { get; set; }

        [BindProperty] public InputModel Input { get; set; }

        public class InputModel
        {
            public bool IsDataManager { get; set; }
            public bool IsUserManager { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            IdentityUser user = _userManager.Users.FirstOrDefault(u => u.Id == id);
            if (user == null) return NotFound($"Unable to load user with ID '{id}'.");

            string userName = await _userManager.GetUserNameAsync(user);

            Id = id;
            Username = userName;

            IList<Claim> userClaims = await _userManager.GetClaimsAsync(user);
            Input = new InputModel
            {
                IsDataManager = Policies.HasDataManagerClaim(userClaims),
                IsUserManager = Policies.HasUserManagerClaim(userClaims),
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (!ModelState.IsValid) return Page();

            IdentityUser user = _userManager.Users.FirstOrDefault(u => u.Id == id);
            if (user == null) return NotFound($"Unable to load user with ID '{id}'.");

            IList<Claim> userClaims = await _userManager.GetClaimsAsync(user);
            bool isDataManager = Policies.HasDataManagerClaim(userClaims);
            bool isUserManager = Policies.HasUserManagerClaim(userClaims);
            if (isDataManager != Input.IsDataManager)
            {
                if (isDataManager)
                    await _userManager.RemoveClaimAsync(user,
                        userClaims.FirstOrDefault(claim => claim.Type == Policies.DataManagerClaimName));
                else
                    await _userManager.AddClaimAsync(user, new Claim(Policies.DataManagerClaimName, "true"));
            }

            if (isUserManager != Input.IsUserManager)
            {
                if (isUserManager)
                    await _userManager.RemoveClaimAsync(user,
                        userClaims.FirstOrDefault(claim => claim.Type == Policies.UserManagerClaimName));
                else
                    await _userManager.AddClaimAsync(user, new Claim(Policies.UserManagerClaimName, "true"));
            }

            //await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "User profile has been updated";
            return RedirectToPage(new {id});
        }
    }
}