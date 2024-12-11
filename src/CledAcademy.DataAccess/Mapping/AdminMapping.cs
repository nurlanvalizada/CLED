using CledAcademy.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CledAcademy.DataAccess.Mapping
{
    public class AdminMapping
    {
        public AdminMapping(EntityTypeBuilder<Admin> entityBuilder)
        {
            entityBuilder.ToTable("Admins");

            entityBuilder.HasKey(a => a.Id);
        }
    }
}