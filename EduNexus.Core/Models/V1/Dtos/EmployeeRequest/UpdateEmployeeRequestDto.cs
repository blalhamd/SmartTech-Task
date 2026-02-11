namespace EduNexus.Core.Models.V1.Dtos.EmployeeRequest
{
    public class UpdateEmployeeRequestDto
    {
        public Guid EmployeeId { get; set; }
        public string FullName { get; set; } = null!;
        public decimal Salary { get; set; }
        public string Position { get; set; } = null!;
    }
}
