using CzechNationalBank.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CzechNationalBank.Persistence.Configurations
{
    /// <inheritdoc />
    public class ExchangeRateConfiguration : IEntityTypeConfiguration<ExchangeRate>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<ExchangeRate> builder)
        {
            builder.HasKey(a => a.Id);
            
            builder.HasIndex(a => new {a.Date, a.Code}).IsUnique();
        }
    }
}