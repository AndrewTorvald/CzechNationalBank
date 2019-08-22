using CzechNationalBank.Entities;
using CzechNationalBank.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace CzechNationalBank.Persistence
{
    /// <inheritdoc />
    public class DatabaseContext : DbContext
    {
        /// <summary>
        /// Валютные курсы
        /// </summary>
        public DbSet<ExchangeRate> ExchangeRates { get; set; }
        
        /// <inheritdoc />
        protected DatabaseContext()
        {
        }

        /// <inheritdoc />
        public DatabaseContext(DbContextOptions options) : base(options)
        {
        }

        /// <inheritdoc />
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ExchangeRateConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}