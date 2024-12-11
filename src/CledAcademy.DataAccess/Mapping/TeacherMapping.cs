using CledAcademy.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MySQL.Data.EntityFrameworkCore.Extensions;

namespace CledAcademy.DataAccess.Mapping
{
    public class TeacherMapping
    {
        public TeacherMapping(EntityTypeBuilder<Teacher> entityBuilder)
        {
            entityBuilder.ToTable("Teachers");

            entityBuilder.HasKey(t => t.Id);

            entityBuilder.Property(t => t.Profession).HasMaxLength(100).IsRequired();
            entityBuilder.Property(t => t.About).ForMySQLHasColumnType("varchar(3000)").IsRequired();
            entityBuilder.Property(t => t.FacebookProfile).HasMaxLength(100);
            entityBuilder.Property(t => t.SkypeProfile).HasMaxLength(100);
            entityBuilder.Property(t => t.TwitterProfile).HasMaxLength(100);
            entityBuilder.Property(t => t.FullName).HasMaxLength(40).IsRequired();
            entityBuilder.Property(t => t.ImageUrl).HasMaxLength(100);
        }
    }
}