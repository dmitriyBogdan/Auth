using System.Linq;
using System.Threading.Tasks;
using Auth.BLL.Interfaces;
using Auth.BLL.Models;
using IdentityModel;
using IdentityServer4.Validation;
using Claim = System.Security.Claims.Claim;

namespace Auth.BLL
{
    public class ResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        private readonly IUserManager userManager;

        public ResourceOwnerPasswordValidator(IUserManager userManager)
        {
            this.userManager = userManager;
        }

        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            UserValidationModel model = await this.userManager.VerifyUser(context.UserName, context.Password);
            if (model.IsVerified)
            {
                string role = model.User
                                    .LocalUserClaims
                                    .Select(x => x.Claim)
                                    .First(x => x.Type == JwtClaimTypes.Role)
                                    .Value;
                var claims = new[] { new Claim(JwtClaimTypes.Role, role) };

                context.Result = new GrantValidationResult(context.UserName, "pwd", claims);
            }
        }
    }
}