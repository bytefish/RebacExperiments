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
    public class ListUserObjectsTests : TransactionalTestBase
    {
        /// <summary>
        /// In this test we create a <see cref="User"/> (user) and a <see cref="UserTask"/> (task). The 'user' is member of 
        /// a <see cref="Team"/> (team). The 'user' is also a member of an <see cref="Organization"/> (oganization). Members 
        /// of the 'organization' are viewers of the 'task' and members of the 'team' are owners of the 'task'.
        /// 
        /// The Relationship-Table is given below.
        /// 
        /// ObjectKey           |  ObjectNamespace  |   ObjectRelation  |   SubjectKey          |   SubjectNamespace    |   SubjectRelation
        /// --------------------|-------------------|-------------------|-----------------------|-----------------------|-------------------
        /// :team.id:           |   Team            |       member      |   :user.id:           |       User            |   NULL
        /// :organization.id:   |   Organization    |       member      |   :user.id:           |       User            |   NULL
        /// :task.id:           |   UserTask        |       viewer      |   :organization.id:   |       Organization    |   member
        /// :task.id:           |   UserTask        |       owner       |   :team.id:           |       Team            |   member
        /// </summary>
        [Test]
        public async Task ListUserObjects_OneUserTaskAssignedThroughOrganizationAndTeam()
        {
            // Arrange
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
                Description = "Organization for Unit Test",
                LastEditedBy = user.Id
            };

            await _applicationDbContext.AddAsync(organization);
            await _applicationDbContext.SaveChangesAsync();

            var team = new Team
            {
                Name = "Test-Team",
                Description = "Team for Unit Test",
                LastEditedBy = user.Id
            };

            await _applicationDbContext.AddAsync(team);
            await _applicationDbContext.SaveChangesAsync();

            var task = new UserTask
            {
                Title = "Test-Task",
                Description = "My Test-Task",
                LastEditedBy = user.Id,
                UserTaskPriority = UserTaskPriorityEnum.High,
                UserTaskStatus = UserTaskStatusEnum.InProgress
            };

            await _applicationDbContext.AddAsync(task);
            await _applicationDbContext.SaveChangesAsync();

            await _applicationDbContext.AddRelationshipAsync(team, Relations.Member, user, null, user.Id);
            await _applicationDbContext.AddRelationshipAsync(organization, Relations.Member, user, null, user.Id);
            await _applicationDbContext.AddRelationshipAsync(task, Relations.Viewer, organization, Relations.Member, user.Id);
            await _applicationDbContext.AddRelationshipAsync(task, Relations.Owner, team, Relations.Member, user.Id);
            await _applicationDbContext.SaveChangesAsync();

            // Act
            var userTasks_Owner = _applicationDbContext
                .ListUserObjects<UserTask>(user.Id, Relations.Owner)
                .AsNoTracking()
                .ToList();

            var userTasks_Viewer = _applicationDbContext
                .ListUserObjects<UserTask>(user.Id, Relations.Viewer)
                .AsNoTracking()
                .ToList();

            var team_Member = _applicationDbContext
                .ListUserObjects<Team>(user.Id, Relations.Member)
                .AsNoTracking()
                .ToList();

            var organization_Member = _applicationDbContext
                .ListUserObjects<Organization>(user.Id, Relations.Member)
                .AsNoTracking()
                .ToList();

            // Assert
            Assert.AreEqual(1, userTasks_Owner.Count);
            Assert.AreEqual(task.Id, userTasks_Owner[0].Id);

            Assert.AreEqual(1, userTasks_Viewer.Count);
            Assert.AreEqual(task.Id, userTasks_Viewer[0].Id);

            Assert.AreEqual(1, team_Member.Count);
            Assert.AreEqual(team.Id, team_Member[0].Id);

            Assert.AreEqual(1, organization_Member.Count);
            Assert.AreEqual(organization.Id, organization_Member[0].Id);
        }

        /// <summary>
        /// In this test we create a <see cref="User"/> (user) and two  <see cref="UserTask"/> (task1, task2). The 'user' is 
        /// member of a <see cref="Team"/> (team). The 'user' is also a member of an <see cref="Organization"/> (oganization). Members 
        /// of the 'organization' are viewers of 'task1' and 'task2'. Members of the 'team' are owners of the 'task2'.
        /// 
        /// The Relationship-Table is given below.
        /// 
        /// ObjectKey           |  ObjectNamespace  |   ObjectRelation  |   SubjectKey          |   SubjectNamespace    |   SubjectRelation
        /// --------------------|-------------------|-------------------|-----------------------|-----------------------|-------------------
        /// :team.id:           |   Team            |       member      |   :user.id:           |       User            |   NULL
        /// :organization.id:   |   Organization    |       member      |   :user.id:           |       User            |   NULL
        /// :task1.id:          |   UserTask        |       viewer      |   :organization.id:   |       Organization    |   member
        /// :task2.id:          |   UserTask        |       viewer      |   :organization.id:   |       Organization    |   member
        /// :task2.id:          |   UserTask        |       owner       |   :team.id:           |       Team            |   member
        /// </summary>
        [Test]
        public async Task ListUserObjects_TwoUserTasksAssignedToOrganizationAndTeam()
        {
            // Arrange
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
                Description = "Organization for Unit Test",
                LastEditedBy = user.Id
            };

            await _applicationDbContext.AddAsync(organization);
            await _applicationDbContext.SaveChangesAsync();

            var team = new Team
            {
                Name = "Test-Team",
                Description = "Team for Unit Test",
                LastEditedBy = user.Id
            };

            await _applicationDbContext.AddAsync(team);
            await _applicationDbContext.SaveChangesAsync();

            var task_AssignedToUserThroughOrganizationMembership = new UserTask
            {
                Title = "Test-Task assigned through Organization Membership",
                Description = "A Test-Task assigned through Organization Membership",
                LastEditedBy = user.Id,
                UserTaskPriority = UserTaskPriorityEnum.High,
                UserTaskStatus = UserTaskStatusEnum.InProgress
            };

            await _applicationDbContext.AddAsync(task_AssignedToUserThroughOrganizationMembership);
            await _applicationDbContext.SaveChangesAsync();
            
            var task_AssignedToUserThroughTeamMembership = new UserTask
            {
                Title = "Test-Task assigned through Team Membership",
                Description = "A Test-Task assigned through Team Membership",
                LastEditedBy = user.Id,
                UserTaskPriority = UserTaskPriorityEnum.High,
                UserTaskStatus = UserTaskStatusEnum.InProgress
            };

            await _applicationDbContext.AddAsync(task_AssignedToUserThroughTeamMembership);
            await _applicationDbContext.SaveChangesAsync();

            await _applicationDbContext.AddRelationshipAsync(team, Relations.Member, user, null, user.Id);
            await _applicationDbContext.AddRelationshipAsync(organization, Relations.Member, user, null, user.Id);
            await _applicationDbContext.AddRelationshipAsync(task_AssignedToUserThroughOrganizationMembership, Relations.Viewer, organization, Relations.Member, user.Id);
            await _applicationDbContext.AddRelationshipAsync(task_AssignedToUserThroughTeamMembership, Relations.Viewer, organization, Relations.Member, user.Id);
            await _applicationDbContext.AddRelationshipAsync(task_AssignedToUserThroughTeamMembership, Relations.Owner, team, Relations.Member, user.Id);
            await _applicationDbContext.SaveChangesAsync();

            // Act
            var userTasks_Owner = _applicationDbContext
                .ListUserObjects<UserTask>(user.Id, Relations.Owner)
                .AsNoTracking()
                .ToList();

            var userTasks_Viewer = _applicationDbContext
                .ListUserObjects<UserTask>(user.Id, Relations.Viewer)
                .AsNoTracking()
                .ToList();

            // Assert
            Assert.AreEqual(1, userTasks_Owner.Count);
            Assert.AreEqual(task_AssignedToUserThroughTeamMembership.Id, userTasks_Owner[0].Id);

            Assert.AreEqual(2, userTasks_Viewer.Count);
            Assert.Contains(task_AssignedToUserThroughOrganizationMembership.Id, userTasks_Viewer.Select(x => x.Id).ToList());
            Assert.Contains(task_AssignedToUserThroughTeamMembership.Id, userTasks_Viewer.Select(x => x.Id).ToList());
        }
    }
}