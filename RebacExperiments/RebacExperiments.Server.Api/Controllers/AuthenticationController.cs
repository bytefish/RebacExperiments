// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using RebacExperiments.Server.Api.Dto;
using RebacExperiments.Server.Api.Infrastructure.Database;
using RebacExperiments.Server.Api.Infrastructure.Exceptions;
using RebacExperiments.Server.Api.Infrastructure.Logging;
using RebacExperiments.Server.Api.Services;
using System.Security.Claims;

namespace RebacExperiments.Server.Api.Controllers
{
    public class AuthenticationController : ControllerBase
    {
        private readonly ILogger<AuthenticationController> _logger;

        public AuthenticationController(ILogger<AuthenticationController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        [Route("sign-in")]
        public async Task<IActionResult> SignInUser([FromServices] ApplicationDbContext context, [FromServices] IUserService userService, [FromBody] CredentialsDto credentials, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            try
            {
                // Create ClaimsPrincipal from Database 
                var userClaims = await userService.GetClaimsAsync(
                    context: context,
                    username: credentials.Username,
                    password: credentials.Password,
                    cancellationToken: cancellationToken);

                // Create the ClaimsPrincipal
                var claimsIdentity = new ClaimsIdentity(userClaims, CookieAuthenticationDefaults.AuthenticationScheme);
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                // It's a valid ClaimsPrincipal, sign in
                await HttpContext.SignInAsync(claimsPrincipal, new AuthenticationProperties { IsPersistent = credentials.RememberMe });

                return Ok();
            } 
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ControllerAction} failed due to an Exception", nameof(SignInUser));

                return ex switch
                {
                    AuthenticationFailedException _ => Unauthorized(),
                    _ => StatusCode(500, "An Internal Server Error occured"),
                };
            }
        }

        [HttpPost]
        [Route("sign-out")]
        public async Task<IActionResult> SignOutUser()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return Ok();
        }
    }
}