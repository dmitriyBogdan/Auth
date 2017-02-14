using Auth.DAL.Entities;

namespace Auth.BLL.Models
{
    public class UserValidationModel
    {
        public bool IsVerified { get; set; }

        public LocalUser User { get; set; }
    }
}