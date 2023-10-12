// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Mvc;
using RebacExperiments.Server.Api.Infrastructure.Authentication;
using RebacExperiments.Server.Api.Infrastructure.Database;
using RebacExperiments.Server.Api.Infrastructure.Errors;
using RebacExperiments.Server.Api.Infrastructure.Logging;
using RebacExperiments.Server.Api.Services;

namespace RebacExperiments.Server.Api.Controllers
{
    public class UserTasksController : ControllerBase
    {
        private readonly ILogger<UserTasksController> _logger;

        public UserTasksController(ILogger<UserTasksController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("usertasks/{userTaskId}")]
        public async Task<IActionResult> GetUserTask([FromServices] ApplicationDbContext context, [FromServices] IUserTaskService userTaskService, [FromRoute] int userTaskId, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            // Create ClaimsPrincipal from Database 
            var serviceResult = await userTaskService.GetUserTaskByIdAsync(context, userTaskId, User.GetUserId(), cancellationToken);

            // If it's not a valid user return 
            if (!serviceResult.Succeeded)
            {
                if(_logger.IsErrorEnabled())
                {
                    _logger.LogError("Authentication failed with ErrorCode = {ErrorCode} and ErrorMessage = {ErrorMessage}", serviceResult.Error.ErrorCode, serviceResult.Error.Message);
                }

                return serviceResult.Error.ErrorCode switch
                {
                    ErrorCodes.EntityNotFound => NotFound(),
                    ErrorCodes.EntityUnauthorized => Forbid(),
                    _ => BadRequest(),
                };
            }

            return Ok(serviceResult.Data);
        }
    }
}