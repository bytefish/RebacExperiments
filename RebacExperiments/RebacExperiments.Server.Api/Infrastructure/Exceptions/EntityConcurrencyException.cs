﻿using RebacExperiments.Server.Api.Infrastructure.Errors;

namespace RebacExperiments.Server.Api.Infrastructure.Exceptions
{
    public class EntityConcurrencyException : ApplicationException
    {
        /// <summary>
        /// Gets or sets an error code.
        /// </summary>
        public override string ErrorCode => ErrorCodes.EntityConcurrencyFailure;

        /// <summary>
        /// Gets or sets an error code.
        /// </summary>
        public override string ErrorMessage => $"EntityConcurrencyFailure (Entity = {EntityName}, EntityID = {EntityId})";

        /// <summary>
        /// Gets or sets the Entity Name.
        /// </summary>
        public required string EntityName { get; set; }

        /// <summary>
        /// Gets or sets the EntityId.
        /// </summary>
        public required int EntityId { get; set; }

        /// <summary>
        /// Creates a new <see cref="EntityNotFoundException"/>.
        /// </summary>
        /// <param name="message">Error Message</param>
        /// <param name="innerException">Reference to the Inner Exception</param>
        public EntityConcurrencyException(string? message = null, Exception? innerException = null)
            : base(message, innerException)
        {
        }
    }
}
