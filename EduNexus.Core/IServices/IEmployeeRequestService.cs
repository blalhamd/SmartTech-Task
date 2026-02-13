using EduNexus.Core.Models.V1.Dtos.EmployeeRequest;
using EduNexus.Core.Models.V1.ViewModels.EmployeeRequest;
using EduNexus.Shared.Common;

namespace EduNexus.Core.IServices
{
    public interface IEmployeeRequestService
    {
        // request to create a new employee request
        Task<ValueResult<Guid>> CreateEmployeeRequestAsync(CreateEmployeeRequestDto createEmployeeRequestDto, CancellationToken cancellation = default);
        Task<Result> ApproveCreateEmployeeRequestAsync(Guid requestId, CancellationToken cancellation = default);
        // request to update an existing employee request
        Task<ValueResult<Guid>> UpdateEmployeeRequestAsync(Guid employeeId, UpdateEmployeeRequestDto dto, CancellationToken cancellation = default);
        Task<ValueResult<Guid>> ApproveUpdateEmployeeRequestAsync(Guid requestId, CancellationToken cancellation = default);
        // request to delete employee request
        Task<Result> DeleteEmployeeRequestAsync(Guid employeeId, CancellationToken cancellation = default);
        Task<Result> ApproveDeleteRequest(Guid requestId, CancellationToken cancellation = default);
        // request to get all employee requests
        Task<ValueResult<PagesResult<EmployeeRequestViewModel>>> GetAllEmployeeRequestsAsync(int pageNumber, int pageSize, CancellationToken cancellation = default);
        // request to get a specific employee request by id with old data and new data
        Task<ValueResult<EmployeeRequestViewModel>> GetEmployeeRequestByIdAsync(Guid requestId, CancellationToken cancellation = default);
        // reject method
        Task<Result> RejectEmployeeRequestAsync(Guid requestId, string? Reason, CancellationToken cancellation = default);

        // Deactivate employee
        Task<ValueResult<Guid>> DeactivateEmployeeRequest(Guid employeeId, CancellationToken cancel = default);
        Task<Result> ApproveDeactivateEmployeeRequest(Guid requestId, CancellationToken cancellation = default);
    }
}
