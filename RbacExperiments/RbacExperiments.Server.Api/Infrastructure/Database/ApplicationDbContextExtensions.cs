using Microsoft.EntityFrameworkCore;
using RbacExperiments.Server.Api.Models;

namespace RbacExperiments.Server.Api.Infrastructure.Database
{
    public static class ApplicationDbContextExtensions
    {
        /// <summary>
        /// Returns all <typeparamref name="TEntityType"/> for a given <paramref name="userId"/> and <paramref name="relation"/>.
        /// </summary>
        /// <param name="userId">UserID</param>
        /// <param name="relation">Relation between the User and UserTask</param>
        /// <returns>All <typeparamref name="TEntityType"/> the user is related to</returns>
        public static IQueryable<TEntityType> GetEntitiesByUserAndRelation<TEntityType>(this ApplicationDbContext context, int userId, string relation)
            where TEntityType : class
        {
            string entityName = typeof(TEntityType).Name;
            string entityPrimaryKeyColumnName = $"{entityName}Id";

            return 
                from entity in context.Set<TEntityType>()
                join objects in context.ListObjects(typeof(TEntityType).Name, relation, typeof(User).Name, userId)
                    on EF.Property<int>(entity, entityPrimaryKeyColumnName) equals objects.ObjectKey
                select entity;
        }
    }
}
