using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CzechNationalBank.Entities;
using CzechNationalBank.Persistence;
using CzechNationalBank.Web.Services.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CzechNationalBank.Web.Services
{
    public class ReportService
    {
        private readonly DatabaseContext _context;
        private readonly List<string> _currencies = new List<string>();

        public ReportService(DatabaseContext context, IConfiguration configuration)
        {
            _context = context;
            configuration.GetSection("Currencies").Bind(_currencies);
        }

        public async Task<ExportFileModel> BuildExchangeRatesReport(DateTimeOffset date, string format)
        {
            var data = await _context.ExchangeRates.AsNoTracking()
                .Where(a => a.Date.Year == date.Year && a.Date.Month == date.Month)
                .Where(a => _currencies.Contains(a.Code))
                .OrderBy(a => a.Date)
                .GroupBy(a => 
                    CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(a.Date.DateTime, CalendarWeekRule.FirstDay, DayOfWeek.Monday))
                .ToListAsync();

            var reportModel = CreateReportModel(data, date);

            switch (format)
            {
                case "json" :
                    return CreateJsonReport(reportModel);
                case "txt" :
                    return CreateTxtReport(reportModel);
                default :
                    throw new ArgumentOutOfRangeException();
            }
        }

        private ExportFileModel CreateJsonReport(ReportModel reportModel)
        {
            var jsonString = JsonConvert.SerializeObject(reportModel, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
            });
            var buffer = Encoding.Default.GetBytes(jsonString);
            
            var memoryStream = new MemoryStream();
            memoryStream.Write(buffer, 0, buffer.Length);
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new ExportFileModel
            {
                Stream = memoryStream,
                FileName = "report.json"
            };
        }

        private ExportFileModel CreateTxtReport(ReportModel reportModel)
        {
            var textBuilder = new StringBuilder();

            textBuilder.AppendLine($"Year: {reportModel.Meta.Year}, month: {reportModel.Meta.Month}");
            textBuilder.AppendLine($"Week periods:");
            
            reportModel.Weeks.ForEach(a =>
            {
                textBuilder.Append($"{a.Period}:");
                
                a.Currencies.ForEach(c =>
                {
                    textBuilder.Append($" {c.Code} - max: {c.Max}, min: {c.Min}, median: {c.Median};");
                });
                
                textBuilder.AppendLine();
            });

            var buffer = Encoding.Default.GetBytes(textBuilder.ToString());
            
            var memoryStream = new MemoryStream();
            memoryStream.Write(buffer, 0, buffer.Length);
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new ExportFileModel
            {
                Stream = memoryStream,
                FileName = "report.txt"
            };
        }

        private ReportModel CreateReportModel(List<IGrouping<int, ExchangeRate>> data, DateTimeOffset date)
        {
            var reportModel = new ReportModel
            {
                Meta = new ReportModel.MetaInformation
                {
                    Year = date.Year,
                    Month = $"{date:MMMM}"
                },
                Weeks = new List<ReportModel.Week>()
            };
            
            foreach (var weekGroup in data)
            {
                var weekDays = weekGroup.Select(a => a.Date.Day).ToList();
                
                var weekModel = new ReportModel.Week
                {
                    Period = $"{weekDays.Min()}..{weekDays.Max()}",
                    Currencies = new List<ReportModel.Week.Currency>()
                };
                
                foreach (var codeGroup in weekGroup.GroupBy(a => a.Code).OrderBy(a => a.Key))
                {
                    var exchangeRates = codeGroup.Select(a => a.Rate).OrderByDescending(a => a).ToList();
                    
                    var currencyModel = new ReportModel.Week.Currency
                    {
                        Code = codeGroup.Key,
                        Max = exchangeRates.First(),
                        Min = exchangeRates.Last(),
                        Median = exchangeRates[exchangeRates.Count / 2]
                    };
                    
                    weekModel.Currencies.Add(currencyModel);
                }
                
                reportModel.Weeks.Add(weekModel);
            }

            return reportModel;
        }
    }
}