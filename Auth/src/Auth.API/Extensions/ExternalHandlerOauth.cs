using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AspNet.Security.OAuth.LinkedIn;
using IdentityModel.Client;
using IdentityServer4.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Http.Features.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace Auth.API.Extensions
{
    public class ExternalHandlerOauth : OAuthHandler<ExternalOauthOptions>
    {
        public ExternalHandlerOauth(HttpClient backchannel) : base(backchannel)
        {
        }

        public override async Task<bool> HandleRequestAsync()
        {
            if (this.Options.CallbackPath == this.Request.Path)
            {
                return await this.HandleRemoteCallbackAsync();
            }

            return false;
        }

        protected override string BuildChallengeUrl(AuthenticationProperties properties, string redirectUri)
        {
            return base.BuildChallengeUrl(properties, redirectUri);
        }

        protected override Task<AuthenticationTicket> CreateTicketAsync(ClaimsIdentity identity, AuthenticationProperties properties, OAuthTokenResponse tokens)
        {
            return base.CreateTicketAsync(identity, properties, tokens);
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

                throw new AggregateException("Unhandled remote failure.", exception);
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
                context.ReturnUri = "/EXternalLoginCallback";
            }

            this.Response.Redirect(context.ReturnUri);
            return true;
        }

        protected override Task<OAuthTokenResponse> ExchangeCodeAsync(string code, string redirectUri)
        {
            return base.ExchangeCodeAsync(code, redirectUri);
        }

        protected override async Task<AuthenticateResult> HandleRemoteAuthenticateAsync()
        {
            TokenClient client = new TokenClient(this.Options.TokenEndpoint, this.Options.ClientId, this.Options.ClientSecret);
            var responsetoken = await client.RequestResourceOwnerPasswordAsync("dimoha_bogdan@mail.ru", "zazazaza4", "LMS.public openid");
            var userInfoClient = new UserInfoClient("http://localhost:5001/connect/userinfo");
            var response = await userInfoClient.GetAsync(responsetoken.AccessToken);
            ClaimsIdentity claims = new ClaimsIdentity();
            claims.AddClaim(new Claim(ClaimTypes.Email, response.Claims.FirstOrDefault(x => x.Type == "sub").Value));
            claims.AddClaim(new Claim(ClaimTypes.Role, response.Claims.FirstOrDefault(x => x.Type == "role").Value));
            var claim = new ClaimsPrincipal(new List<ClaimsIdentity>() { claims });
            AuthenticationProperties prop = new AuthenticationProperties();
            prop.StoreTokens(new[] { new AuthenticationToken { Name = "id_token", Value = responsetoken.AccessToken } });
            var token = new AuthenticationTicket(claim, prop, this.Options.AuthenticationScheme);
            return AuthenticateResult.Success(token);
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            TokenClient client = new TokenClient("http://localhost:5001/connect/token", "UI", "lms");
            var responsetoken = await client.RequestResourceOwnerPasswordAsync("dimoha_bogdan@mail.ru", "zazazaza4", "LMS.public openid");
            var userInfoClient = new UserInfoClient("http://localhost:5001/connect/userinfo");
            var response = await userInfoClient.GetAsync(responsetoken.AccessToken);
            var claim = new ClaimsPrincipal(new List<ClaimsIdentity>() { new ClaimsIdentity(response.Claims) });
            AuthenticationProperties prop = new AuthenticationProperties();
            prop.StoreTokens(new[] { new AuthenticationToken { Name = "id_token", Value = responsetoken.AccessToken } });
            var token = new AuthenticationTicket(claim, prop, this.Options.AuthenticationScheme);
            return AuthenticateResult.Success(token);
        }

        protected override bool ValidateCorrelationId(AuthenticationProperties properties)
        {
            return base.ValidateCorrelationId(properties);
        }

        protected override async Task<bool> HandleForbiddenAsync(ChallengeContext context)
        {
            ChallengeContext challengeContext = new ChallengeContext(this.Options.SignInScheme, context.Properties, ChallengeBehavior.Automatic);
            await this.PriorHandler.ChallengeAsync(challengeContext);
            return true;
        }

        protected override Task HandleSignInAsync(SignInContext context)
        {
            return base.HandleSignInAsync(context);
        }

        protected override Task<bool> HandleUnauthorizedAsync(ChallengeContext context)
        {
            return base.HandleUnauthorizedAsync(context);
        }

        protected override void GenerateCorrelationId(AuthenticationProperties properties)
        {
            base.GenerateCorrelationId(properties);
        }

        protected override Task FinishResponseAsync()
        {
            return base.FinishResponseAsync();
        }
    }
}
