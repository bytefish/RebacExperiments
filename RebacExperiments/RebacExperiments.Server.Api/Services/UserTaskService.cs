// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;
using RebacExperiments.Server.Api.Infrastructure.Constants;
using RebacExperiments.Server.Api.Infrastructure.Database;
using RebacExperiments.Server.Api.Infrastructure.Errors;
using RebacExperiments.Server.Api.Infrastructure.Logging;
using RebacExperiments.Server.Api.Infrastructure.Services;
using RebacExperiments.Server.Api.Models;

namespace RebacExperiments.Server.Api.Services
{
    public class UserTaskService : IUserTaskService
    {
        private readonly ILogger<UserTaskService> _logger;

        public UserTaskService(ILogger<UserTaskService> logger)
        {
            _logger = logger;
        }

        public async Task<ServiceResult<UserTask>> GetUserTaskByIdAsync(ApplicationDbContext context, int userTaskId, int currentUserId, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            var userTask = await context.UserTasks
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == userTaskId, cancellationToken);

            if(userTask == null)
            {
                var serviceError = new ServiceError
                {
                    ErrorCode = ErrorCodes.EntityNotFound,
                    Message = $"No UserTask found for '{userTaskId}'"
                };

                return ServiceResult.Failed<UserTask>(serviceError);
            }

            bool isAuthorized = await context.CheckUserObject(currentUserId, userTask, Relations.Viewer, cancellationToken);

            if(!isAuthorized)
            {
                var serviceError = new ServiceError
                {
                    ErrorCode = ErrorCodes.EntityUnauthorized,
                    Message = $"Unauthorized to access UserTask '{userTaskId}' with Relation '{Relations.Viewer}'"
                };

                return ServiceResult.Failed<UserTask>(serviceError);
            }

            return ServiceResult.Success(userTask);
        }
    }
}