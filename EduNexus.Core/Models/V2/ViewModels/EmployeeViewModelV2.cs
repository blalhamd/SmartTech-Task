namespace EduNexus.Core.Models.V2.ViewModels
{
    public class EmployeeViewModelV2
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = null!;
        public decimal Salary { get; set; }
        public string Position { get; set; } = null!;
    }
}
