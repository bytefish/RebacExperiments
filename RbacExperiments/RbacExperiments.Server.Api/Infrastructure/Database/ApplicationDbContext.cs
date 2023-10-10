using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Hosting;
using RbacExperiments.Server.Api.Models;
using System;

namespace RbacExperiments.Server.Api.Infrastructure.Database
{
    /// <summary>
    /// A <see cref="DbContext"/> to query the database.
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="options">Options to configure the base <see cref="DbContext"/></param>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Gets or sets the UserTasks.
        /// </summary>
        public DbSet<UserTask> UserTasks { get; set; } = null!;

        /// <summary>
        /// Gets the Metadata of the Type <typeparamref name="TEntityType"/>.
        /// </summary>
        /// <typeparam name="TEntityType">Entity Type to get Data from</typeparam>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Thrown when Entity Metadata cannot be determined</exception>
        private (string EntityName, string SchemaQualifiedTableName, string PrimaryKeyName) GetEntityMetadata<TEntityType>()
        {
            var entityType = typeof(TEntityType);

            var modelEntityType = Model.FindEntityType(entityType);

            if(modelEntityType == null)
            {
                throw new InvalidOperationException($"Cannot find Mapping for Entity Type '{entityType}");
            }

            var schemaQualifiedTableName = modelEntityType.GetSchemaQualifiedTableName();
            
            if(schemaQualifiedTableName == null)
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

        /// <summary>
        /// Returns all <typeparamref name="TEntityType"/> for a given <paramref name="userId"/> and <paramref name="relation"/>.
        /// </summary>
        /// <param name="userId">UserID</param>
        /// <param name="relation">Relation between the User and UserTask</param>
        /// <returns>All <typeparamref name="TEntityType"/> the user is related to</returns>
        public IQueryable<TEntityType> GetEntitiesByUserAndRelation<TEntityType>(int userId, string relation)
            where TEntityType : class
        {
            // Get the Metadata (Entity Name, Fully Qualified Table Name, PrimaryKeyName, ...
            var entityMetadata = GetEntityMetadata<TEntityType>();
            
            // Looks a little bit horrible, but it works, so ...
            var sql = @$"SELECT {entityMetadata.SchemaQualifiedTableName}.*
                         FROM [Identity].tvf_RelationTuples_ListObjects('{entityMetadata.EntityName}', '{relation}', 'User', {userId}) as ""user_objects""
			                INNER JOIN {entityMetadata.SchemaQualifiedTableName} ON {entityMetadata.SchemaQualifiedTableName}.{entityMetadata.PrimaryKeyName} = ""user_objects"".ObjectKey";

            return Set<TEntityType>()
                .FromSqlRaw(sql)
                .AsNoTracking(); // Should we really use NoTracking here?
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasSequence<int>("sq_UserTask", schema: "Application")
                .StartsAt(38187)
                .IncrementsBy(1);

            modelBuilder.Entity<UserTask>(entity =>
            {
                entity.ToTable("UserTask", "Application");

                entity.HasKey(e => e.UserTaskId);

                entity.Property(x => x.UserTaskId)
                    .HasColumnType("INT")
                    .HasColumnName("UserTaskID")
                    .HasDefaultValueSql("NEXT VALUE FOR [Application].[sq_UserTask]")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Title)
                    .HasColumnType("NVARCHAR(50)")
                    .HasColumnName("Title")
                    .IsRequired(true)
                    .HasMaxLength(50);
                
                entity.Property(e => e.Description)
                    .HasColumnType("NVARCHAR(2000)")
                    .HasColumnName("Description")
                    .IsRequired(true)
                    .HasMaxLength(2000);
                
                entity.Property(e => e.DueDateTime)
                    .HasColumnType("DATETIME2(7)")
                    .HasColumnName("DueDateTime")
                    .IsRequired(false);

                entity.Property(e => e.ReminderDateTime)
                    .HasColumnType("DATETIME2(7)")
                    .HasColumnName("ReminderDateTime")
                    .IsRequired(false);
                
                entity.Property(e => e.CompletedDateTime)
                    .HasColumnType("DATETIME2(7)")
                    .HasColumnName("CompletedDateTime")
                    .IsRequired(false);

                entity.Property(e => e.AssignedTo)
                    .HasColumnType("INT")
                    .HasColumnName("AssignedTo")
                    .IsRequired(false);

                entity.Property(e => e.UserTaskStatus)
                    .HasColumnType("INT")
                    .HasColumnName("UserTaskStatusID")
                    .HasConversion(v => (int) v, v => (UserTaskStatusEnum)v)
                    .IsRequired(true);

                entity.Property(e => e.UserTaskPriority)
                    .HasColumnType("INT")
                    .HasColumnName("UserTaskPriorityID")
                    .HasConversion(v => (int)v, v => (UserTaskPriorityEnum)v)
                    .IsRequired(true);

                entity.Property(e => e.ValidFrom)
                    .HasColumnType("DATETIME2(7)")
                    .HasColumnName("ValidFrom")
                    .IsRequired(false)
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.ValidTo)
                    .HasColumnType("DATETIME2(7)")
                    .HasColumnName("ValidTo")
                    .IsRequired(false)
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.LastEditedBy)
                    .HasColumnType("INT")
                    .HasColumnName("LastEditedBy")
                    .IsRequired(true);
            });

            modelBuilder.HasSequence<int>("sq_Organization", schema: "Application")
                .StartsAt(38187)
                .IncrementsBy(1);

            modelBuilder.Entity<Organization>(entity =>
            {
                entity.ToTable("Organization", "Application");

                entity.HasKey(e => e.OrganizationId);

                entity.Property(x => x.OrganizationId)
                    .HasColumnType("INT")
                    .HasColumnName("OrganizationID")
                    .HasDefaultValueSql("NEXT VALUE FOR [Application].[sq_Organization]")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Name)
                    .HasColumnType("NVARCHAR(255)")
                    .HasColumnName("Name")
                    .IsRequired(true)
                    .HasMaxLength(255);

                entity.Property(e => e.Description)
                    .HasColumnType("NVARCHAR(2000)")
                    .HasColumnName("Description")
                    .IsRequired(true)
                    .HasMaxLength(2000);

                entity.Property(e => e.ValidFrom)
                    .HasColumnType("DATETIME2(7)")
                    .HasColumnName("ValidFrom")
                    .IsRequired(false)
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.ValidTo)
                    .HasColumnType("DATETIME2(7)")
                    .HasColumnName("ValidTo")
                    .IsRequired(false)
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.LastEditedBy)
                    .HasColumnType("INT")
                    .HasColumnName("LastEditedBy")
                    .IsRequired(true);
            });

            modelBuilder.Entity<Organization>(entity =>
            {
                entity.ToTable("Team", "Application");

                entity.HasKey(e => e.OrganizationId);

                entity.Property(x => x.OrganizationId)
                    .HasColumnType("INT")
                    .HasColumnName("TeamID")
                    .HasDefaultValueSql("NEXT VALUE FOR [Application].[sq_Team]")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Name)
                    .HasColumnType("NVARCHAR(255)")
                    .HasColumnName("Name")
                    .IsRequired(true)
                    .HasMaxLength(255);

                entity.Property(e => e.Description)
                    .HasColumnType("NVARCHAR(2000)")
                    .HasColumnName("Description")
                    .IsRequired(true)
                    .HasMaxLength(2000);

                entity.Property(e => e.ValidFrom)
                    .HasColumnType("DATETIME2(7)")
                    .HasColumnName("ValidFrom")
                    .IsRequired(false)
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.ValidTo)
                    .HasColumnType("DATETIME2(7)")
                    .HasColumnName("ValidTo")
                    .IsRequired(false)
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.LastEditedBy)
                    .HasColumnType("INT")
                    .HasColumnName("LastEditedBy")
                    .IsRequired(true);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
