// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RebacExperiments.Server.Api.Infrastructure.Authentication;
using RebacExperiments.Server.Api.Infrastructure.Constants;
using RebacExperiments.Server.Api.Infrastructure.Database;
using RebacExperiments.Server.Api.Infrastructure.Exceptions;
using RebacExperiments.Server.Api.Infrastructure.Logging;
using RebacExperiments.Server.Api.Models;
using RebacExperiments.Server.Api.Services;

namespace RebacExperiments.Server.Api.Controllers
{
    [Route("UserTasks")]
    public class UserTasksController : ControllerBase
    {
        private readonly ILogger<UserTasksController> _logger;

        public UserTasksController(ILogger<UserTasksController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("{id}")]
        [Authorize(Policy = Policies.RequireUserRole)]
        public async Task<IActionResult> GetUserTask([FromServices] ApplicationDbContext context, [FromServices] IUserTaskService userTaskService, [FromRoute(Name = "id")] int userTaskId, CancellationToken cancellationToken)
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

        [HttpGet]
        [Authorize(Policy = Policies.RequireUserRole)]
        public async Task<IActionResult> GetUserTasks([FromServices] ApplicationDbContext context, [FromServices] IUserTaskService userTaskService, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            try
            {
                var userTasks = await userTaskService.GetUserTasksAsync(context, User.GetUserId(), cancellationToken);

                return Ok(userTasks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ControllerAction} failed due to an Exception", nameof(GetUserTask));

                return ex switch
                {
                    _ => StatusCode(500, "An Internal Server Error occured"),
                };
            }
        }

        [HttpPost]
        [Authorize(Policy = Policies.RequireUserRole)]
        public async Task<IActionResult> PostUserTask([FromServices] ApplicationDbContext context, [FromServices] IUserTaskService userTaskService, [FromBody] UserTask userTask, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            try
            {
                await userTaskService.CreateUserTaskAsync(context, userTask, User.GetUserId(), cancellationToken);

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

        [HttpPut]
        [Route("{id}")]
        [Authorize(Policy = Policies.RequireUserRole)]
        public async Task<IActionResult> PutUserTask([FromServices] ApplicationDbContext context, [FromServices] IUserTaskService userTaskService, [FromBody] UserTask userTask, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            try
            {
                await userTaskService.UpdateUserTaskAsync(context, userTask, User.GetUserId(), cancellationToken);

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

        [HttpDelete]
        [Route("{id}")]
        [Authorize(Policy = Policies.RequireUserRole)]
        public async Task<IActionResult> DeleteUserTask([FromServices] ApplicationDbContext context, [FromServices] IUserTaskService userTaskService, [FromRoute(Name = "id")] int userTaskId, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            try
            {
                await userTaskService.DeleteUserTaskAsync(context, userTaskId, User.GetUserId(), cancellationToken);

                return Ok();
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