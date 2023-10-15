namespace RebacExperiments.Server.Api.Infrastructure.Errors
{
    /// <summary>
    /// Error Codes used in the Application.
    /// </summary>
    public static class ErrorCodes
    {
        /// <summary>
        /// General Authentication Error.
        /// </summary>
        public const string AuthenticationFailed = "Auth:000001";

        /// <summary>
        /// Entity has not been found.
        /// </summary>
        public const string EntityNotFound = "Entity:000001";

        /// <summary>
        /// Access to Entity has been unauthorized.
        /// </summary>
        public const string EntityUnauthorized = "Entity:000002";
    }
}
