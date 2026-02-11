using EduNexus.Core.Constants;
using EduNexus.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace EduNexus.Infrastructure.SeedData
{
    public static class Seed
    {
        public static async Task InitializeAsync(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager)
        {
            await SeedRolesAsync(roleManager);
            await SeedMakerUserAsync(userManager);
            await SeedCheckerUserAsync(userManager);
        }

        private static async Task SeedRolesAsync(RoleManager<ApplicationRole> roleManager)
        {
            var rolePermissions = new Dictionary<string, List<string>>
            {
                { ApplicationRoles.Maker, GetClaimsForMaker() },
                { ApplicationRoles.Checker, GetClaimsForChecker() },
                { ApplicationRoles.Employee, GetClaimsForEmployee() }
            };

            foreach (var entry in rolePermissions)
            {
                var roleName = entry.Key;
                var permissions = entry.Value;

                // 1. تأكد أن الدور موجود، ولو مش موجود انشئه
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new ApplicationRole
                    {
                        Name = roleName,
                        NormalizedName = roleName.ToUpper(),
                        CreatedAt = DateTime.UtcNow
                    });
                }

                // 2. هات الدور من الداتابيز عشان نضيف له الصلاحيات
                var role = await roleManager.FindByNameAsync(roleName);

                // 3. هات الصلاحيات الحالية اللي مع الدور ده عشان منكررش
                var existingClaims = await roleManager.GetClaimsAsync(role!);

                foreach (var permission in permissions)
                {
                    // لو الصلاحية دي مش معاه.. ضيفها
                    // (Check if the role already has this specific claim)
                    if (!existingClaims.Any(c => c.Type == Permissions.Type && c.Value == permission))
                    {
                        await roleManager.AddClaimAsync(role!, new Claim(Permissions.Type, permission));
                    }
                }
            }
        }

        private static async Task SeedMakerUserAsync(UserManager<ApplicationUser> userManager)
        {
            var makerUser = await userManager.FindByEmailAsync(DefaultUser.Maker.Email);

            if (makerUser == null)
            {
                makerUser = new ApplicationUser
                {
                    FullName = DefaultUser.Maker.FullName,
                    Email = DefaultUser.Maker.Email,
                    UserName = DefaultUser.Maker.UserName,
                    EmailConfirmed = true,
                    CreatedAt = DateTime.UtcNow,
                };

                var result = await userManager.CreateAsync(makerUser, DefaultUser.Maker.Password);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(makerUser, ApplicationRoles.Maker);
                }
            }
        }

        private static async Task SeedCheckerUserAsync(UserManager<ApplicationUser> userManager)
        {
            var checkerUser = await userManager.FindByEmailAsync(DefaultUser.Checker.Email);

            if (checkerUser == null)
            {
                checkerUser = new ApplicationUser
                {
                    FullName = DefaultUser.Checker.FullName,
                    Email = DefaultUser.Checker.Email,
                    UserName = DefaultUser.Checker.UserName,
                    EmailConfirmed = true,
                    CreatedAt = DateTime.UtcNow,
                };

                var result = await userManager.CreateAsync(checkerUser, DefaultUser.Checker.Password);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(checkerUser, ApplicationRoles.Checker);
                }
            }
        }


        private static List<string> GetClaimsForMaker()
        {
            return new List<string>
            {
                Permissions.EmployeeRequest.View,
                Permissions.EmployeeRequest.Create,
                Permissions.EmployeeRequest.Update,
                Permissions.EmployeeRequest.Delete,
                Permissions.Employee.View,
                Permissions.Employee.ViewDetails
            };
        }

        private static List<string> GetClaimsForChecker()
        {
            return new List<string>
            {
                Permissions.EmployeeRequest.View,
                Permissions.EmployeeRequest.Approve,
                Permissions.EmployeeRequest.Reject,
                Permissions.Employee.View,
                Permissions.Employee.ViewDetails
            };
        }

        private static List<string> GetClaimsForEmployee()
        {
            return new List<string>
            {
                Permissions.Employee.View
            };
        }
    }
}
