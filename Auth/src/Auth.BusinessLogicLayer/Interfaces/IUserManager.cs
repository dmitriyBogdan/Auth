using System.Threading.Tasks;
using Auth.BusinessLogicLayer.Models;
using Auth.DataAccessLayer.Entities;

namespace Auth.BusinessLogicLayer.Interfaces
{
    public interface IUserManager
    {
        Task Register(UserModel model);

        Task<UserValidationModel> VerifyUser(string email, string password);
    }
}