using CledAcademy.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MySQL.Data.EntityFrameworkCore.Extensions;

namespace CledAcademy.DataAccess.Mapping
{
    public class AnswerMapping
    {
        public AnswerMapping(EntityTypeBuilder<Answer> entityBuilder)
        {
            entityBuilder.ToTable("Answers");

            entityBuilder.HasKey(a => a.Id);

            entityBuilder.Property(a => a.Text).ForMySQLHasColumnType("varchar(10000)").IsRequired();

            //configuring one to many between entities
            entityBuilder.HasOne(a => a.Test)
                .WithMany(t => t.Answers)
                .HasForeignKey(a => a.TestId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}