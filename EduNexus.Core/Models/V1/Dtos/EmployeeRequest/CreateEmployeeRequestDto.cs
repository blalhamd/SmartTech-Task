namespace EduNexus.Core.Models.V1.Dtos.EmployeeRequest
{
    public class CreateEmployeeRequestDto
    {
        public string FullName { get; set; } = null!;
        public decimal Salary { get; set; }
        public string Position { get; set; } = null!;
        public string Email { get; set; } = null!;
    }
}
