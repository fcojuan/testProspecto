using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Prospecto.Areas.Identity.Data;
using System.Security.Claims;

namespace Prospecto.Data
{
    public class MyUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<ProspectoUser, IdentityRole>
    {
        public MyUserClaimsPrincipalFactory(UserManager<ProspectoUser> userManager, RoleManager<IdentityRole> roleManager, IHttpContextAccessor httpContextAccessor,
            IOptions<IdentityOptions> optionsAccessor) : base(userManager, roleManager, optionsAccessor)
        {
        }
        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ProspectoUser user)
        {
            var identity = await base.GenerateClaimsAsync(user);
            identity.AddClaim(new Claim("ContactName", user.NombreCompleto ?? ""));
             
            var UserRole = UserManager.GetRolesAsync(user);
            var Rool = UserRole.Result.First().ToString().Trim().ToUpper();
            identity.AddClaim(new Claim("Role", Rool));

            return identity;

        }

    }
}
