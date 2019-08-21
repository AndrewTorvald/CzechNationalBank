using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CzechNationalBank.Web.Utils
{
    public static class WebHostExtensions
    {
        public static IWebHost MigrateDatabase<TContext>(this IWebHost host)
            where TContext : DbContext
        {
            using (var scope = host.Services.CreateScope())
            {
                scope.ServiceProvider.GetRequiredService<TContext>().Database.Migrate();
            }
            return host;
        }
    }
}