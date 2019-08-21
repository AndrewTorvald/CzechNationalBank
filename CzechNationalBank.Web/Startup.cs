using System;
using System.IO;
using CzechNationalBank.Persistence;
using CzechNationalBank.Web.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;

namespace CzechNationalBank.Web
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddSwaggerGen(a =>
            {
                a.SwaggerDoc("v1", new Info {Title = "Документация API", Version = "v1"});
                a.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "CzechNationalBank.Web.xml"));
                a.DescribeAllEnumsAsStrings();
            });
            
            services.AddDbContext<DatabaseContext>();

            services.AddScoped<ReportService>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseSwagger(options => { options.RouteTemplate = "api/CzechNationalBank/swagger/{documentName}/swagger.json"; });
                app.UseSwaggerUI(a =>
                {
                    a.SwaggerEndpoint("/api/CzechNationalBank/swagger/v1/swagger.json", "CzechNationalBank API");
                    a.RoutePrefix = "api/help";
                });
            }

            app.UseMvcWithDefaultRoute();
        }
    }
}