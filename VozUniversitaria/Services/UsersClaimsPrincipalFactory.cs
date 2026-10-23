using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using VozUniversitaria.Models;

namespace VozUniversitaria.Services;

/// <summary>Personaliza los claims de Identity con el nombre completo de la cuenta.</summary>
public sealed class UsersClaimsPrincipalFactory
    : UserClaimsPrincipalFactory<Users, IdentityRole<int>>
{
    /// <summary>Inicializa la fábrica con los administradores de usuarios y roles de Identity.</summary>
    public UsersClaimsPrincipalFactory(
        UserManager<Users> userManager,
        RoleManager<IdentityRole<int>> roleManager,
        IOptions<IdentityOptions> optionsAccessor)
        : base(userManager, roleManager, optionsAccessor)
    {
    }

    /// <summary>Genera los claims estándar y reemplaza el claim de nombre por nombre y apellido.</summary>
    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(Users user)
    {
        var identity = await base.GenerateClaimsAsync(user);

        // Se reemplaza únicamente el claim de nombre, conservando los demás claims de Identity.
        var nameClaim = identity.FindFirst(identity.NameClaimType);
        if (nameClaim is not null)
        {
            identity.RemoveClaim(nameClaim);
        }

        identity.AddClaim(new Claim(identity.NameClaimType, $"{user.Name} {user.LastName}"));
        return identity;
    }
}
