using System.Threading.Tasks;
using Auth.BLL.Models;
using Auth.DAL.Entities;

namespace Auth.BLL.Interfaces
{
    public interface IUserManager
    {
        Task Register(UserModel model);

        Task<UserValidationModel> VerifyUser(string email, string password);

        Task AddExternalUser(ExternalUser externalUser);
    }
}