using CledAcademy.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MySQL.Data.EntityFrameworkCore.Extensions;

namespace CledAcademy.DataAccess.Mapping
{
    public class NewsMapping
    {
        public NewsMapping(EntityTypeBuilder<News> entityBuilder)
        {
            entityBuilder.ToTable("News");

            entityBuilder.HasKey(n => n.Id);
            entityBuilder.Property(n => n.ImageUrl).HasMaxLength(100);
            entityBuilder.Property(n => n.ShortContent).ForMySQLHasColumnType("varchar(500)").IsRequired();
            entityBuilder.Property(n => n.Content).ForMySQLHasColumnType("text").IsRequired();
            entityBuilder.Property(n => n.Title).HasMaxLength(100).IsRequired();
            entityBuilder.Property(n => n.PublishDate).IsRequired();

            //configuring one to many between entities
            entityBuilder.HasOne(n => n.Admin)
                .WithMany(a => a.Newses)
                .HasForeignKey(n => n.AdminId);
        }
    }
}