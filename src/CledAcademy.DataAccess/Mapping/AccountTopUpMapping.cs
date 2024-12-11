using CledAcademy.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CledAcademy.DataAccess.Mapping
{
    public class AccountTopUpMapping
    {
        public AccountTopUpMapping(EntityTypeBuilder<AccountTopUp> entityBuilder)
        {
            entityBuilder.ToTable("AccountTopUps");

            entityBuilder.HasKey(at => at.Id);

            entityBuilder.HasIndex(at => at.StudentId);
            entityBuilder.HasIndex(at => at.PaymentKey);

            entityBuilder.Property(at => at.DateTime).IsRequired();
            entityBuilder.Property(at => at.CardNumber).HasMaxLength(16);
            entityBuilder.Property(at => at.PaymentKey).HasMaxLength(50).IsRequired();
            entityBuilder.Property(at => at.Rrn).HasMaxLength(20);
            entityBuilder.Property(at => at.StatusCode).HasMaxLength(100);

            //configuring relatiopnships
            entityBuilder.HasOne(at => at.Student).WithMany(s => s.AccountTopUps).HasForeignKey(at => at.StudentId);
        }
    }
}