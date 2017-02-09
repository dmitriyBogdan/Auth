using System.Collections.Generic;
using IdentityServer4.Models;

namespace Auth.Initialization
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
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets =
                    {
                        new Secret("lms".Sha256())
                    },
                    AllowedScopes = { "LMS.public" }
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