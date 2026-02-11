namespace EduNexus.Core.Constants
{
    public static class Permissions
    {
        public static string Type { get; } = "permissions";

        public static class EmployeeRequest
        {
            public const string View = "Permissions.EmployeeRequest.View";
            public const string Create = "Permissions.EmployeeRequest.Create";
            public const string Update = "Permissions.EmployeeRequest.Update";
            public const string Delete = "Permissions.EmployeeRequest.Delete";
            public const string Approve = "Permissions.EmployeeRequest.Approve";
            public const string Reject = "Permissions.EmployeeRequest.Reject";
        }
        public static class Employee
        {
            public const string View = "Permissions.Employee.View";
            public const string ViewDetails = "Permissions.Employee.ViewDetails";
        }
    }
}
