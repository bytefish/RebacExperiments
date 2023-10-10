namespace RebacExperiments.Server.Api.Models
{
    public class Team
    {
        /// <summary>
        /// Gets or sets the OrganizationID.
        /// </summary>
        public int TeamId { get; set; }

        /// <summary>
        /// Gets or sets the Name.
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// Gets or sets the Description.
        /// </summary>
        public required string Description { get; set; }

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
