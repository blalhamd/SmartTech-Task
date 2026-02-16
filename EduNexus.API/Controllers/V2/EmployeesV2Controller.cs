using EduNexus.API.Controllers.Base;
using EduNexus.API.Filters.Authorization;
using EduNexus.Core.Constants;
using EduNexus.Core.IServices;
using EduNexus.Core.Models.V2.ViewModels;
using EduNexus.Shared.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduNexus.API.Controllers.V2
{
    [ApiVersion("2.0")]
    [Route("api/v2/employees")]
    public class EmployeesV2Controller : BasApiController
    {
        private readonly IEmployeeService _employeeService;

        public EmployeesV2Controller(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpGet]
        [MapToApiVersion("2.0")]
        [HasPermission(Permissions.Employee.View)]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(PagesResult<EmployeeViewModelV2>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
            => HandleResult(await _employeeService.GetEmployeesV2(pageNumber, pageSize));

    }
}
