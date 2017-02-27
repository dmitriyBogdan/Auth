using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Auth.API.Extensions;
using Auth.BLL.Interfaces;
using IdentityModel;
using IdentityModel.Client;
using IdentityServer4;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Auth.API.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserManager userManager;

        public AccountController(IUserManager userManager)
        {
            this.userManager = userManager;
        }

        public async Task TestLogin()
        {
            var disco = await DiscoveryClient.GetAsync("http://localhost:5001");
            var tokenClient = new TokenClient(disco.TokenEndpoint, "External", "External");
            var tokenResponse = await tokenClient.RequestResourceOwnerPasswordAsync("dimoha_bogdan@mail.ru", "zazazaza4", "LMS.public");
        }
    }
}
