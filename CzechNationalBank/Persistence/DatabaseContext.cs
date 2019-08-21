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
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //TODO: убрать дублирование строки подключения
            optionsBuilder.UseNpgsql("Host=localhost;Username=postgres;Password=root;Database=CzechNationalBank");

            base.OnConfiguring(optionsBuilder);
        }

        /// <inheritdoc />
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ExchangeRateConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}