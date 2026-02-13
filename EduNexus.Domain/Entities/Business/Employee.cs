using EduNexus.Domain.Entities.Base;
using EduNexus.Domain.Entities.Identity;
using EduNexus.Domain.Errors;
using EduNexus.Shared.Common;

namespace EduNexus.Domain.Entities.Business
{
    public class Employee : BaseEntity
    {
        public string FullName { get; private set; } = null!;
        public decimal Salary { get; private set; }
        public string Position { get; private set; } = null!;
        public bool IsActive { get; private set; }

        public Guid UserId { get; private set; } 
        public ApplicationUser User { get; private set; } = null!;

        private Employee() { } 

        private Employee(string fullName, decimal salary, string position, Guid userId)
        {
            FullName = fullName;
            Salary = salary;
            Position = position;
            UserId = userId;
            IsActive = true;
        }

        public static ValueResult<Employee> Create(string fullName, decimal salary, string position, Guid userId)
        {
            var error = Validate(fullName, salary, position, userId);
            if (error != Error.None)
                return ValueResult<Employee>.Failure(error);

            return ValueResult<Employee>.Success(new Employee(fullName, salary, position, userId));
        }

        public Result Update(string fullName, decimal salary, string position)
        {
            var error = Validate(fullName, salary, position, UserId);
            if (error != Error.None)
                return Result.Failure(error);

            FullName = fullName;
            Salary = salary;
            Position = position;

            return Result.Success();
        }

        public Result Lock()
        {
            IsActive = false;
            return Result.Success();
        }

        private static Error Validate(string fullName, decimal salary, string position, Guid userId)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                return EmployeeErrors.InvalidFullName;

            if (salary < 0)
                return EmployeeErrors.InvalidSalary;

            if (string.IsNullOrWhiteSpace(position))
                return EmployeeErrors.InvalidPosition;

            if(userId == Guid.Empty)
                return EmployeeErrors.InvalidUserId;

            return Error.None;
        }
    }
}
