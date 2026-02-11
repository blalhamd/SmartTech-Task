using EduNexus.Core.Constants;
using EduNexus.Core.Models.V1.Dtos.Auth;
using FluentValidation;

namespace EduNexus.Core.Models.V1.Validators
{
    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Email is invalid");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required")
                .Matches(ApplicationConstants.StrongPasswordPattern)
                .WithMessage("Password must be at least 8 characters long and include uppercase," +
                " lowercase, number, and special character.");

        }
    }
}
