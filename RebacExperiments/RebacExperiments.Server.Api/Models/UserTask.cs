namespace RebacExperiments.Server.Api.Models
{
    /// <summary>
    /// A User Task.
    /// </summary>
    public class UserTask
    {
        /// <summary>
        /// Gets or sets the
        /// </summary>
        public int UserTaskId { get; set; }

        /// <summary>
        /// Gets or sets the Title.
        /// </summary>
        public required string Title { get; set; }

        /// <summary>
        /// Gets or sets the Description.
        /// </summary>
        public required string Description { get; set; }

        /// <summary>
        /// Gets or sets the date a task is due.
        /// </summary>
        public DateTime? DueDateTime { get; set; }

        /// <summary>
        /// Gets or sets the date a task should be reminded of.
        /// </summary>        
        public DateTime? ReminderDateTime { get; set; }

        /// <summary>
        /// Gets or sets the date a task has been completed.
        /// </summary>
        public DateTime? CompletedDateTime { get; set; }

        /// <summary>
        /// Gets or sets the user the task is assigned to.
        /// </summary>
        public int? AssignedTo { get; set; }

        /// <summary>
        /// Gets or sets the user the task priority.
        /// </summary>
        public UserTaskPriorityEnum UserTaskPriority { get; set; }

        /// <summary>
        /// Gets or sets the user the task status.
        /// </summary>
        public UserTaskStatusEnum UserTaskStatus { get; set; }

        /// <summary>
        /// Gets or sets the user the task row version.
        /// </summary>
        public byte[]? RowVersion { get; set; }

        /// <summary>
        /// Gets or sets the user, that made the latest modifications.
        /// </summary>
        public int LastEditedBy { get; set; }

        /// <summary>
        /// Gets or sets the Start Date for the row.
        /// </summary>
        public DateTime? ValidFrom { get; set; }

        /// <summary>
        /// Gets or sets the End Date for the row.
        /// </summary>
        public DateTime? ValidTo { get; set; }
    }
}
