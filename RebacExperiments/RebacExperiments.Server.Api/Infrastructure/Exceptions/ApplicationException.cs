// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace RebacExperiments.Server.Api.Infrastructure.Exceptions
{
    /// <summary>
    /// Base Exception for the Application.
    /// </summary>
    public abstract class ApplicationException : Exception
    {
        /// <summary>
        /// Gets the Error Code.
        /// </summary>
        public abstract string ErrorCode { get; }

        /// <summary>
        /// Gets the Error Message.
        /// </summary>
        public abstract string ErrorMessage { get; }

        protected ApplicationException(string? message, Exception? innerException)
            : base(message, innerException)
        {
        }
    }
}
