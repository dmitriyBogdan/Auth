using System.Threading.Tasks;
using Auth.API.Extensions;
using Auth.API.ViewModels.User;
using Auth.BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Auth.API.Controllers
{
    [Route("user")]
    public class UserController : Controller
    {
        private readonly IUserManager userManager;

        public UserController(IUserManager userManager)
        {
            this.userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody]RegisterUserViewModel viewModel)
        {
            if (this.ModelState.IsValid)
            {
                await this.userManager.Register(viewModel.ToModel());

                return this.Ok();
            }

            return this.BadRequest(this.ModelState);
        }
    }
}
