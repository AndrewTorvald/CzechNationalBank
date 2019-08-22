using System;
using System.IO;
using CzechNationalBank.Persistence;
using CzechNationalBank.Web.Infrastructure.Reports;
using CzechNationalBank.Web.Infrastructure.Reports.Abstractions;
using CzechNationalBank.Web.Infrastructure.Reports.Abstractions.Builders;
using CzechNationalBank.Web.Infrastructure.Reports.Builders;
using CzechNationalBank.Web.Services;
using CzechNationalBank.Web.Services.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;

namespace CzechNationalBank.Web
{
    /// <summary/>
    public class Startup
    {
        private readonly IConfiguration _configuration;

        /// <summary/>
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary/>
        public void ConfigureServices(IServiceCollection services)
        {
            // Presentation
            services.AddMvc();

            services.AddSwaggerGen(a =>
            {
                a.SwaggerDoc("v1", new Info {Title = "Документация API", Version = "v1"});
                a.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "CzechNationalBank.Web.xml"));
                a.DescribeAllEnumsAsStrings();
            });
            
            // Infrastructure
            services.AddDbContext<DatabaseContext>(options =>
            {
                options.UseNpgsql(_configuration["Storage:ConnectionString"],
                    builder => { builder.MigrationsAssembly("CzechNationalBank"); });
            });
            
            services.AddScoped<IReportBuildersFactory, ReportBuildersFactory>();
            services.AddScoped<ITxtReportBuilder<ExchangeRatesReportModel>, TxtReportBuilder>();
            services.AddScoped<IJsonReportBuilder<ExchangeRatesReportModel>, JsonReportBuilder>();
            
            // Application
            services.AddScoped<IExchangeRateService, ExchangeRateService>();
        }

        /// <summary/>
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