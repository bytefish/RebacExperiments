using Microsoft.EntityFrameworkCore;
using RebacExperiments.Server.Api.Infrastructure.Constants;
using RebacExperiments.Server.Api.Infrastructure.Database;
using RebacExperiments.Server.Api.Models;
using System.Linq;

namespace RebacExperiments.Server.Api.Tests
{
    public class Tests : TransactionalTestBase
    {
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