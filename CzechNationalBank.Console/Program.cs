using System;
using System.IO;
using System.Threading.Tasks;
using CzechNationalBank.Clients;
using CzechNationalBank.Console.UI;
using CzechNationalBank.Console.UI.Commands;
using CzechNationalBank.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CzechNationalBank.Console
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            var provider = serviceCollection.BuildServiceProvider();

            ConfigureLogging(provider);
            
            using (var scope = provider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
                dbContext.Database.Migrate();
            }

            var commandLineInterface = provider.GetService<CommandLineInterface>();
            ConfigureCommandLineInterface(commandLineInterface);
            
            await commandLineInterface.Run(args);
        }

        private static void ConfigureLogging(IServiceProvider provider)
        {
            var loggerFactory = provider.GetService<ILoggerFactory>();

            loggerFactory.AddConsole();
        }

        private static void ConfigureServices(ServiceCollection services)
        {
            // Configuration
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            // Infrastructure
            services.AddDbContext<DatabaseContext>();
            
            services.AddHttpClient<NationalBankClient>(client =>
                client.BaseAddress = new Uri(configuration["ExternalServices:NationalBank"]));

            // UI
            services.AddScoped<CommandLineInterface>();
            
            services.AddScoped<UpdateExchangeRatesCommand>();
            services.AddScoped<GetExchangeRatesCommand>();
        }
        
        private static void ConfigureCommandLineInterface(CommandLineInterface commandLineInterface)
        {
            commandLineInterface.AddCommand<UpdateExchangeRatesCommand>("update");
            commandLineInterface.AddCommand<GetExchangeRatesCommand>("get");
        }
    }
}
