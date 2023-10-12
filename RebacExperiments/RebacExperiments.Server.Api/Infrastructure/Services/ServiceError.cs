namespace RebacExperiments.Server.Api.Infrastructure.Services
{
    /// <summary>
    /// All errors contained in ServiceResult objects must return an error of this type Error codes allow the caller to 
    /// easily identify the received error and take action. Error messages allow the caller to easily show error messages 
    /// to the end user.
    /// </summary>
    [Serializable]
    public class ServiceError
    {
        /// <summary>
        /// Machine readable error code
        /// </summary>
        public required int ErrorCode { get; set; }

        /// <summary>
        /// Human readable error message
        /// </summary>
        public required string Message { get; set; }

        /// <summary>
        /// Gets or sets error details.
        /// </summary>
        public List<ServiceErrorDetail>? Details { get; set; }

        /// <summary>
        /// Gets or sets an inner error for debugging.
        /// </summary>
        public ServiceErrorInnerError? InnerError { get; set; }
    }

}
