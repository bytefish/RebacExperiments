using EfCoreAudit.Tests;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using RbacExperiments.Server.Api.Infrastructure.Constants;
using RbacExperiments.Server.Api.Models;
using System.Linq;
using System.Threading.Tasks;

namespace RbacExperiments.Server.Api.Tests
{
    public class Tests : TransactionalTestBase
    {
        [Test]
        public void TestUserObjects()
        {
            var query1 = _applicationDbContext.GetEntitiesByRelation<UserTask>(3, Relations.Viewer);
            var query2 = _applicationDbContext.GetEntitiesByRelation<UserTask>(3, Relations.Owner);

            var result = query1.Union(query2).ToList();
        }
    }
}