namespace Auth.DAL.Entities
{
    public class ExternalUser
    {
        public int Id { get; set; }

        public LocalUser User { get; set; }
    }
}