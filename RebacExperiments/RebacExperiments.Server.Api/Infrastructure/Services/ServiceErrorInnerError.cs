namespace RebacExperiments.Server.Api.Infrastructure.Services
{
    /// <summary>
    /// Class representing implementation specific debugging information to help determine the cause of the error.
    /// </summary>
    public class ServiceErrorInnerError
    {
        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        public string Message { get; set; } = null!;

        /// <summary>
        /// Gets or sets the Exception of this error.
        /// </summary>
        public Exception? Exception { get; set; }

        /// <summary>
        /// Properties to be written for the inner error.
        /// </summary>
        public IDictionary<string, object?>? AdditionalProperties { get; set; }

        /// <summary>
        /// Gets or sets the nested implementation specific debugging information.
        /// </summary>
        public ServiceErrorInnerError? InnerError { get; set; }
    }
}
