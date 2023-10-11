﻿// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using RebacExperiments.Server.Api.Models;

namespace RebacExperiments.Server.Api.Infrastructure.Database
{
    /// <summary>
    /// Extensions on the <see cref="ApplicationDbContext"/> to allow Relationship-based ACL.
    /// </summary>
    public static class ApplicationDbContextExtensions
    {
        /// <summary>
        /// Returns all <typeparamref name="TObjectType"/> for a given <typeparamref name="TSubjectType"/> and <paramref name="relation"/>.
        /// </summary>
        /// <param name="subjectId">Subject Key to resolve</param>
        /// <param name="relation">Relation between the Object and Subject</param>
        /// <returns>All <typeparamref name="TEntityType"/> the user is related to</returns>
        public static IQueryable<TObjectType> ListObjects<TObjectType, TSubjectType>(this ApplicationDbContext context, int subjectId, string relation)
            where TObjectType : Entity
            where TSubjectType : Entity
        {
            return
                from entity in context.Set<TObjectType>()
                join objects in context.ListObjects(typeof(TObjectType).Name, relation, typeof(TSubjectType).Name, subjectId)
                    on entity.Id equals objects.ObjectKey
                select entity;
        }

        /// <summary>
        /// Returns all <typeparamref name="TEntityType"/> for a given <paramref name="userId"/> and <paramref name="relation"/>.
        /// </summary>
        /// <param name="userId">UserID</param>
        /// <param name="relation">Relation between the User and UserTask</param>
        /// <returns>All <typeparamref name="TEntityType"/> the user is related to</returns>
        public static IQueryable<TEntityType> ListUserObjects<TEntityType>(this ApplicationDbContext context, int userId, string relation)
            where TEntityType : Entity
        {
            return context.ListObjects<TEntityType, User>(userId, relation);
        }

        /// <summary>
        /// Creates a Relationship between a <typeparamref name="TObjectType"/> and a <typeparamref name="TSubjectType"/>.
        /// </summary>
        /// <typeparam name="TObjectType">Type of the Object</typeparam>
        /// <typeparam name="TSubjectType">Type of the Subject</typeparam>
        /// <param name="context">DbContext</param>
        /// <param name="object">Object Entity</param>
        /// <param name="relation">Relation between Object and Subject</param>
        /// <param name="subject">Subject Entity</param>
        /// <param name="subjectRelation">Relation to the Subject</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns></returns>
        public static async Task CreateRelationshipAsync<TObjectType, TSubjectType>(this ApplicationDbContext context, TObjectType @object, string relation, TSubjectType subject, string? subjectRelation = null, CancellationToken cancellationToken = default)
            where TObjectType : Entity
            where TSubjectType : Entity
        {
            var relationTuple = new RelationTuple
            {
                ObjectNamespace = typeof(TObjectType).Name,
                ObjectKey = @object.Id,
                ObjectRelation = relation,
                SubjectNamespace = typeof(TSubjectType).Name,
                SubjectKey = subject.Id,
                SubjectRelation = subjectRelation
            };

            await context.Set<RelationTuple>().AddAsync(relationTuple, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}