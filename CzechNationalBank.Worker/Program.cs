using System;
using System.IO;
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
    class Program
    {
        static async Task Main()
        {
            var host = new HostBuilder()
                .ConfigureLogging((hostContext, configLogging) =>
                {
                    configLogging.AddConsole();
                    configLogging.AddDebug();
                })
                .ConfigureAppConfiguration((hostContext, configApp) =>
                {
                    configApp
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", optional: false)
                        .Build();
                })
                .ConfigureServices((hostContext, services) =>
                    {
                        services.AddHttpClient<INationalBankClient, NationalBankClient>(client =>
                            client.BaseAddress = new Uri(hostContext.Configuration["ExternalServices:NationalBank"]));
                        
                        services.AddHostedService<CurrentExchangeRatesTracker>();

                        services.AddDbContext<DatabaseContext>(options =>
                        {
                            options.UseNpgsql(hostContext.Configuration["Storage:ConnectionString"],
                                builder => { builder.MigrationsAssembly("CzechNationalBank"); });
                        });
                    })
                .UseConsoleLifetime()
                .Build();
            

            using (var scope = host.Services.CreateScope())
            {
                scope.ServiceProvider.GetRequiredService<DatabaseContext>().Database.Migrate();
            }

            await host.StartAsync();
        }
    }
}
