// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using RebacExperiments.Server.Api.Infrastructure.Authentication;
using RebacExperiments.Server.Api.Infrastructure.Constants;
using RebacExperiments.Server.Api.Infrastructure.Database;
using RebacExperiments.Server.Api.Infrastructure.Errors;
using RebacExperiments.Server.Api.Infrastructure.Logging;
using RebacExperiments.Server.Api.Infrastructure.Services;
using RebacExperiments.Server.Api.Models;
using System.Security.Claims;


namespace RebacExperiments.Server.Api.Services
{
    public class UserService : IUserService
    {
        private readonly ILogger<UserService> _logger;
        private readonly IPasswordHasher _passwordHasher;

        public UserService(ILogger<UserService> logger, IPasswordHasher passwordHasher)
        {
            _logger = logger;
            _passwordHasher = passwordHasher;
        }

        public async Task<ServiceResult<List<Claim>>> GetClaimsAsync(ApplicationDbContext context, string username, string password, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            var user = await context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.LogonName == username, cancellationToken);

            if(user == null)
            {
                var serviceError = new ServiceError
                {
                    ErrorCode = ErrorCodes.AuthenticationFailed,
                    Message = $"Failed to read user details for user '{username}'"
                };

                return ServiceResult.Failed<List<Claim>>(serviceError);
            }

            if (!user.IsPermittedToLogon)
            {
                var serviceError = new ServiceError
                {
                    ErrorCode = ErrorCodes.AuthenticationFailed,
                    Message = $"User '{username}' is not permitted to login",
                };

                return ServiceResult.Failed<List<Claim>>(serviceError);
            }

            // Verify hashed password in database against the provided password
            var isVerifiedPassword = _passwordHasher.VerifyHashedPassword(user.HashedPassword, password);

            if (!isVerifiedPassword)
            {
                var serviceError = new ServiceError
                {
                    ErrorCode = ErrorCodes.AuthenticationFailed,
                    Message = $"Password mismatch for '{username}'"
                };

                return ServiceResult.Failed<List<Claim>>(serviceError);
            }

            // Load the Roles from the List of Objects
            var roles = await context
                .ListUserObjects<Role>(user.Id, Relations.Member)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            // Build the Claims for the ClaimsPrincipal
            var claims = CreateClaims(user, roles);

            return ServiceResult.Success(claims);
        }

        private List<Claim> CreateClaims(User user, List<Role> roles)
        {
            _logger.TraceMethodEntry();

            var claims = new List<Claim>();

            if (user.LogonName != null)
            {
                claims.Add(new Claim(ClaimTypes.Email, Convert.ToString(user.LogonName)));
            }

            // Default Claims:
            claims.Add(new Claim(ClaimTypes.Sid, Convert.ToString(user.Id)));
            claims.Add(new Claim(ClaimTypes.Name, Convert.ToString(user.PreferredName)));

            // Roles:
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.Name));
            }

            return claims;
        }
    }
}