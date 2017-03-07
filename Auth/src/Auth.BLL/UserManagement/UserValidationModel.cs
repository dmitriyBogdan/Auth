using Auth.DAL.Entities;

namespace Auth.BLL.UserManagement
{
    public class UserValidationModel
    {
        public bool IsVerified { get; set; }

        public LocalUser User { get; set; }
    }
}