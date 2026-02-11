using EduNexus.Domain.Entities.Business;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EduNexus.Infrastructure.Data.EntitiesConfiguration
{
    public class EmployeeRequestConfiguration : IEntityTypeConfiguration<EmployeeRequest>
    {
        public void Configure(EntityTypeBuilder<EmployeeRequest> builder)
        {
            builder.ToTable("EmployeeRequests")
                .HasKey(e => e.Id);

            builder.Property(e => e.Id)
                .HasDefaultValueSql("NEWID()");

            builder.Property(e => e.ActionType)
                  .HasConversion<string>()
                  .HasMaxLength(50)
                  .IsRequired();

            builder.Property(e => e.Status)
                 .HasConversion<string>()
                 .HasMaxLength(50)
                 .IsRequired();

            builder.Property(e => e.RejectionReason)
                 .HasMaxLength(500);

            builder.HasOne(er => er.Employee)
               .WithMany()
               .HasForeignKey(er => er.EmployeeId)
               .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
