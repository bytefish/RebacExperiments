// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;
using RebacExperiments.Server.Api.Infrastructure.Constants;
using RebacExperiments.Server.Api.Infrastructure.Database;
using RebacExperiments.Server.Api.Infrastructure.Exceptions;
using RebacExperiments.Server.Api.Infrastructure.Logging;
using RebacExperiments.Server.Api.Models;
using System.Threading.Tasks;

namespace RebacExperiments.Server.Api.Services
{
    public class UserTaskService : IUserTaskService
    {
        private readonly ILogger<UserTaskService> _logger;

        public UserTaskService(ILogger<UserTaskService> logger)
        {
            _logger = logger;
        }

        public async Task<UserTask> CreateUserTaskAsync(ApplicationDbContext context, UserTask userTask, int currentUserId, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            using (var transaction = await context.Database.BeginTransactionAsync(cancellationToken))
            {
                // Add the new Task, the HiLo Pattern automatically assigns a new Id using the HiLo Pattern
                await context.AddAsync(userTask, cancellationToken);

                // The User is Viewer and Owner of the Task
                await context.AddRelationshipAsync<UserTask, User>(userTask.Id, Relations.Viewer, currentUserId, null, currentUserId, cancellationToken);
                await context.AddRelationshipAsync<UserTask, User>(userTask.Id, Relations.Owner, currentUserId, null, currentUserId, cancellationToken);

                // We want the created task to be visible by all members of the organization the user is in
                var organizations = await context
                    .ListUserObjects<Organization>(currentUserId, Relations.Member)
                    .AsNoTracking()
                    .ToListAsync(cancellationToken);

                foreach (var organization in organizations)
                {
                    await context.AddRelationshipAsync<UserTask, Organization>(userTask.Id, Relations.Viewer, organization.Id, Relations.Member, currentUserId, cancellationToken);
                }

                await context.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);
            }

            return userTask;
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
                    .SetProperty(x => x.UserTaskStatus, userTask.UserTaskStatus)
                    .SetProperty(x => x.LastEditedBy, currentUserId), cancellationToken);

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


        public async Task DeleteUserTaskAsync(ApplicationDbContext context, UserTask userTask, int currentUserId, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            bool isAuthorized = await context.CheckUserObject<UserTask>(currentUserId, userTask.Id, Relations.Owner, cancellationToken);

            if (!isAuthorized)
            {
                throw new EntityUnauthorizedAccessException()
                {
                    EntityName = nameof(UserTask),
                    EntityId = userTask.Id,
                    UserId = currentUserId,
                };
            }

            using (var transaction = await context.Database.BeginTransactionAsync(cancellationToken))
            {
                // Start by deleting all References to the UserTask. This looks repetitive
                // and it is. We should probably pass in a list of relations to delete,
                // create a union and delete them in one go.
                await context
                    .ListObjects<Organization, UserTask>(userTask.Id, Relations.Viewer)
                    .ExecuteDeleteAsync(cancellationToken);

                await context
                    .ListObjects<Organization, UserTask>(userTask.Id, Relations.Owner)
                    .ExecuteDeleteAsync(cancellationToken);
                
                await context
                    .ListObjects<Team, UserTask>(userTask.Id, Relations.Viewer)
                    .ExecuteDeleteAsync(cancellationToken);

                await context
                    .ListObjects<Team, UserTask>(userTask.Id, Relations.Owner)
                    .ExecuteDeleteAsync(cancellationToken);

                await context
                    .ListObjects<User, UserTask>(userTask.Id, Relations.Viewer)
                    .ExecuteDeleteAsync(cancellationToken);

                await context
                    .ListObjects<User, UserTask>(userTask.Id, Relations.Owner)
                    .ExecuteDeleteAsync(cancellationToken);

                // After removing all possible references, delete the UserTask itself
                int rowsAffected = await context.UserTasks
                    .Where(t => t.Id == userTask.Id && t.RowVersion == userTask.RowVersion)
                    .ExecuteDeleteAsync(cancellationToken);

                // If no row was affected, I want to throw an Exception. Why? Because someone might
                // have edited the Task, while we were about to delete it. And removing it after
                // someone has concurrently been updating it, feels wrong ...
                if (rowsAffected == 0)
                {
                    throw new EntityConcurrencyException()
                    {
                        EntityName = nameof(UserTask),
                        EntityId = userTask.Id,
                    };
                }

                await transaction.CommitAsync(cancellationToken);
            }
        }
    }
}