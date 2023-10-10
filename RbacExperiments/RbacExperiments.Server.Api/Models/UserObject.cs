namespace RbacExperiments.Server.Api.Models
{
    public class UserObject
    {
        public required string Entity { get; set; }

        public required string Relation { get; set; }

        public required int ObjectId { get; set; }

        public required int UserId { get; set; }
    }
}
