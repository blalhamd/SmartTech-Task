using EduNexus.Domain.Entities.Business;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EduNexus.Infrastructure.Data.EntitiesConfiguration
{
    public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder.ToTable("Employees")
                .HasKey(e => e.Id);

            builder.Property(e => e.Id)
                .HasDefaultValueSql("NEWID()");

            builder.Property(e => e.FullName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(e => e.Salary)
                .HasPrecision(10, 2);

            builder.Property(e => e.Position)
                .IsRequired()
                .HasMaxLength(100);
            
            builder.HasIndex(e => e.FullName)
                .IsUnique()
                .HasFilter("[IsDeleted] = 0"); 

            builder.HasIndex(e => e.UserId).IsUnique();

            builder.HasOne(e => e.User)
                .WithOne()
                .HasForeignKey<Employee>(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
