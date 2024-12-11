using CledAcademy.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MySQL.Data.EntityFrameworkCore.Extensions;

namespace CledAcademy.DataAccess.Mapping
{
    public class CourseMapping
    {
        public CourseMapping(EntityTypeBuilder<Course> entityBuilder)
        {
            entityBuilder.ToTable("Courses");

            entityBuilder.HasKey(c => c.Id);

            entityBuilder.Property(c => c.Name).HasMaxLength(100);
            entityBuilder.Property(c => c.Description).ForMySQLHasColumnType("varchar(3000)");
            entityBuilder.Property(c => c.ImageUrl).HasMaxLength(100);
            entityBuilder.Property(c => c.VideoUrl).HasMaxLength(100);
            entityBuilder.Property(c => c.TeacherId).IsRequired(false);

            //configuring one to many between entities
            entityBuilder.HasOne(c => c.Teacher)
                .WithMany(t => t.Courses)
                .HasForeignKey(c => c.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}