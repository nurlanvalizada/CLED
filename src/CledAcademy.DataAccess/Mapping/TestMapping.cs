using CledAcademy.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MySQL.Data.EntityFrameworkCore.Extensions;

namespace CledAcademy.DataAccess.Mapping
{
    public class TestMapping
    {
        public TestMapping(EntityTypeBuilder<Test> entityBuilder)
        {
            entityBuilder.ToTable("Tests");

            entityBuilder.HasKey(t => t.Id);

            entityBuilder.Property(t => t.Content).ForMySQLHasColumnType("text");

            //configuring one to one relationships between entities
            entityBuilder
                .HasOne(t => t.TestAnswer)
                .WithOne(ta => ta.Test)
                .OnDelete(DeleteBehavior.Restrict);

            entityBuilder
              .HasOne(t => t.Course)
              .WithMany(c => c.Tests)
              .OnDelete(DeleteBehavior.Restrict);
        }
    }
}