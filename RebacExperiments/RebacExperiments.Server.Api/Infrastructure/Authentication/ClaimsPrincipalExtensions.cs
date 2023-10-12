using System;
using System.Security.Claims;

namespace RebacExperiments.Server.Api.Infrastructure.Authentication
{
    public static class ClaimsPrincipalExtensions
    {
        public static int GetUserId(this ClaimsPrincipal user)
        {
            var userId = user.FindFirstValue(ClaimTypes.Sid);

            if (userId == null)
            {
                throw new InvalidOperationException("No UserID found for User");
            }

            return Convert.ToInt32(userId);
        }
    }
}
