using EfCoreAudit.Tests;
using RbacExperiments.Server.Api.Infrastructure.Constants;
using RbacExperiments.Server.Api.Infrastructure.Database;
using RbacExperiments.Server.Api.Models;
using System.Linq;

namespace RbacExperiments.Server.Api.Tests
{
    public class Tests : TransactionalTestBase
    {
        [Test]
        public void TestUserObjects()
        {
            var query1 = _applicationDbContext.GetEntitiesByUserAndRelation<UserTask>(3, Relations.Viewer);
            var query2 = _applicationDbContext.GetEntitiesByUserAndRelation<UserTask>(3, Relations.Owner);

            var result = query1.Union(query2).ToList();
        }

        [Test]
        public void GetOrganizationObjects()
        {
            var query1 = _applicationDbContext.GetEntitiesByUserAndRelation<Organization>(2, Relations.Member);

            var result = query1.ToList();
        }
    }
}