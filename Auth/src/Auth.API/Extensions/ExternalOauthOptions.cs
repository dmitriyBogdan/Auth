using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;

namespace Auth.API.Extensions
{
    public class ExternalOauthOptions : OAuthOptions
    {
        public ExternalOauthOptions()
        {
                this.Events = new OAuthEvents()
                {
                    OnCreatingTicket = this.OnCreatingTicket,
                    OnRedirectToAuthorizationEndpoint = this.OnRedirectToAuthorizationEndpoint,
                    OnRemoteFailure = this.OnRemoteFailure
                };
        }

        private Task OnRemoteFailure(FailureContext failureContext)
        {
            return Task.FromResult("14214124");
        }

        private Task OnRedirectToAuthorizationEndpoint(OAuthRedirectToAuthorizationContext oAuthRedirectToAuthorizationContext)
        {
           return Task.FromResult("14214124");
        }

        private Task OnCreatingTicket(OAuthCreatingTicketContext oAuthCreatingTicketContext)
        {
            return Task.FromResult("14214124");
        }
    }
}
