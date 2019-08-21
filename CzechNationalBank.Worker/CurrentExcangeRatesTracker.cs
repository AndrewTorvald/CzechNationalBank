using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CzechNationalBank.Clients;
using CzechNationalBank.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CzechNationalBank.Worker
{
    /// <summary>
    /// Служба для сохранения текущего курса валют
    /// </summary>
    public class CurrentExchangeRatesTracker : IHostedService, IDisposable
    {
        private readonly INationalBankClient _client;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<CurrentExchangeRatesTracker> _logger;
        private readonly TimeSpan _dueDate;
        private readonly TimeSpan _period;
        
        private Timer _timer;

        /// <inheritdoc />
        public CurrentExchangeRatesTracker(ILogger<CurrentExchangeRatesTracker> logger, 
            IServiceProvider serviceProvider, INationalBankClient client, IConfiguration configuration)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _client = client;
            _dueDate = TimeSpan.Parse(configuration["CurrentExchangeRatesTracker:DueDate"]);
            _period = TimeSpan.Parse(configuration["CurrentExchangeRatesTracker:Period"]);
        }

        /// <inheritdoc />
        public Task StartAsync(CancellationToken cancellationToken) //TODO поддержка остановки через токен
        {
            _logger.LogInformation("Начало работы фоновой службы для сохранения текущего курса валют");

            _timer = new Timer(SaveCurrentExchangeRates, null, _dueDate, _period);

            return Task.CompletedTask;
        }

        private async void SaveCurrentExchangeRates(object state)
        {
            _logger.LogInformation($"Добавление данных за {DateTimeOffset.UtcNow.Date:d}");

            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
                var exchangeRates = await _client.GetDailyExchangeRates();

                if (exchangeRates.Any())
                {
                    var date = exchangeRates.First().Date;
                    var existingData = await context.ExchangeRates
                        .Where(a => a.Date == date)
                        .ToListAsync();

                    exchangeRates = exchangeRates.Where(a => !existingData.Any(existing => 
                        existing.Code == a.Code && existing.Date == a.Date)).ToList();
    
                    context.ExchangeRates.AddRange(exchangeRates);
                    await context.SaveChangesAsync();
                }
            }
            
            _logger.LogInformation(
                $"Следующее добавление данных произойдет в {DateTimeOffset.UtcNow.Add(_period):G}");
        }


        /// <inheritdoc />
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Завершение работы фоновой службы для сохранения текущего курса валют");

            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}