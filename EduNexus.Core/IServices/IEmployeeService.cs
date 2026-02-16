using EduNexus.Core.Models.V1.ViewModels.Employee;
using EduNexus.Core.Models.V2.ViewModels;
using EduNexus.Shared.Common;

namespace EduNexus.Core.IServices
{
    public interface IEmployeeService
    {
        Task<ValueResult<PagesResult<EmployeeViewModel>>> GetEmployees(int pageNumber, int pageSize);
        Task<ValueResult<PagesResult<EmployeeViewModelV2>>> GetEmployeesV2(int pageNumber, int pageSize);
        Task<ValueResult<EmployeeViewModel>> GetById(Guid id);
    }
}
