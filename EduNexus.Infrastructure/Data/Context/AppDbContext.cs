using EduNexus.Domain.Entities.Base;
using EduNexus.Domain.Entities.Business;
using EduNexus.Domain.Entities.Identity;
using EduNexus.Shared.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Security.Claims;

namespace EduNexus.Infrastructure.Data.Context
{
    public class AppDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
         public DbSet<Employee> Employees { get; set; } 
         public DbSet<EmployeeRequest> EmployeeRequests { get; set; } 

        private readonly IHttpContextAccessor _contextAccessor;
        public AppDbContext(DbContextOptions<AppDbContext> options, IHttpContextAccessor contextAccessor) : base(options)
        {
            _contextAccessor = contextAccessor;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ---------------------------------------------------------------------------------
            // 2. CONFIGURE IDENTITY TABLE NAMES AND SCHEMA
            // ---------------------------------------------------------------------------------
            const string identitySchema = "Security";                

            modelBuilder.Entity<ApplicationRole>()
                .ToTable(name: "Roles", schema: identitySchema);

            // This is the join table between Users and Roles
            modelBuilder.Entity<IdentityUserRole<Guid>>()
                .ToTable(name: "UserRoles", schema: identitySchema);

            modelBuilder.Entity<IdentityUserClaim<Guid>>()
                .ToTable(name: "UserClaims", schema: identitySchema);

            modelBuilder.Entity<IdentityUserLogin<Guid>>()
                .ToTable(name: "UserLogins", schema: identitySchema);

            modelBuilder.Entity<IdentityRoleClaim<Guid>>()
                .ToTable(name: "RoleClaims", schema: identitySchema);

            modelBuilder.Entity<IdentityUserToken<Guid>>()
                .ToTable(name: "UserTokens", schema: identitySchema);


            // ---------------------------------------------------------------------------------
            // 4. DYNAMIC GLOBAL QUERY FILTER FOR SOFT DELETES (ISoftDeletable)
            // ---------------------------------------------------------------------------------

            ApplyGlobalSoftDeleteQueryFilter(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }

        public override async Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            var now = DateTime.UtcNow;
            var currentUserId = _contextAccessor.HttpContext?
                                .User?
                                .FindFirstValue(ClaimTypes.NameIdentifier);

            var ip = _contextAccessor.HttpContext?
                     .Connection
                     .RemoteIpAddress?
                     .ToString();

            var tracked = ChangeTracker
                .Entries<BaseEntity>()
                .Where(e => e.State is EntityState.Added
                                    or EntityState.Modified
                                    or EntityState.Deleted);

            Guid? userGuid = null;
            if (Guid.TryParse(currentUserId, out Guid guid))
                userGuid = guid;

            foreach (var e in tracked)
            {
                // ---------- stamps ----------
                switch (e.State)
                {
                    case EntityState.Added:
                        e.Property(p => p.CreatedAt).CurrentValue = now;
                        e.Property(p => p.CreatedBy).CurrentValue = userGuid;
                        break;

                    case EntityState.Modified:
                        if (e.Properties.Any(p => p.IsModified))
                        {
                            e.Property(p => p.UpdatedAt).CurrentValue = now;
                            e.Property(p => p.UpdatedBy).CurrentValue = userGuid;
                        }
                        break;

                    case EntityState.Deleted when e.Entity is ISoftDeletable soft:
                        e.State = EntityState.Modified; // soft-delete
                        e.Entity.MarkAsDeleted(userGuid ?? Guid.Empty);
                        break;
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }

        private void ApplyGlobalSoftDeleteQueryFilter(ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                // Check if the entity implements ISoftDeletable
                // (Assuming ISoftDeletable is in your BaseEntity)
                if (typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType)
                     && entityType.BaseType == null)
                {
                    // 1. Create the parameter (e.g., "e")
                    var parameter = Expression.Parameter(entityType.ClrType, "e");

                    // 2. Create the property access (e.g., "e.IsDeleted")
                    // Using nameof(ISoftDeletable.IsDeleted) is safer than the string "IsDeleted"
                    var property = Expression.Property(parameter, nameof(ISoftDeletable.IsDeleted));

                    // 3. Create the "false" constant
                    var falseConstant = Expression.Constant(false);

                    // 4. Create the expression body (e.g., "e.IsDeleted == false")
                    var body = Expression.Equal(property, falseConstant);

                    // 5. Create the Lambda Expression (e.g., "e => e.IsDeleted == false")
                    var lambda = Expression.Lambda(body, parameter);

                    // 6. Apply the filter
                    modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
                }
            }
        }
    }
}
