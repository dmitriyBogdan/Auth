using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Auth.DAL.Entities;
using IdentityServer4;
using IdentityServer4.Extensions;

namespace Auth.API.Extensions
{
    public static class ClaimPrincipalConvertExtensions
    {
        public static ExternalUser GetExternalUser(this ClaimsPrincipal claim)
        {
            var identity = claim.Identity;
            var claims = claim.Claims;
            return new ExternalUser()
            {
                Provider = identity.AuthenticationType,
                FullName = identity.Name,
                Email = claims.FirstOrDefault(x => x.Type == ClaimTypes.Email).Value,
                FirstName = claims.FirstOrDefault(x => x.Type == ClaimTypes.GivenName)?.Value,
                NameIdentifier = claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value,
                Surname = claims.FirstOrDefault(x => x.Type == ClaimTypes.Surname)?.Value
            };
        }
    }
}
