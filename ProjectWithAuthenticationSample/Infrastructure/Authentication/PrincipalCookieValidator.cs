using Microsoft.AspNetCore.Authentication.Cookies;
using Sample.BLL.Services;
using System.Security.Claims;

namespace ProjectWithAuthenticationSample.Infrastructure.Authentication
{
    public static class PrincipalCookieValidator
    {
        internal static async Task ValidatePrincipal(CookieValidatePrincipalContext context)
        {
            var userService = context.HttpContext.RequestServices.GetRequiredService<UserService>();
            var userId = context.Principal.GetUserId();
            var userLastUpdatedUtc = await userService.GetLastUpdatedTimeAsync(userId);

            var identity = context.Principal.Identity as ClaimsIdentity;
            // ReSharper disable once PossibleNullReferenceException
            var dateTimeIssuedClaim = identity.FindFirst(CustomClaimTypes.DateTimeIssued);
            var issuedDateUtc = new DateTime(long.Parse(dateTimeIssuedClaim.Value), DateTimeKind.Utc);

            // user may not have the update date yet, so set it to just before issue date
            userLastUpdatedUtc = userLastUpdatedUtc ?? issuedDateUtc.AddSeconds(-10);

            var isValid = issuedDateUtc > userLastUpdatedUtc;

            if (isValid)
            {
                return;
            }

            var user = await userService.GetByIdNotTrackingAsync(userId);

            // if user doesn't exist or is inactive, log them out
            if (user == null)
            {
                context.RejectPrincipal();
                return;
            }

            var newClaims = ClaimsProvider.GetUserClaims(user, context.Principal.IsRememberMe(),
                default);

            var newIdentity = new ClaimsIdentity(newClaims, CookieAuthenticationDefaults.AuthenticationScheme,
                ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            var newPrincipal = new ClaimsPrincipal(newIdentity);

            context.ReplacePrincipal(newPrincipal);
            context.ShouldRenew = true;
        }
    }
}
