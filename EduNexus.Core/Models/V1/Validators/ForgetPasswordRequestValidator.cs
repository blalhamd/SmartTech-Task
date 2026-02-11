using EduNexus.Core.Models.V1.Dtos.Auth;
using FluentValidation;

namespace EduNexus.Core.Models.V1.Validators
{
    public class ForgetPasswordRequestValidator : AbstractValidator<ForgetPasswordRequest>
    {
        public ForgetPasswordRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("A valid email address is required.");

        }
    }
}
