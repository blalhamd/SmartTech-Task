using EduNexus.Core.Constants;
using EduNexus.Core.Models.V1.Dtos.Auth;
using FluentValidation;

namespace EduNexus.Core.Models.V1.Validators
{
    public class ResetPasswordRequestValidator : AbstractValidator<ResetPasswordRequest>
    {
        public ResetPasswordRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("A valid email address is required.");

            RuleFor(x=> x.NewPassword)
                .NotEmpty().WithMessage("Password is required")
                .Matches(ApplicationConstants.StrongPasswordPattern)
                .WithMessage("Password must be at least 8 characters long and include uppercase," +
                " lowercase, number, and special character.");

            RuleFor(x => x.Token)
                .NotEmpty().WithMessage("Token is required");

        }
    }
}
