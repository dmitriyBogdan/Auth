using System.Security.Claims;
using System.Threading.Tasks;
using Auth.BLL.UserManagement;
using Auth.DAL.Entities;
using Microsoft.AspNetCore.Http;

namespace Auth.BLL.Interfaces
{
    public interface IUserManager
    {
        Task Register(UserModel model);

        Task<UserValidationModel> VerifyUser(string email, string password);

        Task AddExternalUser(ExternalUser externalUser);

        Task<string> GetToken(HttpContext httpContext, ClaimsPrincipal tempUser);
    }
}