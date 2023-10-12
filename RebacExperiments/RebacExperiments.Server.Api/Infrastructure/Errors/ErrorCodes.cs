namespace RebacExperiments.Server.Api.Infrastructure.Errors
{
    public static class ErrorCodes
    {
        /// <summary>
        /// General Authentication Error.
        /// </summary>
        public const int AuthenticationFailed = 70000;

        /// <summary>
        /// Entity has not been found.
        /// </summary>
        public const int EntityNotFound = 70001;

        /// <summary>
        /// Entity has not been found.
        /// </summary>
        public const int EntityUnauthorized = 70002;
    }
}
