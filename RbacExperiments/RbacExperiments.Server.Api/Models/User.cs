namespace RbacExperiments.Server.Api.Models
{
    public class User
    {
        /// <summary>
        /// Gets or sets the UserID.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets the FullName.
        /// </summary>
        public required string FullName { get; set; }

        /// <summary>
        /// Gets or sets the PreferredName.
        /// </summary>
        public required string PreferredName { get; set; }

        /// <summary>
        /// Gets or sets the IsPermittedToLogon.
        /// </summary>
        public int IsPermittedToLogon { get; set; }

        /// <summary>
        /// Gets or sets the LogonName.
        /// </summary>
        public string? LogonName { get; set; }

        /// <summary>
        /// Gets or sets the HashedPassword.
        /// </summary>
        public string? HashedPassword { get; set; }

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
