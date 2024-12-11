using CledAcademy.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CledAcademy.DataAccess.Mapping
{
    public class StudentLessonMapping
    {
        public StudentLessonMapping(EntityTypeBuilder<StudentLesson> entityBuilder)
        {
            entityBuilder.ToTable("StudentLessons");

            entityBuilder.HasKey(sl => sl.Id);
            entityBuilder.HasIndex(sl => new { sl.StudentId, sl.LessonId }).IsUnique();

            entityBuilder.Property(sl => sl.StartDate).IsRequired();
            entityBuilder.Property(sl => sl.LastVisitDateTime).IsRequired(false);
            entityBuilder.Property(sl => sl.VisitCounts).IsRequired();

            //configuring many to many
            entityBuilder
                .HasOne(sl => sl.Student)
                .WithMany(s => s.StudentLessons)
                .HasForeignKey(sl => sl.StudentId);

            entityBuilder
                .HasOne(sl => sl.Lesson)
                .WithMany(l => l.StudentLessons)
                .HasForeignKey(sl => sl.LessonId);
        }
    }
}