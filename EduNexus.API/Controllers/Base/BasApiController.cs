using EduNexus.Shared;
using EduNexus.Shared.Common;
using Microsoft.AspNetCore.Mvc;

namespace EduNexus.API.Controllers.Base
{
    [ApiController]
    public abstract class BasApiController : ControllerBase
    {

        [NonAction]
        public IActionResult HandleResult(Result result)
        {
            return result.IsSuccess ? Ok() : CreateProblemDetails(result.Error);
        }

        [NonAction]
        public IActionResult HandleResult<T>(ValueResult<T> result)
        {
            return result.IsSuccess ? Ok(result.Value) : CreateProblemDetails(result.Error);
        }

        [NonAction]
        private IActionResult CreateProblemDetails(Error error)
        {
            var statusCode = StatusCodes.Status500InternalServerError;

            switch (error.ErrorType)
            {
                case ErrorType.Validation:
                    statusCode = StatusCodes.Status400BadRequest;
                    break;
                case ErrorType.NotFound:
                    statusCode = StatusCodes.Status404NotFound;
                    break;
                case ErrorType.Conflict:
                    statusCode = StatusCodes.Status409Conflict;
                    break;
                default:
                    break;
            }

            return Problem(statusCode: statusCode, title: error.Code, detail: error.Description);
        }
    }
}
