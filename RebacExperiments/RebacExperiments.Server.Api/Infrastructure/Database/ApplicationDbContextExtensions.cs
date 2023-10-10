using Microsoft.EntityFrameworkCore;
using RebacExperiments.Server.Api.Models;

namespace RebacExperiments.Server.Api.Infrastructure.Database
{
    public static class ApplicationDbContextExtensions
    {
        /// <summary>
        /// Returns all <typeparamref name="TObjectType"/> for a given <typeparamref name="TSubjectType"/> and <paramref name="relation"/>.
        /// </summary>
        /// <param name="subjectId">Subject Key to resolve</param>
        /// <param name="relation">Relation between the Object and Subject</param>
        /// <returns>All <typeparamref name="TEntityType"/> the user is related to</returns>
        public static IQueryable<TObjectType> ListObjects<TObjectType, TSubjectType>(this ApplicationDbContext context, int subjectId, string relation)
            where TObjectType : class
            where TSubjectType : class
        {
            return
                from entity in context.Set<TObjectType>()
                join objects in context.ListObjects(typeof(TObjectType).Name, relation, typeof(TSubjectType).Name, subjectId)
                    on EF.Property<int>(entity, $"{typeof(TObjectType).Name}Id") equals objects.ObjectKey
                select entity;
        }

        /// <summary>
        /// Returns all <typeparamref name="TEntityType"/> for a given <paramref name="userId"/> and <paramref name="relation"/>.
        /// </summary>
        /// <param name="userId">UserID</param>
        /// <param name="relation">Relation between the User and UserTask</param>
        /// <returns>All <typeparamref name="TEntityType"/> the user is related to</returns>
        public static IQueryable<TEntityType> ListUserObjects<TEntityType>(this ApplicationDbContext context, int userId, string relation)
            where TEntityType : class
        {
            return context.ListObjects<TEntityType, User>(userId, relation);
        }
    }
}
