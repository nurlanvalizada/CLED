using CledAcademy.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CledAcademy.DataAccess.Mapping
{
    public class StudentOrderMapping
    {
        public StudentOrderMapping(EntityTypeBuilder<StudentOrder> entityBuilder)
        {
            entityBuilder.ToTable("StudentOrders");

            entityBuilder.HasKey(so => so.Id);

            entityBuilder.HasIndex(so => new { so.StudentId, so.OrderId, so.OrderType}).IsUnique();

            entityBuilder.Property(so => so.DateTime).IsRequired();

            //configuring many to many
            entityBuilder
                .HasOne(so => so.Student)
                .WithMany(s => s.StudentOrders)
                .HasForeignKey(so => so.StudentId);
        }
    }
}