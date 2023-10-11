// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace RebacExperiments.Server.Api.Models
{
    public class RelationTuple
    {
        /// <summary>
        /// Gets or sets the RelationTupleId.
        /// </summary>
        public int RelationTupleId { get; set; }

        /// <summary>
        /// Gets or sets the ObjectKey.
        /// </summary>
        public int ObjectKey { get; set; }

        /// <summary>
        /// Gets or sets the ObjectNamespace.
        /// </summary>
        public required string ObjectNamespace { get; set; }

        /// <summary>
        /// Gets or sets the ObjectRelation.
        /// </summary>
        public required string ObjectRelation { get; set; }

        /// <summary>
        /// Gets or sets the SubjectKey.
        /// </summary>
        public required int SubjectKey { get; set; }

        /// <summary>
        /// Gets or sets the SubjectNamespace.
        /// </summary>
        public string? SubjectNamespace { get; set; }

        /// <summary>
        /// Gets or sets the SubjectRelation.
        /// </summary>
        public string? SubjectRelation { get; set; }

        /// <summary>
        /// Gets or sets the RowVersion.
        /// </summary>
        public byte[]? RowVersion { get; set; }

        /// <summary>
        /// Gets or sets the LastEditedBy.
        /// </summary>
        public int LastEditedBy { get; set; }

        /// <summary>
        /// Gets or sets the ValidFrom.
        /// </summary>
        public DateTime? ValidFrom { get; set; }

        /// <summary>
        /// Gets or sets the ValidTo.
        /// </summary>
        public DateTime? ValidTo { get; set; }
    }
}
