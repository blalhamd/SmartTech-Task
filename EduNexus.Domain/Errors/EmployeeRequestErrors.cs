using EduNexus.Shared;
using EduNexus.Shared.Common;

namespace EduNexus.Domain.Errors
{
    public class EmployeeRequestErrors
    {
        public static Error InValidNewData = new("EmployeeRequest.InvalidNewData", "New data cannot be empty.", ErrorType.Validation);
        public static Error AlreadyApproved = new("EmployeeRequest.AlreadyApproved", "Only pending requests can be approved.", ErrorType.Validation);
        public static Error InvalidEmployeeId = new("EmployeeRequest.InvalidEmployeeId", "Employee Id cannot be empty.", ErrorType.Validation);
        public static Error InavlidOldData = new("EmployeeRequest.InavlidOldData", "Old data cannot be empty.", ErrorType.Validation);
        public static Error AlreadyProcessed = new("EmployeeRequest.AlreadyProcessed", "Request Already Processed.", ErrorType.Validation);
        public static Error ReviewerCannotBeMaker = new("EmployeeRequest.ReviewerCannotBeMaker", "Reviewer can't be maker.", ErrorType.Validation);
        public static Error NotFound = new("EmployeeRequest.NotFound", "Employee request not found.", ErrorType.NotFound);
    }
}
