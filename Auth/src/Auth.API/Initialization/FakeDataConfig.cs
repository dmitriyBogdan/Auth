using System.Collections.Generic;
using IdentityServer4;
using IdentityServer4.Models;

namespace Auth.API.Initialization
{
    public class FakeDataConfig
    {
        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId = "UI",
                    ClientName = "UI Client",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    ClientSecrets =
                    {
                        new Secret("lms".Sha256())
                    },
                    AllowedScopes =
                    {
                        "LMS.public",
                        "LMS.private",
                        "LRS.private",
                        "offline_access",
                        IdentityServerConstants.StandardScopes.OpenId
                    },
                    AllowOfflineAccess = true
                }
            };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("LMS.public", "Auth API")
            };
        }
    }
}