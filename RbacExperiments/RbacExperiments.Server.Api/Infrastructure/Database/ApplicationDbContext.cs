using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualBasic;
using RbacExperiments.Server.Api.Models;

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
        /// Gets or sets the UserTasks.
        /// </summary>
        public DbSet<Team> Teams { get; set; } = null!;

        /// <summary>
        /// Gets or sets the UserTasks.
        /// </summary>
        public DbSet<Organization> Organizations { get; set; } = null!;

        /// <summary>
        /// Gets or sets the UserTasks.
        /// </summary>
        public DbSet<RelationTuple> RelationTuples { get; set; } = null!;

        /// <summary>
        /// List Objects.
        /// </summary>
        /// <param name="objectNamespace"></param>
        /// <param name="objectRelation"></param>
        /// <param name="subjectNamespace"></param>
        /// <param name="subjectKey"></param>
        /// <returns></returns>
        public IQueryable<RelationTuple> ListObjects(string objectNamespace, string objectRelation, string subjectNamespace, int subjectKey)
            => FromExpression(() => ListObjects(objectNamespace, objectRelation, subjectNamespace, subjectKey));

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Add ListObjects Function:
            modelBuilder
                .HasDbFunction(
                    methodInfo: typeof(ApplicationDbContext).GetMethod(nameof(ListObjects), new[] { typeof(string), typeof(string), typeof(string), typeof(int) })!,
                    builderAction: builder => builder
                        .HasSchema("Identity")
                        .HasName("tvf_RelationTuples_ListObjects"));

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

            modelBuilder.Entity<Team>(entity =>
            {
                entity.ToTable("Team", "Application");

                entity.HasKey(e => e.TeamId);

                entity.Property(x => x.TeamId)
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
            
            modelBuilder.Entity<RelationTuple>(entity =>
            {
                entity.ToTable("RelationTuple", "Identity");

                entity.HasKey(e => e.RelationTupleId);

                entity.Property(x => x.RelationTupleId)
                    .HasColumnType("INT")
                    .HasColumnName("RelationTupleID")
                    .HasDefaultValueSql("NEXT VALUE FOR [Identity].[sq_RelationTuple]")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.ObjectKey)
                    .HasColumnType("INT")
                    .HasColumnName("ObjectKey")
                    .IsRequired(true);

                entity.Property(e => e.ObjectNamespace)
                    .HasColumnType("NVARCHAR(50)")
                    .HasColumnName("ObjectNamespace")
                    .IsRequired(true)
                    .HasMaxLength(50);

                entity.Property(e => e.ObjectRelation)
                    .HasColumnType("NVARCHAR(50)")
                    .HasColumnName("ObjectRelation")
                    .IsRequired(true)
                    .HasMaxLength(50);

                entity.Property(e => e.SubjectKey)
                    .HasColumnType("INT")
                    .HasColumnName("SubjectKey")
                    .IsRequired(true);

                entity.Property(e => e.SubjectNamespace)
                    .HasColumnType("NVARCHAR(50)")
                    .HasColumnName("SubjectNamespace")
                    .IsRequired(true)
                    .HasMaxLength(50);

                entity.Property(e => e.SubjectRelation)
                    .HasColumnType("NVARCHAR(50)")
                    .HasColumnName("SubjectRelation")
                    .IsRequired(false)
                    .HasMaxLength(50);

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
