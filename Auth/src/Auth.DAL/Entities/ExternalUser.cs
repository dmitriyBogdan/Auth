namespace Auth.DAL.Entities
{
    public class ExternalUser
    {
        public int Id { get; set; }

        public LocalUser User { get; set; }

        public string Provider { get; set; }

        public string NameIdentifier { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }

        public string Surname { get; set; }

        public string FullName { get; set; }
    }
}