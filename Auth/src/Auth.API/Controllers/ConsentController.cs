using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Auth.API.Controllers
{
    public class ConsentController : Controller
    {
        private IIdentityServerInteractionService interaction;

        public ConsentController(IIdentityServerInteractionService interaction)
        {
            this.interaction = interaction;
        }

        [Route("consent")]
        public async Task<IActionResult> Index(string returnUrl)
        {
            var grantedConsent = new ConsentResponse
            {
                RememberConsent = true,
                ScopesConsented = new string[] { "LMS.public" }
            };
            var request = await this.interaction.GetAuthorizationContextAsync(returnUrl);

            // communicate outcome of consent back to identityserver
            await this.interaction.GrantConsentAsync(request, grantedConsent);
            await this.interaction.RevokeUserConsentAsync(request.ClientId);

            return this.Redirect(request.RedirectUri + "/" + request);
        }
    }
}
