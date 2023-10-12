using Microsoft.AspNetCore.Mvc;
using RebacExperiments.Server.Api.Infrastructure.Resources;
using System.ComponentModel.DataAnnotations;

namespace RebacExperiments.Server.Api.Dto
{
    [ModelMetadataType(typeof(CredentialsDtoMetadata))]
    public class CredentialsDto
    {
        public string Username { get; set; } = null!;

        public string Password { get; set; } = null!;

        public bool RememberMe { get; set; }
    }

    public class CredentialsDtoMetadata
    {
        [Required(ErrorMessageResourceName = nameof(ErrorMessages.Validation_Required), ErrorMessageResourceType = typeof(ErrorMessages))]
        [StringLength(255, ErrorMessageResourceName = nameof(ErrorMessages.Validation_StringLength), ErrorMessageResourceType = typeof(ErrorMessages))]
        public string Username { get; set; } = null!;

        [Required(ErrorMessageResourceName = nameof(ErrorMessages.Validation_Required), ErrorMessageResourceType = typeof(ErrorMessages))]
        [StringLength(255, ErrorMessageResourceName = nameof(ErrorMessages.Validation_StringLength), ErrorMessageResourceType = typeof(ErrorMessages))]
        public string Password { get; set; } = null!;
    }
}
