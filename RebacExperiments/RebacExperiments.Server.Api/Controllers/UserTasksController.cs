// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RebacExperiments.Server.Api.Infrastructure.Authentication;
using RebacExperiments.Server.Api.Infrastructure.Constants;
using RebacExperiments.Server.Api.Infrastructure.Database;
using RebacExperiments.Server.Api.Infrastructure.Exceptions;
using RebacExperiments.Server.Api.Infrastructure.Logging;
using RebacExperiments.Server.Api.Services;

namespace RebacExperiments.Server.Api.Controllers
{
    [Authorize(Policy = Policies.RequireUserRole)]
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

            try
            {
                var userTask = await userTaskService.GetUserTaskByIdAsync(context, userTaskId, User.GetUserId(), cancellationToken);

                return Ok(userTask);
            } 
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ControllerAction} failed due to an Exception", nameof(GetUserTask));

                return ex switch
                {
                    EntityNotFoundException _ => NotFound(),
                    EntityUnauthorizedAccessException _ => Forbid(),
                    _ => StatusCode(500, "An Internal Server Error occured"),
                };
            }
        }
    }
}