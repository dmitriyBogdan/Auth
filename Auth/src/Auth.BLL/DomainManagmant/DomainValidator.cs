using System.Threading.Tasks;
using Auth.BLL.Interfaces;
using IdentityModel;
using IdentityServer4.Validation;

namespace Auth.BLL.DomainManagmant
{
    public class DomainValidator : IResourceOwnerPasswordValidator
    {
        private readonly IDomainManager domainManager;

        public DomainValidator(IDomainManager domainManager)
        {
            this.domainManager = domainManager;
        }

        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            bool isVerifiDomain = await this.domainManager.VerifiDomain(context.UserName, context.Password);
            if (isVerifiDomain)
            {
                context.Result = new GrantValidationResult(context.UserName,
                    OidcConstants.AuthenticationMethods.Password);
            }
        }
    }
}
