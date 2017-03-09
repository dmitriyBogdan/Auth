using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNet.Security.OAuth.LinkedIn;
using Auth.API.Extensions;
using IdentityServer4;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace Auth.API.Initialization
{
    public static class UseExternalAuthProvidersExtension
    {
        public static IApplicationBuilder UseFacebook(this IApplicationBuilder app, IConfiguration config)
        {
            app.UseFacebookAuthentication(new FacebookOptions
            {
                AuthenticationScheme = config["Facebook:AuthScheme"],
                SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme,
                AppId = config["Facebook:ClientId"],
                AppSecret = config["Facebook:ClientSecret"],
                SaveTokens = true
            });

            return app;
        }

        public static IApplicationBuilder UseLinkedin(this IApplicationBuilder app, IConfiguration config)
        {
            app.UseLinkedInAuthentication(new LinkedInAuthenticationOptions()
            {
                AuthenticationScheme = config["LinkedIn:AuthScheme"],
                SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme,
                ClientId = config["LinkedIn:ClientId"],
                ClientSecret = config["LinkedIn:ClientSecret"]
            });
            return app;
        }

        public static IApplicationBuilder UseProxy(this IApplicationBuilder app, IConfiguration config)
        {
            app.UseProxyMiddleware(new ProxyOauthOptions()
            {
                SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme,
                ClientId = config["proxy:ClientId"],
                ClientSecret = config["proxy:ClientSecret"]
            });
            return app;
        }

        public static IApplicationBuilder UseCookies(this IApplicationBuilder app, IConfiguration config)
        {
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme,
                AutomaticAuthenticate = false,
                AutomaticChallenge = false,
                AccessDeniedPath = "/ErrorHandler"
            });
            return app;
        }
    }
}
