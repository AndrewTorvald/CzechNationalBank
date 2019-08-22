using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CzechNationalBank.Web.Utils
{
    /// <summary>
    /// Методы расширения для IWebHost
    /// </summary>
    public static class WebHostExtensions
    {
        /// <summary>
        /// Миграция БД
        /// </summary>
        /// <typeparam name="TContext">Тип контекста БД</typeparam>
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