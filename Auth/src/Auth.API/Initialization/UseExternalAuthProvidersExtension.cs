using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNet.Security.OAuth.LinkedIn;
using IdentityServer4;
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
            Console.WriteLine($"{config["LinkedIn:AuthScheme"]}   |   {config["LinkedIn:ClientId"]}     |   {config["LinkedIn:ClientSecret"]}");
            app.UseLinkedInAuthentication(new LinkedInAuthenticationOptions()
            {
                AuthenticationScheme = config["LinkedIn:AuthScheme"],
                SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme,
                ClientId = config["LinkedIn:ClientId"],
                ClientSecret = config["LinkedIn:ClientSecret"]
            });
            return app;
        }
    }
}
