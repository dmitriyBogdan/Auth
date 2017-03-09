using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AspNet.Security.OAuth.LinkedIn;
using IdentityModel;
using IdentityModel.Client;
using IdentityServer4.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Http.Features.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace Auth.API.Extensions
{
    public class ProxyHandlerOauth : OAuthHandler<ProxyOauthOptions>
    {
        public ProxyHandlerOauth(HttpClient backchannel) : base(backchannel)
        {
        }

        public override async Task<bool> HandleRequestAsync()
        {
            if (this.Options.CallbackPath == this.Request.Path && this.Options.AuthenticationScheme == this.Request.Query["provider"].ToString())
            {
                return await this.HandleRemoteCallbackAsync();
            }

            return false;
        }

        protected override Task HandleSignInAsync(SignInContext context)
        {
            return TaskCache.CompletedTask;
        }

        protected override async Task<bool> HandleRemoteCallbackAsync()
        {
            AuthenticationTicket ticket = (AuthenticationTicket)null;
            Exception exception = (Exception)null;
            try
            {
                AuthenticateResult authenticateResult = await this.HandleRemoteAuthenticateAsync();
                if (authenticateResult == null)
                {
                    exception = (Exception)new InvalidOperationException("Invalid return state, unable to redirect.");
                }
                else
                {
                    if (authenticateResult.Handled)
                    {
                        return true;
                    }

                    if (authenticateResult.Skipped)
                    {
                        return false;
                    }

                    if (!authenticateResult.Succeeded)
                    {
                        exception = authenticateResult.Failure ?? (Exception)new InvalidOperationException("Invalid return state, unable to redirect.");
                    }
                }

                ticket = authenticateResult.Ticket;
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            if (exception != null)
            {
                FailureContext errorContext = new FailureContext(this.Context, exception);
                await this.Options.Events.RemoteFailure(errorContext);
                if (errorContext.HandledResponse)
                {
                    return true;
                }

                if (errorContext.Skipped)
                {
                    return false;
                }

                return true;
            }

            TicketReceivedContext context = new TicketReceivedContext(this.Context, (RemoteAuthenticationOptions)this.Options, ticket)
            {
                ReturnUri = ticket.Properties.RedirectUri
            };

            ticket.Properties.RedirectUri = (string)null;
            context.Properties.Items[".AuthScheme"] = this.Options.AuthenticationScheme;
            await this.Options.Events.TicketReceived(context);
            if (context.HandledResponse)
            {
                return true;
            }

            if (context.Skipped)
            {
                return false;
            }

            await this.Context.Authentication.SignInAsync(this.Options.SignInScheme, context.Principal, context.Properties);
            if (string.IsNullOrEmpty(context.ReturnUri))
            {
                context.ReturnUri = "/ExternalLoginCallback";
            }

            this.Response.Redirect(context.ReturnUri);
            return true;
        }

        protected override async Task<AuthenticateResult> HandleRemoteAuthenticateAsync()
        {
            var domain = this.Request.Query["domain"];
            var sid = this.Request.Query["sid"];
            TokenClient client = new TokenClient(this.Options.TokenEndpoint, this.Options.ClientId, this.Options.ClientSecret);
            var responseToken = await client.RequestResourceOwnerPasswordAsync(domain, sid, "LMS.public openid");
            if (responseToken.AccessToken == null)
            {
                return AuthenticateResult.Fail(responseToken.ErrorDescription);
            }

            JwtSecurityToken securityToken = new JwtSecurityToken(responseToken.AccessToken);
            var emailClime = new Claim(ClaimTypes.Email, securityToken.Claims.FirstOrDefault(x => x.Type == "sub").Value);
            var roleClime = new Claim(ClaimTypes.Role, "user");
            var claimPrincipal = new ClaimsPrincipal(Identity.Create(this.Options.AuthenticationScheme, emailClime, roleClime));
            AuthenticationProperties prop = new AuthenticationProperties();
            prop.StoreTokens(new[] { new AuthenticationToken { Name = "access_token", Value = responseToken.AccessToken } });
            var token = new AuthenticationTicket(claimPrincipal, prop, this.Options.AuthenticationScheme);
            return AuthenticateResult.Success(token);
        }
    }
}
