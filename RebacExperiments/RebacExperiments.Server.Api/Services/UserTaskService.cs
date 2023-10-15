// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;
using RebacExperiments.Server.Api.Infrastructure.Constants;
using RebacExperiments.Server.Api.Infrastructure.Database;
using RebacExperiments.Server.Api.Infrastructure.Exceptions;
using RebacExperiments.Server.Api.Infrastructure.Logging;
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

        public async Task<UserTask> GetUserTaskByIdAsync(ApplicationDbContext context, int userTaskId, int currentUserId, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            var userTask = await context.UserTasks
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == userTaskId, cancellationToken);

            if(userTask == null)
            {
                throw new EntityNotFoundException() 
                {
                    EntityName = nameof(UserTask),
                    EntityId = userTaskId,
                };
            }

            bool isAuthorized = await context.CheckUserObject(currentUserId, userTask, Relations.Viewer, cancellationToken);

            if(!isAuthorized)
            {
                throw new EntityUnauthorizedAccessException()
                {
                    EntityName = nameof(UserTask),
                    EntityId = userTaskId,
                    UserId = currentUserId,
                };
            }

            return userTask;
        }

        public async Task<UserTask> UpdateUserTaskAsync(ApplicationDbContext context, UserTask userTask, int currentUserId, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            bool isAuthorized = await context.CheckUserObject(currentUserId, userTask, Relations.Owner, cancellationToken);

            if (!isAuthorized)
            {
                throw new EntityUnauthorizedAccessException()
                {
                    EntityName = nameof(UserTask),
                    EntityId = userTask.Id,
                    UserId = currentUserId,
                };
            }

            int rowsAffected = await context.UserTasks
                .Where(t => t.Id == userTask.Id && t.RowVersion == userTask.RowVersion)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(x => x.Title, userTask.Title)
                    .SetProperty(x => x.Description, userTask.Description)
                    .SetProperty(x => x.DueDateTime, userTask.DueDateTime)
                    .SetProperty(x => x.CompletedDateTime, userTask.CompletedDateTime)
                    .SetProperty(x => x.ReminderDateTime, userTask.ReminderDateTime)
                    .SetProperty(x => x.AssignedTo, userTask.AssignedTo)
                    .SetProperty(x => x.UserTaskPriority, userTask.UserTaskPriority)
                    .SetProperty(x => x.UserTaskStatus, userTask.UserTaskStatus), cancellationToken);

            if(rowsAffected == 0)
            {
                throw new EntityConcurrencyException()
                {
                    EntityName = nameof(UserTask),
                    EntityId = userTask.Id,
                };
            }

            return userTask;
        }
    }
}