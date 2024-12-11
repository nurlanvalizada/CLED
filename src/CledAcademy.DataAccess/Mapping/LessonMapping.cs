using CledAcademy.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CledAcademy.DataAccess.Mapping
{
    public class LessonMapping
    {
        public LessonMapping(EntityTypeBuilder<Lesson> entityBuilder)
        {
            entityBuilder.ToTable("Lessons");

            entityBuilder.HasKey(l => l.Id);

            entityBuilder.Property(l => l.Name).HasMaxLength(50).IsRequired();
            entityBuilder.Property(l => l.VideoUrl).HasMaxLength(100);

            //configuring one to many between entities
            entityBuilder
               .HasOne(l => l.Section)
               .WithMany(s => s.Lessons)
               .HasForeignKey(l => l.SectionId)
               .OnDelete(DeleteBehavior.Restrict);
        }
    }
}