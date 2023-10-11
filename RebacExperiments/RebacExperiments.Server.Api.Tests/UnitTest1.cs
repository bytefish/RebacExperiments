// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using RebacExperiments.Server.Api.Infrastructure.Constants;
using RebacExperiments.Server.Api.Infrastructure.Database;
using RebacExperiments.Server.Api.Models;
using System.Linq;
using System.Threading.Tasks;

namespace RebacExperiments.Server.Api.Tests
{
    public class Tests : TransactionalTestBase
    {

        [Test]
        public async Task CheckRelationships()
        {
            // Prepare
            var user = new User
            {
                FullName = "Test-User",
                PreferredName = "Test-User",
                IsPermittedToLogon = false,
                LastEditedBy = 1,
                LogonName = "test-user@test-user.localhost"
            };

            await _applicationDbContext.AddAsync(user);
            await _applicationDbContext.SaveChangesAsync();

            var organization = new Organization
            {
                Name = "Test-Organization",
                Description = "Organization for Unit Test"
            };

            await _applicationDbContext.AddAsync(organization);
            await _applicationDbContext.SaveChangesAsync();

            var team = new Team
            {
                Name = "Test-Team",
                Description = "Team for Unit Test"
            };

            await _applicationDbContext.AddAsync(team);
            await _applicationDbContext.SaveChangesAsync();

            var task = new UserTask
            {
                Title = "Test-Task",
                Description = "My Test-Task",
                LastEditedBy = 1,
                UserTaskPriority = UserTaskPriorityEnum.High,
                UserTaskStatus = UserTaskStatusEnum.InProgress
            };

            await _applicationDbContext.AddAsync(task);
            await _applicationDbContext.SaveChangesAsync();

            await _applicationDbContext.AddRelationshipAsync(team, Relations.Member, organization);
            await _applicationDbContext.AddRelationshipAsync(user, Relations.Member, team);
            await _applicationDbContext.AddRelationshipAsync(task, Relations.Viewer, organization, Relations.Member);
            await _applicationDbContext.AddRelationshipAsync(task, Relations.Owner, team, Relations.Member);
            await _applicationDbContext.SaveChangesAsync();

            // Act
            var result = _applicationDbContext
                .ListUserObjects<UserTask>(user.Id, Relations.Viewer)
                .AsNoTracking();

            // Assert
        }

        [Test]
        public void TestUserObjects()
        {
            var query1 = _applicationDbContext
                .ListUserObjects<UserTask>(3, Relations.Viewer)
                .AsNoTracking();

            var query2 = _applicationDbContext
                .ListUserObjects<UserTask>(3, Relations.Owner)
                .AsNoTracking();

            var result = query1.Union(query2).ToList();
        }

        [Test]
        public void GetOrganizationObjects()
        {
            var result = _applicationDbContext
                .ListUserObjects<Organization>(2, Relations.Member)
                .AsNoTracking()
                .ToList();
        }
    }
}