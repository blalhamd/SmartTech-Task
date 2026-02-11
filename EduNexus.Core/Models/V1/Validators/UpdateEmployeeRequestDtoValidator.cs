using EduNexus.Core.Models.V1.Dtos.EmployeeRequest;
using FluentValidation;

namespace EduNexus.Core.Models.V1.Validators
{
    public class UpdateEmployeeRequestDtoValidator : AbstractValidator<UpdateEmployeeRequestDto>
    {
        public UpdateEmployeeRequestDtoValidator()
        {
            RuleFor(x => x.EmployeeId)
                .NotEmpty().WithMessage("Employee ID is required.");

            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Full name is required.")
                .MaximumLength(100).WithMessage("Full name cannot exceed 100 characters.");

            RuleFor(x => x.Salary)
                .GreaterThan(0).WithMessage("Salary must be greater than zero.");

            RuleFor(x => x.Position)
                .NotEmpty().WithMessage("Position is required.")
                .MaximumLength(50).WithMessage("Position cannot exceed 50 characters.");

        }
    }
}
