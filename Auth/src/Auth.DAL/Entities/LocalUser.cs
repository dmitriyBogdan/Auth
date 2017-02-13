using System.Collections.Generic;

namespace Auth.DAL.Entities
{
    public class LocalUser
    {
        public int Id { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string Salt { get; set; }

        public List<LocalUserClaim> LocalUserClaims { get; set; }

        public List<ExternalUser> ExternalUsers { get; set; }
    }
}