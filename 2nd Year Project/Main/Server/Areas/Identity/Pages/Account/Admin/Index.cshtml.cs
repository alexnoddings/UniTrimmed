using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using EduLocate.Server.Models;

namespace EduLocate.Server.Areas.Identity.Pages.Account.Admin
{
    public partial class IndexModel : PageModel
    {
        public class IndexUserModel
        {
            public string Id { get; }
            public string UserName { get; }
            public string Email { get; }

            public string UserManagerChecked { get; }
            public string DataManagerChecked { get; }

            public IndexUserModel(IdentityUser user, IList<Claim> claims)
            {
                Id = user.Id;
                UserName = user.UserName;
                Email = user.Email;
                UserManagerChecked = Policies.HasUserManagerClaim(claims) ? "checked" : null;
                DataManagerChecked = Policies.HasDataManagerClaim(claims) ? "checked" : null;
            }
        }

        private readonly UserManager<IdentityUser> _userManager;

        public IndexModel(
            UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public IEnumerable<IndexUserModel> Users { get; private set; }

        [TempData] public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var users = new List<IndexUserModel>();
            foreach (IdentityUser user in _userManager.Users)
            {
                IList<Claim> claims = await _userManager.GetClaimsAsync(user);
                users.Add(new IndexUserModel(user, claims));
            }

            Users = users;

            return Page();
        }
    }
}