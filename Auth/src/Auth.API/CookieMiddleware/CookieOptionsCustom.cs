using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.Extensions.Primitives;

namespace Auth.API.CookieMiddleware
{
    public class CookieOptionsCustom : CookieAuthenticationOptions
    {
        public CookieOptionsCustom()
        {
            this.Events = new CookieAuthenticationEvents()
            {
                OnRedirectToAccessDenied = this.OnRedirectToAccessDenied
            };
        }

        private Task OnRedirectToAccessDenied(CookieRedirectContext context)
        {
            context.Response.Redirect(this.AccessDeniedPath);
            return TaskCache.CompletedTask;
        }
    }
}
