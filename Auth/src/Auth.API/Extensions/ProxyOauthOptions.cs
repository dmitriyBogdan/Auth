using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;

namespace Auth.API.Extensions
{
    public class ProxyOauthOptions : OAuthOptions
    {
        public ProxyOauthOptions()
        {
            this.AuthenticationScheme = "proxy";
            this.TokenEndpoint = "http://localhost:5001/connect/token";
            this.AuthorizationEndpoint = "http://localhost:5001/connect/authorize";
            this.UserInformationEndpoint = "http://localhost:5001/connect/authorize";
            this.CallbackPath = "/ExternalLogin";
            this.Events = new OAuthEvents()
            {
                OnRedirectToAuthorizationEndpoint = this.OnRedirectToAuthorizationEndpoint
            };
        }

        private Task OnRedirectToAuthorizationEndpoint(OAuthRedirectToAuthorizationContext oAuthRedirectToAuthorizationContext)
        {
            oAuthRedirectToAuthorizationContext.HttpContext.Request.PathBase = this.CallbackPath;
            return Task.FromResult("Ok");
        }
    }
}
