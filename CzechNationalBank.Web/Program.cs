using CzechNationalBank.Persistence;
using CzechNationalBank.Web.Utils;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace CzechNationalBank.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args)
                .Build()
                .MigrateDatabase<DatabaseContext>()
                .Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}