using CledAcademy.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MySQL.Data.EntityFrameworkCore.Extensions;

namespace CledAcademy.DataAccess.Mapping
{
    public class ContactMessageMapping
    {
        public ContactMessageMapping(EntityTypeBuilder<ContactMessage> entityBuilder)
        {
            entityBuilder.ToTable("ContactMessages");

            entityBuilder.HasKey(c => c.Id);
            entityBuilder.Property(c => c.FullName).HasMaxLength(40).IsRequired(false);
            entityBuilder.Property(c => c.Email).HasMaxLength(40).IsRequired();
            entityBuilder.Property(c => c.Phone).HasMaxLength(40).IsRequired(false);
            entityBuilder.Property(c => c.Subject).HasMaxLength(100).IsRequired();
            entityBuilder.Property(c => c.Message).ForMySQLHasColumnType("varchar(5000)").IsRequired();
            entityBuilder.Property(c => c.SendDate).IsRequired();
        }
    }
}