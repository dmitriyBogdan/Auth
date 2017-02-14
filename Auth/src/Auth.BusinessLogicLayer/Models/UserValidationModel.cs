using Auth.DataAccessLayer.Entities;

namespace Auth.BusinessLogicLayer.Models
{
    public class UserValidationModel
    {
        public bool IsVerified { get; set; }

        public LocalUser User { get; set; }
    }
}