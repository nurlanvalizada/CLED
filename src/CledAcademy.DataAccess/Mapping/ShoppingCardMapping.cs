using CledAcademy.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CledAcademy.DataAccess.Mapping
{
    public class ShoppingCardMapping
    {
        public ShoppingCardMapping(EntityTypeBuilder<ShoppingCard> entityBuilder)
        {
            entityBuilder.ToTable("ShoppingCard");

            entityBuilder.HasKey(s => s.Id);

            entityBuilder.HasIndex(sc => new { sc.OrderId, sc.OrderType }).IsUnique();

            //configuring mapping between entities
            entityBuilder.HasOne(sc => sc.Student)
                .WithMany(s => s.ShoppingCards)
                .HasForeignKey(sc => sc.StudentId);
        }
    }
}