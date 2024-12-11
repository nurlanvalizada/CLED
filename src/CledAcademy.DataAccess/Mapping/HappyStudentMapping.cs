using CledAcademy.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MySQL.Data.EntityFrameworkCore.Extensions;

namespace CledAcademy.DataAccess.Mapping
{
    public class HappyStudentMapping
    {
        public HappyStudentMapping(EntityTypeBuilder<HappyStudent> entityBuilder)
        {
            entityBuilder.ToTable("HappyStudents");

            entityBuilder.HasKey(hs => hs.Id);

            entityBuilder.Property(hs => hs.FullName).HasMaxLength(40).IsRequired();
            entityBuilder.Property(hs => hs.ImageUrl).HasMaxLength(100);
            entityBuilder.Property(hs => hs.Review).ForMySQLHasColumnType("varchar(1000)").IsRequired();
        }
    }
}