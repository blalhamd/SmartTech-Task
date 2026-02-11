using EduNexus.Domain.Enums;

namespace EduNexus.Core.Models.V1.ViewModels.EmployeeRequest
{
    public class EmployeeRequestViewModel
    {
        public Guid Id { get; set; }
        public Guid? EmployeeId { get; set; }
        public ActionType ActionType { get; set; } 
        public RequestStatus Status { get; set; }

        public string? FullName { get; set; } = null!;
        public string? Email { get; set; } = null!;
        public decimal? Salary { get; set; } 
        public string? Position { get; set; } = null!;
        public string? RejectionReason { get; set; }
        public string? OldData { get; set; }
    }
}
