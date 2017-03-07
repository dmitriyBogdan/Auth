using System.Linq;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Claim = System.Security.Claims.Claim;

namespace Auth.BLL.UserManagement
{
    public class ProfileService : IProfileService
    {
        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            await Task.Run(() =>
                                {
                                    Claim role = context.Subject.Claims.First(x => x.Type == JwtClaimTypes.Role);
                                    context.IssuedClaims.Add(role);
                                });
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            await Task.Run(() => { });
        }
    }
}
