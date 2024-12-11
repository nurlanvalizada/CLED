using CledAcademy.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MySQL.Data.EntityFrameworkCore.Extensions;

namespace CledAcademy.DataAccess.Mapping
{
    public class FaqMapping
    {
        public FaqMapping(EntityTypeBuilder<Faq> entityBuilder)
        {
            entityBuilder.ToTable("Faqs");

            entityBuilder.HasKey(f => f.Id);

            entityBuilder.Property(f => f.Question).ForMySQLHasColumnType("varchar(2000)").IsRequired();
            entityBuilder.Property(f => f.Answer).ForMySQLHasColumnType("varchar(5000)").IsRequired();
        }
    }
}