using CledAcademy.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MySQL.Data.EntityFrameworkCore.Extensions;

namespace CledAcademy.DataAccess.Mapping
{
    public class DataDictionaryMapping
    {
        public DataDictionaryMapping(EntityTypeBuilder<DataDictionary> entityBuilder)
        {
            entityBuilder.ToTable("DataDictionary");

            entityBuilder.HasKey(dd => dd.Id);

            entityBuilder.Property(dd => dd.Key).HasMaxLength(20).IsRequired();
            entityBuilder.Property(dd => dd.Header).HasMaxLength(100).IsRequired();
            entityBuilder.Property(dd => dd.Value).ForMySQLHasColumnType("nvarchar(20000)").IsRequired();
        }
    }
}