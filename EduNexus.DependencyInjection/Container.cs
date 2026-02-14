using EduNexus.Business.Services;
using EduNexus.Business.Services.Notification;
using EduNexus.Core.Constants;
using EduNexus.Core.IRepositories.Generic;
using EduNexus.Core.IRepositories.Non_Generic;
using EduNexus.Core.IServices;
using EduNexus.Core.IServices.Notification;
using EduNexus.Core.IUnit;
using EduNexus.Core.Models.V1.Validators;
using EduNexus.Domain.Entities.Identity;
using EduNexus.Infrastructure.Data.Context;
using EduNexus.Infrastructure.Repositories.Generic;
using EduNexus.Infrastructure.Repositories.Non_Generic;
using EduNexus.Infrastructure.Unit;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EduNexus.DependencyInjection
{
    public static class Container
    {

        public static IServiceCollection RegisterConfiguartion(this IServiceCollection services, IConfiguration configuration)
        {
            services.RegisterConnectionString(configuration)
                    .RegisterIdentity()
                    .RegisterFluentValidation()
                    .RegisterServices()
                    .RegisterRepositories()
                    .RegisterUnitOfWork();

            return services;
        }

        private static IServiceCollection RegisterConnectionString(this IServiceCollection services, IConfiguration configuration)
        {
            var connection = configuration["ConnectionStrings:DefaultConnectionString"];
            services.AddDbContext<AppDbContext>(x => x.UseSqlServer(connection));
            services.AddScoped<AppDbContext>();

            return services;
        }

        private static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IJwtProvider, JwtProvider>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IEmployeeRequestService, EmployeeRequestService>();
            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<INotificationServiceFactory, NotificationServiceFactory>();
            services.AddKeyedScoped<INotificationService, EmailNotificationService>(ServiceKey.Email_Key);
            services.AddKeyedScoped<INotificationService, SmsNotificationService>(ServiceKey.SMS_Key);

            return services;
        }


        private static IServiceCollection RegisterRepositories(this IServiceCollection services)
        {
            services.AddScoped(typeof(IGenericRepositoryAsync<>), typeof(GenericRepositoryAsync<>));
            services.AddScoped<IEmployeeRepositoryAsync, EmpolyeeRepositoryAsync>();
            services.AddScoped<IEmployeeRequestRepositoryAsync, EmpolyeeRequestRepositoryAsync>();

            return services;
        }

        private static IServiceCollection RegisterUnitOfWork(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWorkAsync, UnitOfWorkAsync>();

            return services;
        }

        private static IServiceCollection RegisterIdentity(this IServiceCollection services)
        {
            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                // Password policy
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 5;

                // User settings
                options.User.RequireUniqueEmail = true;
            })
              .AddEntityFrameworkStores<AppDbContext>() // store in DB
              .AddDefaultTokenProviders(); // for password reset, email confirm, etc.

            return services;
        }

        private static IServiceCollection RegisterFluentValidation(this IServiceCollection services)
        {
            services.AddValidatorsFromAssemblyContaining<UpdateEmployeeRequestDtoValidator>();

            return services;
        }
    }
}
