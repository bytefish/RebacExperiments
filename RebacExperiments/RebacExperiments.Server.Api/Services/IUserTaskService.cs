// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using RebacExperiments.Server.Api.Infrastructure.Database;
using RebacExperiments.Server.Api.Models;

namespace RebacExperiments.Server.Api.Services
{
    /// <summary>
    /// An <see cref="IUserTaskService"/> is responsible for authorized access to a <see cref="UserTask"/>.
    /// </summary>
    public interface IUserTaskService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="userTaskId"></param>
        /// <param name="currentUserId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<UserTask> GetUserTaskByIdAsync(ApplicationDbContext context, int userTaskId, int currentUserId, CancellationToken cancellationToken);
    }
}
