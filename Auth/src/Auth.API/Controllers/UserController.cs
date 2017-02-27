using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Auth.API.Extensions;
using Auth.API.Initialization;
using Auth.API.ViewModels.User;
using Auth.BLL.Interfaces;
using Auth.DAL.Entities;
using IdentityModel;
using IdentityModel.Client;
using IdentityServer4;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Mvc;
using Claim = System.Security.Claims.Claim;

namespace Auth.API.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserManager userManager;

        public UserController(IUserManager userManager, ITokenService tokenService)
        {
            this.userManager = userManager;
        }

        [Route("Register")]
        public async Task<IActionResult> Register()
        {
            RegisterUserViewModel viewModel = new RegisterUserViewModel();
            await this.userManager.Register(viewModel.ToModel());
            return this.Ok();
        }

        [HttpGet]
        [Route("ProxyLogin")]
        public async Task<IActionResult> ProxyLogin(string login, string password, string provider)
        {
            TokenClient client = new TokenClient("http://localhost:5001/connect/token", "UI", "lms");
            var responsetoken = await client.RequestResourceOwnerPasswordAsync(login, password, "LMS.public openid");
            var userInfoClient = new UserInfoClient("http://localhost:5001/connect/userinfo");
            var response = await userInfoClient.GetAsync(responsetoken.AccessToken);
            var claims = response.Claims;
            var externalUser = this.GetExternalUser(claims);
            AuthenticationProperties props = new AuthenticationProperties();
            var additionalClaims = new List<Claim>();
            var sid = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.SessionId);
            if (sid != null)
            {
                additionalClaims.Add(new Claim(JwtClaimTypes.SessionId, sid.Value));
            }

            await this.userManager.AddExternalUser(externalUser);
            props.StoreTokens(new[] { new AuthenticationToken { Name = "id_token", Value = responsetoken.AccessToken } });
            await this.HttpContext.Authentication.SignInAsync(new Guid().ToString(), externalUser.FullName, externalUser.Provider, props, additionalClaims.ToArray());
            return this.Redirect("/");
        }

        [Route("Account/Login")]
        public void Login(string returnUrl)
        {
            var x = this.HttpContext.Items;
        }

        private ExternalUser GetExternalUser(IEnumerable<Claim> claims)
        {
            return new ExternalUser()
            {
                Email = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Subject).Value,
                Provider = "External",
                FullName = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Subject).Value
            };
        }

        [Route("ExternalLoginCallback")]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = "/connect/authorize")
        {
            var info =
                await
                    this.HttpContext.Authentication.GetAuthenticateInfoAsync(
                        IdentityServerConstants.ExternalCookieAuthenticationScheme);
            var tempUser = info?.Principal;
            if (tempUser == null)
            {
                throw new Exception("External authentication error");
            }

            var externalUser = tempUser.GetExternalUser();
            await this.userManager.AddExternalUser(externalUser);
            AuthenticationProperties props = new AuthenticationProperties();
            var additionalClaims = new List<Claim>();
            var sid = tempUser.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.SessionId);
            if (sid != null)
            {
                additionalClaims.Add(new Claim(JwtClaimTypes.SessionId, sid.Value));
            }

            props.StoreTokens(new[] { new AuthenticationToken { Name = "id_token", Value = info.Properties.GetTokenValue("id_token") } });
            await this.HttpContext.Authentication.SignInAsync(new Guid().ToString(), externalUser.Email, externalUser.Provider, props, additionalClaims.ToArray());
            await this.HttpContext.Authentication.SignOutAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);
            return this.Redirect(this.AddQuery(returnUrl));
        }

        [HttpGet]
        [Route("ExternalLogin")]
        public IActionResult ExternalLogin(string provider = "", string returnUrl = "")
        {
            returnUrl = this.Url.Action("ExternalLoginCallback", new { returnUrl = returnUrl });
            var props = new AuthenticationProperties
            {
                RedirectUri = returnUrl,
                Items = { { "scheme", provider } }
            };

            return new ChallengeResult(provider, props);
        }

        public string AddQuery(string url)
        {
            url += $"?client_Id=UI&response_Type=token&response_Mode=form_post&redirect_uri=http://localhost:5000/VseZaebis&state={CryptoRandom.CreateUniqueId()}&nonce={CryptoRandom.CreateUniqueId()}&scope=LMS.public";
            return url;
        }
    }
}