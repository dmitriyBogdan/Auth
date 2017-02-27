using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;

namespace Auth.API.Extensions
{
    public static class ExtensionExternalMiddlevare
    {
        public static IApplicationBuilder UseExternalMiddleware(this IApplicationBuilder app, ExternalOauthOptions options)
        {
            return app.UseMiddleware<ExternalMiddleware>((object)Microsoft.Extensions.Options.Options.Create<ExternalOauthOptions>(options));
        }
    }
}
