using EduNexus.Shared;
using EduNexus.Shared.Common;

namespace EduNexus.Domain.Errors
{
    public class EmployeeErrors
    {
        public static Error InvalidFullName = new (Code: "InvalidFullName", Description: "Full name cannot be empty.", ErrorType: ErrorType.Validation);
        public static Error InvalidSalary = new (Code: "InvalidSalary", Description: "Salary can't be negative value.", ErrorType: ErrorType.Validation);
        public static Error InvalidPosition = new (Code: "InvalidPosition", Description: "Position cannot be empty.", ErrorType: ErrorType.Validation);
        public static Error InvalidUserId = new (Code: "InvalidUserId", Description: "Employee Id cannot be empty.", ErrorType: ErrorType.Validation);
        public static Error NotFound = new (Code: "NotFound", Description: "Employee not found.", ErrorType: ErrorType.NotFound);
        public static Error Conflict = new (Code: "Conflict", Description: "Employee already exist.", ErrorType: ErrorType.Conflict);
        public static Error AlreadyDeactivated = new (Code: "AlreadyDeactivated", Description: "Employee already exist deactivated.", ErrorType: ErrorType.Conflict);
    }
}
