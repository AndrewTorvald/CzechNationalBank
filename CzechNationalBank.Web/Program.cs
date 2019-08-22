using CzechNationalBank.Persistence;
using CzechNationalBank.Web.Utils;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace CzechNationalBank.Web
{
    /// <summary/>
    public class Program
    {
        /// <summary/>
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args)
                .Build()
                .MigrateDatabase<DatabaseContext>()
                .Run();
        }

        /// <summary/>
        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}