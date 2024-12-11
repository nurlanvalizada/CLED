using CledAcademy.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MySQL.Data.EntityFrameworkCore.Extensions;

namespace CledAcademy.DataAccess.Mapping
{
    public class SectionMapping
    {
        public SectionMapping(EntityTypeBuilder<Section> entityBuilder)
        {
            entityBuilder.ToTable("Sections");

            entityBuilder.HasKey(s=> s.Id);

            entityBuilder.Property(s => s.Name).HasMaxLength(60).IsRequired();
            entityBuilder.Property(c => c.Description).ForMySQLHasColumnType("varchar(2000)");

            //configuring one to many between entities
            entityBuilder
                .HasOne(s => s.Course)
                .WithMany(s => s.Sections)
                .HasForeignKey(s => s.CourseId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}