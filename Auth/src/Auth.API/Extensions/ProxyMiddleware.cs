﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Auth.API.Extensions
{
    public class ProxyMiddleware : OAuthMiddleware<ProxyOauthOptions>
    {
        public ProxyMiddleware(RequestDelegate next, IDataProtectionProvider dataProtectionProvider, ILoggerFactory loggerFactory, UrlEncoder encoder, IOptions<SharedAuthenticationOptions> sharedOptions, IOptions<ProxyOauthOptions> options) : base(next, dataProtectionProvider, loggerFactory, encoder, sharedOptions, options)
        {
        }

        protected override AuthenticationHandler<ProxyOauthOptions> CreateHandler()
        {
            return (AuthenticationHandler<ProxyOauthOptions>)new ProxyHandlerOauth(this.Backchannel);
        }
    }
}
