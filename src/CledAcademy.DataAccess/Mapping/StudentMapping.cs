using CledAcademy.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MySQL.Data.EntityFrameworkCore.Extensions;

namespace CledAcademy.DataAccess.Mapping
{
    public class StudentMapping
    {
        public StudentMapping(EntityTypeBuilder<Student> entityBuilder)
        {
            entityBuilder.ToTable("Students");

            entityBuilder.HasKey(s => s.Id);
            entityBuilder.Property(s => s.DateOfBirth).ForMySQLHasColumnType("date").IsRequired();
            entityBuilder.Property(s => s.Phone).HasMaxLength(15);
        }
    }
}