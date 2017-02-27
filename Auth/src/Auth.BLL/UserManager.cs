using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Auth.BLL.Interfaces;
using Auth.BLL.Models;
using Auth.DAL;
using Auth.DAL.Entities;
using IdentityModel;
using IdentityModel.Client;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Claim = Auth.DAL.Entities.Claim;

namespace Auth.BLL
{
    public class UserManager : IUserManager
    {
        private readonly AuthContext context;
        private readonly ICrypto crypto;
        private readonly ITokenService tokenService;

        public UserManager(AuthContext context, ICrypto crypto, ITokenService tokenService)
        {
            this.context = context;
            this.crypto = crypto;
            this.tokenService = tokenService;
        }

        public async Task Register(UserModel model)
        {
            LocalUser user = await this.CreateUser(model);
            await this.AddRoleClaim(user);
        }

        private async Task<LocalUser> CreateUser(UserModel model)
        {
            string salt = this.crypto.GenerateSalt();

            var localUser = new LocalUser
            {
                Email = model.Email,
                Password = this.crypto.ComputeHash(model.Password, salt),
                Salt = salt
            };

            await this.context.LocalUsers.AddAsync(localUser);
            await this.context.SaveChangesAsync();

            return localUser;
        }

        private async Task AddRoleClaim(LocalUser user)
        {
            Claim claim = await this.context.Claims.FirstAsync(x => x.Type == JwtClaimTypes.Role && x.Value == Roles.User);

            var localUserClaim = new LocalUserClaim
            {
                LocalUserId = user.Id,
                LocalUser = user,
                ClaimId = claim.Id,
                Claim = claim
            };

            await this.context.LocalUserClaims.AddAsync(localUserClaim);
            await this.context.SaveChangesAsync();
        }

        public async Task<UserValidationModel> VerifyUser(string email, string password)
        {
            var model = new UserValidationModel();

            LocalUser user = await this.context.LocalUsers
                                                .Include(x => x.LocalUserClaims)
                                                .ThenInclude(x => x.Claim)
                                                .FirstOrDefaultAsync(x => x.Email == email);
            if (user != null)
            {
                string hash = this.crypto.ComputeHash(password, user.Salt);
                if (hash == user.Password)
                {
                    model.IsVerified = true;
                    model.User = user;
                }
            }

            return model;
        }

        public async Task AddExternalUser(ExternalUser externalUser)
        {
            var existLocalUser = await this.context.LocalUsers.FirstOrDefaultAsync(x => x.Email == externalUser.Email);
            if (existLocalUser != null)
            {
                if (!await this.context.ExternalUsers.AnyAsync(x => x.Email == externalUser.Email))
                {
                    await this.AddExternalToLocalUser(externalUser, existLocalUser);
                }
            }
            else
            {
                await this.AddExistLocalUserAndExternal(externalUser);
            }
        }

        public LocalUser GetUser(string email)
        {
            return this.context.LocalUsers.FirstOrDefault(x => x.Email == email);
        }

        public async Task<string> GetToken(HttpContext context, ClaimsPrincipal claim)
        {
            // var email = claim.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email).Value;
            // var currentUser = this.GetUser(email);
            // List<System.Security.Claims.Claim> claims = new List<System.Security.Claims.Claim>();
            // claims.AddRange(claim.Claims.Select(x => new System.Security.Claims.Claim(x.Type, x.Value)));
            // claims.AddRange(currentUser.LocalUserClaims.Select(x => new System.Security.Claims.Claim(x.Claim.Type, x.Claim.Value)));
            // var issuer = context.GetIdentityServerIssuerUri();
            // var token = new Token(OidcConstants.TokenTypes.AccessToken)
            // {
            //     Audiences = { string.Format("{0}resources", issuer), "LMS.public" },
            //     Issuer = issuer,
            //     Lifetime = 3600,
            //     Claims = claims,
            //     ClientId = "UI",
            //     AccessTokenType = AccessTokenType.Jwt,
            //     Type = "local"
            // };
            // return await this.tokenService.CreateSecurityTokenAsync(token);
            var request = new AuthorizeRequest("http://localhost:5000/connect/authorize");
            var url = request.CreateAuthorizeUrl(
                clientId: "UI",
                responseType: OidcConstants.ResponseTypes.IdTokenToken,
                responseMode: OidcConstants.ResponseModes.FormPost,
                redirectUri: "http://localhost:5000/VseZaebis",
                state: CryptoRandom.CreateUniqueId(),
                nonce: CryptoRandom.CreateUniqueId(),
                scope: "LMS.public");
            HttpClient client = new HttpClient();
            var x = await client.GetAsync(url);
            var response = new IdentityModel.Client.AuthorizeResponse(url);

            var accessToken = response.AccessToken;
            var idToken = response.IdentityToken;
            var state = response.State;
            Console.WriteLine(accessToken);
            return accessToken;
        }

        private async Task AddExternalToLocalUser(ExternalUser externalUser, LocalUser existLocalUser)
        {
            externalUser.User = existLocalUser;
            await this.context.ExternalUsers.AddAsync(externalUser);
            await this.context.SaveChangesAsync();
        }

        private async Task AddExistLocalUserAndExternal(ExternalUser externalUser)
        {
            var localUser = await this.CreateUser(new UserModel() { Email = externalUser.Email });
            await this.AddRoleClaim(localUser);
            externalUser.User = localUser;
            await this.context.ExternalUsers.AddAsync(externalUser);
            await this.context.SaveChangesAsync();
        }
    }
}