using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Sample.Common.Entities;
using System.Security.Claims;

namespace ProjectWithAuthenticationSample.Infrastructure.Authentication
{
    public static class AuthenticationHttpContextExtensions
    {
        public static async Task AuthenticateUser(this HttpContext httpContext, User user, bool rememberMe, byte sessionTimeout)
        {
            // Data in cookie
            var claims = ClaimsProvider.GetUserClaims(user, rememberMe, sessionTimeout);

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme, ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);


            await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), new AuthenticationProperties
            {
                ExpiresUtc = rememberMe ? DateTime.UtcNow.AddYears(1) : DateTime.UtcNow.AddHours(8)
            });
        }

        public static async Task SignOutUserAsync(this HttpContext httpContext)
        {
            await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}
