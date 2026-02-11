using EduNexus.API.Controllers.Base;
using EduNexus.API.Filters.Authorization;
using EduNexus.Core.Constants;
using EduNexus.Core.IServices;
using EduNexus.Core.Models.V1.Dtos.EmployeeRequest;
using EduNexus.Core.Models.V1.ViewModels.EmployeeRequest;
using EduNexus.Shared.Common;
using Microsoft.AspNetCore.Mvc;

namespace EduNexus.API.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/employee-requests")]
    public class EmployeeRequestsController : BasApiController
    {
        private readonly IEmployeeRequestService _requestService;

        public EmployeeRequestsController(IEmployeeRequestService requestService)
            => _requestService = requestService;

        #region Maker Actions

        [HttpPost]
        [MapToApiVersion("1.0")]
        [HasPermission(Permissions.EmployeeRequest.Create)]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateEmployeeRequestDto dto, CancellationToken ct)
            => HandleResult(await _requestService.CreateEmployeeRequestAsync(dto, ct));


        [HttpPost("update/{employeeId:guid}")]
        [MapToApiVersion("1.0")]
        [HasPermission(Permissions.EmployeeRequest.Update)]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
        public async Task<IActionResult> RaiseUpdate(Guid employeeId, [FromBody] UpdateEmployeeRequestDto dto, CancellationToken ct)
            => HandleResult(await _requestService.UpdateEmployeeRequestAsync(employeeId, dto, ct));


        [HttpDelete("{employeeId:guid}")]
        [MapToApiVersion("1.0")]
        [HasPermission(Permissions.EmployeeRequest.Delete)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        public async Task<IActionResult> RaiseDelete(Guid employeeId, CancellationToken ct)
            => HandleResult(await _requestService.DeleteEmployeeRequestAsync(employeeId, ct));

        #endregion

        #region Checker Actions

        [HttpPatch("{id:guid}/approve-create")]
        [MapToApiVersion("1.0")]
        [HasPermission(Permissions.EmployeeRequest.Approve)]
        public async Task<IActionResult> ApproveCreate(Guid id, CancellationToken ct)
            => HandleResult(await _requestService.ApproveCreateEmployeeRequestAsync(id, ct));


        [HttpPatch("{id:guid}/approve-update")]
        [MapToApiVersion("1.0")]
        [HasPermission(Permissions.EmployeeRequest.Approve)]
        public async Task<IActionResult> ApproveUpdate(Guid id, CancellationToken ct)
            => HandleResult(await _requestService.ApproveUpdateEmployeeRequestAsync(id, ct));


        [HttpPatch("{id:guid}/approve-delete")]
        [MapToApiVersion("1.0")]
        [HasPermission(Permissions.EmployeeRequest.Approve)]
        public async Task<IActionResult> ApproveDelete(Guid id, CancellationToken ct)
            => HandleResult(await _requestService.ApproveDeleteRequest(id, ct));


        [HttpPatch("{id:guid}/reject")]
        [MapToApiVersion("1.0")]
        [HasPermission(Permissions.EmployeeRequest.Reject)]
        public async Task<IActionResult> Reject(Guid id, [FromQuery] string? reason, CancellationToken ct)
            => HandleResult(await _requestService.RejectEmployeeRequestAsync(id, reason, ct));

        #endregion

        #region Queries

        [HttpGet]
        [MapToApiVersion("1.0")]
        [HasPermission(Permissions.EmployeeRequest.View)]
        [ProducesResponseType(typeof(PagesResult<EmployeeRequestViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(int pageNumber = 1, int pageSize = 10, CancellationToken ct = default)
            => HandleResult(await _requestService.GetAllEmployeeRequestsAsync(pageNumber, pageSize, ct));


        [HttpGet("{id:guid}")]
        [MapToApiVersion("1.0")]
        [HasPermission(Permissions.EmployeeRequest.View)]
        [ProducesResponseType(typeof(EmployeeRequestViewModel), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
            => HandleResult(await _requestService.GetEmployeeRequestByIdAsync(id, ct));

        #endregion
    }
}
