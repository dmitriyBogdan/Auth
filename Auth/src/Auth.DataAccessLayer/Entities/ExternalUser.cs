namespace Auth.DataAccessLayer.Entities
{
    public class ExternalUser
    {
        public int Id { get; set; }

        public LocalUser User { get; set; }
    }
}