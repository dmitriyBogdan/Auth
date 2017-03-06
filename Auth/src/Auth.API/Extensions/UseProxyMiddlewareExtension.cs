using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;

namespace Auth.API.Extensions
{
    public static class UseProxyMiddlewareExtension
    {
        public static IApplicationBuilder UseProxyMiddleware(this IApplicationBuilder app, ProxyOauthOptions options)
        {
            return app.UseMiddleware<ProxyMiddleware>((object)Microsoft.Extensions.Options.Options.Create<ProxyOauthOptions>(options));
        }
    }
}
