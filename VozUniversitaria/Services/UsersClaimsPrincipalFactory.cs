using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using VozUniversitaria.Models;

namespace VozUniversitaria.Services;

public sealed class UsersClaimsPrincipalFactory
    : UserClaimsPrincipalFactory<Users, IdentityRole<int>>
{
    public UsersClaimsPrincipalFactory(
        UserManager<Users> userManager,
        RoleManager<IdentityRole<int>> roleManager,
        IOptions<IdentityOptions> optionsAccessor)
        : base(userManager, roleManager, optionsAccessor)
    {
    }

    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(Users user)
    {
        var identity = await base.GenerateClaimsAsync(user);
        var nameClaim = identity.FindFirst(identity.NameClaimType);
        if (nameClaim is not null)
        {
            identity.RemoveClaim(nameClaim);
        }

        identity.AddClaim(new Claim(identity.NameClaimType, $"{user.Name} {user.LastName}"));
        return identity;
    }
}
