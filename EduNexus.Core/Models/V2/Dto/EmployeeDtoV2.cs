namespace EduNexus.Core.Models.V2.Dto
{
    public class EmployeeDtoV2
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = null!;
        public decimal Salary { get; set; }
        public string Position { get; set; } = null!;
    }
}
