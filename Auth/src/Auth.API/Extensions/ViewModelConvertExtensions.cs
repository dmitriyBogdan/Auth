using Auth.API.ViewModels.User;
using Auth.BusinessLogicLayer.Models;

namespace Auth.API.Extensions
{
    public static class ViewModelConvertExtensions
    {
        public static UserModel ToModel(this RegisterUserViewModel viewModel)
        {
            return new UserModel
            {
                Email = viewModel.Email,
                Password = viewModel.Password
            };
        }
    }
}