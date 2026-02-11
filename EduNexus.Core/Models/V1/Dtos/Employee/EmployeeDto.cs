
namespace EduNexus.Core.Models.V1.Dtos.Employee
{
    public class EmployeeDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = null!;
        public decimal Salary { get; set; }
        public string Position { get; set; } = null!;
        public bool IsActive { get; set; }
        public Guid UserId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
