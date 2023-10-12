namespace RebacExperiments.Server.Api.Infrastructure.Services
{
    /// <summary>
    /// Class representing error details.
    /// </summary>
    public class ServiceErrorDetail
    {
        /// <summary>
        /// Gets or sets the error code.
        /// </summary>
        public required int ErrorCode { get; set; }

        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        public required string Message { get; set; }

        /// <summary>
        /// Gets or sets an optional property name.
        /// </summary>
        public string? PropertyName { get; set; }

        /// <summary>
        /// Properties to be written for the inner error.
        /// </summary>
        public IDictionary<string, object?>? AdditionalProperties { get; set; }
    }
}
