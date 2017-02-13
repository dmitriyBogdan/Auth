namespace Auth.DAL.Entities
{
    public class LocalUserClaim
    {
        public int LocalUserId { get; set; }

        public LocalUser LocalUser { get; set; }

        public int ClaimId { get; set; }

        public Claim Claim { get; set; }
    }
}