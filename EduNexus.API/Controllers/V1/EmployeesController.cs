using EduNexus.API.Controllers.Base;
using EduNexus.API.Filters.Authorization;
using EduNexus.Core.Constants;
using EduNexus.Core.IServices;
using EduNexus.Core.Models.V1.ViewModels.Employee;
using EduNexus.Shared.Common;
using Microsoft.AspNetCore.Mvc;

namespace EduNexus.API.Controllers.V1
{
    [ApiVersion("1.0")]
    [Route("api/v1/employees")]
    public class EmployeesController : BasApiController
    {
        private readonly IEmployeeService _employeeService;

        public EmployeesController(IEmployeeService employeeService)
            => _employeeService = employeeService;

        [HttpGet]
        [MapToApiVersion("1.0")]
        [HasPermission(Permissions.Employee.View)]
        [ProducesResponseType(typeof(PagedResult<EmployeeViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
            => HandleResult(await _employeeService.GetEmployees(pageNumber, pageSize));

        [HttpGet("{id:guid}")]
        [MapToApiVersion("1.0")]
        [HasPermission(Permissions.Employee.ViewDetails)]
        [ProducesResponseType(typeof(EmployeeViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
            => HandleResult(await _employeeService.GetById(id));

    }
}
