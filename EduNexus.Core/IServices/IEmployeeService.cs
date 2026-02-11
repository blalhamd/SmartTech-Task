using EduNexus.Core.Models.V1.ViewModels.Employee;
using EduNexus.Shared.Common;

namespace EduNexus.Core.IServices
{
    public interface IEmployeeService
    {
        Task<ValueResult<PagesResult<EmployeeViewModel>>> GetEmployees(int pageNumber, int pageSize);
        Task<ValueResult<EmployeeViewModel>> GetById(Guid id);
    }
}
