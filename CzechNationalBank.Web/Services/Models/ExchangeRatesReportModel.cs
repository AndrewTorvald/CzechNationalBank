using System;
using System.Collections.Generic;

namespace CzechNationalBank.Web.Services.Models
{
    /// <summary>
    /// Модель для сборки отчета по валютным курсам
    /// </summary>
    public class ExchangeRatesReportModel
    {
        public MetaInformation Meta { get; set; }
        public List<WeekInformation> WeeksInformation { get; set; }
        
        public class MetaInformation
        {
            public int Year { get; set; }
            public string Month { get; set; }
        }

        public class WeekInformation
        {
            public DateTimeOffset PeriodBegin { get; set; }
            public DateTimeOffset PeriodEnd { get; set; }
            public List<CurrencyInformation> CurrenciesInformation { get; set; }
            
            public class CurrencyInformation
            {
                public string Code { get; set; }
                public decimal Max { get; set; }
                public decimal Median { get; set; }
                public decimal Min { get; set; }
            }
        }
    }
}