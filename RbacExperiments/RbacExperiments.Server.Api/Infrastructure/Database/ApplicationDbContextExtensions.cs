using Microsoft.EntityFrameworkCore;

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
            // Get the Metadata (Entity Name, Fully Qualified Table Name, PrimaryKeyName, ...
            var entityMetadata = GetEntityMetadata<TEntityType>(context);

            // Looks a little bit horrible, but it works, so ...
            var sql = @$"SELECT {entityMetadata.SchemaQualifiedTableName}.*
                         FROM [Identity].tvf_RelationTuples_ListObjects('{entityMetadata.EntityName}', '{relation}', 'User', {userId}) as ""user_objects""
			                INNER JOIN {entityMetadata.SchemaQualifiedTableName} ON {entityMetadata.SchemaQualifiedTableName}.{entityMetadata.PrimaryKeyName} = ""user_objects"".ObjectKey";

            return context.Set<TEntityType>()
                .FromSqlRaw(sql)
                .AsNoTracking(); // Should we really use NoTracking here?
        }

        /// <summary>
        /// Gets the Metadata of the Type <typeparamref name="TEntityType"/>.
        /// </summary>
        /// <typeparam name="TEntityType">Entity Type to get Data from</typeparam>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Thrown when Entity Metadata cannot be determined</exception>
        private static (string EntityName, string SchemaQualifiedTableName, string PrimaryKeyName) GetEntityMetadata<TEntityType>(ApplicationDbContext context)
        {
            var entityType = typeof(TEntityType);

            var modelEntityType = context.Model.FindEntityType(entityType);

            if (modelEntityType == null)
            {
                throw new InvalidOperationException($"Cannot find Mapping for Entity Type '{entityType}");
            }

            var schemaQualifiedTableName = modelEntityType.GetSchemaQualifiedTableName();

            if (schemaQualifiedTableName == null)
            {
                throw new InvalidOperationException($"Cannot find Table for Entity Type '{entityType}");
            }

            var primaryKeyName = modelEntityType.FindPrimaryKey()
                ?.Properties
                .Select(x => x.GetColumnName())
                .FirstOrDefault();

            if (primaryKeyName == null)
            {
                throw new InvalidOperationException($"Cannot find Primary Key for Entity Type '{entityType}");
            }

            return (entityType.Name, schemaQualifiedTableName, primaryKeyName);
        }

    }
}
