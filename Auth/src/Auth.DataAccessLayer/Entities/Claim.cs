using System.Collections.Generic;

namespace Auth.DataAccessLayer.Entities
{
    public class Claim
    {
        public int Id { get; set; }

        public string Type { get; set; }

        public string Value { get; set; }

        public List<LocalUserClaim> LocalUserClaims { get; set; }
    }
}