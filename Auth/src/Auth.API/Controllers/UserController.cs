using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Auth.API.Extensions;
using Auth.API.ViewModels.User;
using Auth.BLL.Interfaces;
using IdentityModel;
using IdentityServer4;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Http.Features.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace Auth.API.Controllers
{
    [Route("user")]
    public class UserController : Controller
    {
        private readonly IUserManager userManager;

        public UserController(IUserManager userManager)
        {
            this.userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterUserViewModel viewModel)
        {
            if (this.ModelState.IsValid)
            {
                await this.userManager.Register(viewModel.ToModel());

                return this.Ok();
            }

            return this.BadRequest(this.ModelState);
        }

        [HttpGet]
        [Route("ExternalLogin")]
        public IActionResult ExternalLogin(string provider = "", string returnUrl = "")
        {
            returnUrl = this.Url.Action("ExternalLoginCallback", new { returnUrl });
            var props = new AuthenticationProperties
            {
                RedirectUri = returnUrl,
                Items = { { "scheme", provider } }
            };

            return new ChallengeResult(provider, props);
        }

        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null)
        {
            var info = await this.HttpContext.Authentication.GetAuthenticateInfoAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);
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

            props.StoreTokens(new[] { new AuthenticationToken { Name = "id_token", Value = info.Properties.GetTokenValue("access_token") } });
            await this.HttpContext.Authentication.SignInAsync(externalUser.NameIdentifier, externalUser.FullName, externalUser.Provider, props, additionalClaims.ToArray());

            await this.HttpContext.Authentication.SignOutAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);
            return this.RedirectToAction("CheckRegistr");
        }
    }
}