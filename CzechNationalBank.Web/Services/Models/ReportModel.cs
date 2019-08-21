using System.Collections.Generic;

namespace CzechNationalBank.Web.Services.Models
{
    public class ReportModel
    {
        public MetaInformation Meta { get; set; }
        public List<Week> Weeks { get; set; }
        
        public class MetaInformation
        {
            public int Year { get; set; }
            public string Month { get; set; }
        }

        public class Week
        {
            public string Period { get; set; }
            public List<Currency> Currencies { get; set; }
            
            public class Currency
            {
                public string Code { get; set; }
                public decimal Max { get; set; }
                public decimal Median { get; set; }
                public decimal Min { get; set; }
            }
        }
    }
}