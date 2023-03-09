using MAP_S_TestApp.Models.Domains;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace MAP_S_TestApp.Helpers;

public static class AuthenticateHelper
{
    public static ClaimsIdentity CreateClaimsIdentity(ApplicationUser applicationUser) 
    {
        var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, applicationUser.Email),
                new Claim(ClaimTypes.Name, applicationUser.FirstName),
                new Claim(ClaimTypes.Surname, applicationUser.LastName),
            };

        return new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
    }

}
