using EduNexus.Core.IServices;
using EduNexus.Core.Models.V1.Dtos.Auth;
using EduNexus.Core.Models.V1.ViewModels.Auth;
using Microsoft.AspNetCore.Mvc;

namespace EduNexus.API.Controllers
{
    [Route("api/v{version:apiVersion}/auth")]
    [ApiVersion("1.0")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpPost("login")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> LoginAsync(LoginRequest loginRequest)
            => Ok(await _authenticationService.LoginAsync(loginRequest));


        [HttpPost("forget-password")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GeneratePasswordResetTokenAsync(ForgetPasswordRequest request)
            => Ok(await _authenticationService.GeneratePasswordResetTokenAsync(request));

        [HttpPost("reset-password")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ResetPasswordAsync(ResetPasswordRequest request)
            => Ok(await _authenticationService.ResetPasswordAsync(request));

       
    }
}
