using Auth.API.ViewModels.User;
using Auth.DataAccessLayer;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Auth.API.Validators.User
{
    public class RegisterUserValidator : AbstractValidator<RegisterUserViewModel>
    {
        public RegisterUserValidator(AuthContext context)
        {
            this.RuleFor(x => x.Email).NotEmpty().WithMessage("E-mail must not be empty")
                                    .EmailAddress().WithMessage("Invalid E-mail address")
                                    .MustAsync(async (x, token) => (await context.LocalUsers.FirstOrDefaultAsync(l => l.Email == x)) == null)
                                    .WithMessage("User with such E-mail already exists");

            this.RuleFor(x => x.Password).NotEmpty().WithMessage("Password must not be empty")
                                        .Must(x => x.Length >= 8).WithMessage("Password length must be greater or equal than 8")
                                        .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[A-Za-z\d]")
                                        .WithMessage("Password must contain at least 1 uppercase, 1 lowercase and 1 number");

            this.RuleFor(x => x.RepeatedPassword).NotEmpty().WithMessage("Repeated password must not be empty")
                                                .Equal(x => x.Password).WithMessage("Passwords don't match");
        }
    }
}