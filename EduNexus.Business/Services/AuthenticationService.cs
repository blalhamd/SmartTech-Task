using EduNexus.Core.Constants;
using EduNexus.Core.IServices;
using EduNexus.Core.Models.V1.Dtos.Auth;
using EduNexus.Core.Models.V1.ViewModels.Auth;
using EduNexus.Domain.Entities.Identity;
using EduNexus.Shared.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using LoginRequest = EduNexus.Core.Models.V1.Dtos.Auth.LoginRequest;

namespace EduNexus.Business.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IJwtProvider _jwtProvider;
        private readonly ILogger<AuthenticationService> _logger;
        private readonly IValidator<LoginRequest> _loginValidator;
        private readonly IValidator<ResetPasswordRequest> _resetValidator;
        public AuthenticationService(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            IJwtProvider jwtProvider,
            ILogger<AuthenticationService> logger,
            IValidator<LoginRequest> loginValidator,
            IValidator<ResetPasswordRequest> resetValidator
            )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtProvider = jwtProvider;
            _logger = logger;
            _loginValidator = loginValidator;
            _resetValidator = resetValidator;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            // Validate request
            var validationResult = await _loginValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
                return HandleInvalidLogin();

            // Find user
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user is null)
                return HandleInvalidLogin();

            // Validate password
            if (!await _userManager.CheckPasswordAsync(user, request.Password))
                return HandleInvalidLogin();

            // Roles & permissions
            var (roles, permissions) = await GetRolesAndPermissions(user);

            // Generate token
            var tokenResult = _jwtProvider.GenerateToken(user, roles, permissions);

            await _userManager.UpdateAsync(user);

            return new LoginResponse
            {
                UserId = user.Id,
                FullName = user.FullName ?? "UnKnown",
                Email = user.Email ?? request.Email,
                AccessToken = tokenResult.Token,
                AccessTokenExpiration = tokenResult.TokenExpiration,
            };
        }

        public async Task<string> GeneratePasswordResetTokenAsync(ForgetPasswordRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user is null)
            {
                _logger.LogWarning("Password reset token generation failed: User with email {Email} not found.", request.Email);
                return string.Empty;
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            _logger.LogInformation("Password reset token generated successfully for user {Email}", request.Email);

            return token;
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordRequest request)
        {
            // Validate request
            var validationResult = await _resetValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                var errors = string.Join(',', validationResult.Errors.Select(x => x.ErrorMessage).ToList());
                _logger.LogWarning(errors);
                throw new BadRequestException(errors);
            }

            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user is null)
            {
                _logger.LogWarning("Password reset failed: User with email {Email} not found.", request.Email);
                throw new ItemNotFoundException("User not found.");
            }

            var result = await _userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(x => x.Description));
                _logger.LogWarning("Password reset failed for user {Email}. Errors: {Errors}", request.Email, errors);
                throw new BadRequestException($"Password reset failed: {errors}");
            }

            _logger.LogInformation("Password reset successfully for user {Email}", request.Email);
            return true;
        }


        private async Task<(IEnumerable<string> roles, IEnumerable<string> permissions)> GetRolesAndPermissions(ApplicationUser user)
        {
            // 1. إحضار أسماء الرتب
            var roles = await _userManager.GetRolesAsync(user);

            // استخدام HashSet لمنع تكرار الصلاحيات لو موجودة في أكثر من رتبة
            var allPermissions = new HashSet<string>();

            // 2. الدوران على الرتب لاستخراج صلاحيات كل رتبة
            foreach (var roleName in roles)
            {
                // لازم نستخدم _roleManager هنا لأن الـ userManager ميعرفش تفاصيل الرتبة
                var role = await _roleManager.FindByNameAsync(roleName);
                if (role != null)
                {
                    var roleClaims = await _roleManager.GetClaimsAsync(role);

                    foreach (var claim in roleClaims)
                    {
                        // تأكد إننا بنضيف الـ Permissions بس (حسب الثابت اللي بتستخدمه)
                        if (claim.Type == Permissions.Type)
                        {
                            allPermissions.Add(claim.Value);
                        }
                    }
                }
            }

            // 3. إحضار الصلاحيات المباشرة لليوزر (إن وجدت) ودمجها
            var userClaims = await _userManager.GetClaimsAsync(user);
            foreach (var claim in userClaims)
            {
                if (claim.Type == Permissions.Type)
                {
                    allPermissions.Add(claim.Value);
                }
            }

            return (roles, allPermissions);
        }

     
        private LoginResponse HandleInvalidLogin()
        {
            _logger.LogWarning("Invalid login attempt with provided credentials.");
            throw new BadRequestException("Invalid email or password");
        }

    }
}
