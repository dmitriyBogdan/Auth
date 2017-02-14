using System.Linq;
using Auth.DataAccessLayer;
using Auth.DataAccessLayer.Entities;
using IdentityModel;
using Microsoft.AspNetCore.Builder;

namespace Auth.API.Initialization
{
    public static class UserRolesInitializationExtensions
    {
        public static void InitRoles(this IApplicationBuilder app)
        {
            AuthContext context = (AuthContext)app.ApplicationServices.GetService(typeof(AuthContext));

            CreateIfDoesNotExist(context, Roles.Admin);
            CreateIfDoesNotExist(context, Roles.User);
        }

        private static void CreateIfDoesNotExist(AuthContext context, string roleName)
        {
            Claim claim = context.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Role && x.Value == roleName);
            if (claim == null)
            {
                claim = new Claim
                {
                    Type = JwtClaimTypes.Role,
                    Value = roleName
                };
                context.Claims.Add(claim);
                context.SaveChanges();
            }
        }
    }
}