using Sample.Common.Entities;
using System.Security.Claims;

namespace ProjectWithAuthenticationSample.Infrastructure.Authentication
{
    public static class ClaimsProvider
    {
        public static List<Claim> GetUserClaims(User user)
        {
            var claims = new List<Claim>
            {
                new(ClaimsIdentity.DefaultNameClaimType, user.Username),
                new(CustomClaimTypes.UserId, user.Id.ToString()),
                new(CustomClaimTypes.DateTimeIssued, DateTime.UtcNow.Ticks.ToString()),
            };

            return claims;
        }

        public static List<Claim> GetUserClaims(User user, bool rememberMe, byte sessionTimeout)
        {
            var claims = GetUserClaims(user);

            claims.Add(new Claim(CustomClaimTypes.RememberMe, rememberMe.ToString()));
            claims.Add(new Claim(CustomClaimTypes.SessionTimeout, sessionTimeout.ToString()));

            return claims;
        }
    }
}
