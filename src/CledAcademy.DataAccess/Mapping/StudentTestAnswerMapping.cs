using CledAcademy.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CledAcademy.DataAccess.Mapping
{
    public class StudentTestAnswerMapping
    {
        public StudentTestAnswerMapping(EntityTypeBuilder<StudentTestAnswer> entityBuilder)
        {
            entityBuilder.ToTable("StudentTestAnswers");

            entityBuilder.HasKey(sta => sta.Id);
            entityBuilder.HasIndex(sta => new { sta.StudentId, sta.AnswerId }).IsUnique();

            //configuring many to many
            entityBuilder
                .HasOne(sta => sta.Answer)
                .WithMany(a => a.StudentTestAnswers)
                .HasForeignKey(sta => sta.AnswerId);

            entityBuilder
                .HasOne(sta => sta.Student)
                .WithMany(s => s.StudentTestAnswers)
                .HasForeignKey(sta => sta.StudentId);
        }
    }
}