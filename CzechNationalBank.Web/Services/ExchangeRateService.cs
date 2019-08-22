using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using CzechNationalBank.Entities;
using CzechNationalBank.Persistence;
using CzechNationalBank.Web.Infrastructure.Reports.Abstractions;
using CzechNationalBank.Web.Services.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CzechNationalBank.Web.Services
{
    /// <inheritdoc />
    public class ExchangeRateService : IExchangeRateService
    {
        private readonly DatabaseContext _context;
        private readonly List<string> _currencies = new List<string>();
        private readonly IReportBuildersFactory _buildersFactory;

        /// <inheritdoc />
        public ExchangeRateService(DatabaseContext context, IConfiguration configuration, IReportBuildersFactory buildersFactory)
        {
            _context = context;
            _buildersFactory = buildersFactory;
            configuration.GetSection("Currencies").Bind(_currencies);
        }

        /// <inheritdoc />
        public async Task<ExportFileModel> BuildReport(DateTimeOffset date, ExportFormatOption format)
        {
            var exchangeRates = await _context.ExchangeRates.AsNoTracking()
                .Where(a => a.Date.Year == date.Year && a.Date.Month == date.Month)
                .Where(a => _currencies.Contains(a.Code))
                .ToListAsync();
            
            var reportModel = CreateReportModel(exchangeRates, date);

            return await _buildersFactory.DefineBuilder<ExchangeRatesReportModel>(format)
                .BuildReport(reportModel);
        }

        private ExchangeRatesReportModel CreateReportModel(List<ExchangeRate> data, DateTimeOffset date)
        {
            var reportModel = new ExchangeRatesReportModel
            {
                Meta = new ExchangeRatesReportModel.MetaInformation
                {
                    Year = date.Year,
                    Month = $"{date:MMMM}"
                },
                WeeksInformation = new List<ExchangeRatesReportModel.WeekInformation>()
            };
            
            var groupingData = data.GroupBy(a =>
                    CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(a.Date.DateTime, CalendarWeekRule.FirstDay, DayOfWeek.Monday))
                .OrderBy(a => a.Key);
            
            foreach (var weekGroup in groupingData)
            {
                var weekModel = new ExchangeRatesReportModel.WeekInformation
                {
                    PeriodBegin = weekGroup.Min(a => a.Date),
                    PeriodEnd = weekGroup.Max(a => a.Date),
                    CurrenciesInformation = new List<ExchangeRatesReportModel.WeekInformation.CurrencyInformation>()
                };
                
                foreach (var codeGroup in weekGroup.GroupBy(a => a.Code).OrderBy(a => a.Key))
                {
                    var exchangeRates = codeGroup.Select(a => a.Rate).OrderByDescending(a => a).ToList();
                    
                    var currencyModel = new ExchangeRatesReportModel.WeekInformation.CurrencyInformation
                    {
                        Code = codeGroup.Key,
                        Max = exchangeRates.First(),
                        Min = exchangeRates.Last(),
                        Median = exchangeRates[exchangeRates.Count / 2]
                    };
                    
                    weekModel.CurrenciesInformation.Add(currencyModel);
                }
                
                reportModel.WeeksInformation.Add(weekModel);
            }

            return reportModel;
        }
    }
}