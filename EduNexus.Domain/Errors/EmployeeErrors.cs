using EduNexus.Shared;
using EduNexus.Shared.Common;

namespace EduNexus.Domain.Errors
{
    public class EmployeeErrors
    {
        public static Error InvalidFullName = new (Code: "InvalidFullName", Description: "Full name cannot be empty.", ErrorType: ErrorType.Validation);
        public static Error InvalidSalary = new (Code: "InvalidSalary", Description: "Salary can't be negative value.", ErrorType: ErrorType.Validation);
        public static Error InvalidPosition = new (Code: "InvalidPosition", Description: "Position cannot be empty.", ErrorType: ErrorType.Validation);
        public static Error InvalidUserId = new (Code: "InvalidUserId", Description: "User Id cannot be empty.", ErrorType: ErrorType.Validation);
        public static Error NotFound = new (Code: "NotFound", Description: "User not found.", ErrorType: ErrorType.NotFound);
        public static Error Conflict = new (Code: "NotFound", Description: "User alreay exist.", ErrorType: ErrorType.Conflict);
    }
}
