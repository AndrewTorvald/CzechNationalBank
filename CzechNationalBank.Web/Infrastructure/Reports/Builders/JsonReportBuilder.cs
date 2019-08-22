using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using CzechNationalBank.Web.Infrastructure.Reports.Abstractions.Builders;
using CzechNationalBank.Web.Services.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace CzechNationalBank.Web.Infrastructure.Reports.Builders
{
    /// <inheritdoc />
    public class JsonReportBuilder : IJsonReportBuilder<ExchangeRatesReportModel>
    {
        /// <inheritdoc />
        public async Task<ExportFileModel> BuildReport(ExchangeRatesReportModel reportModel)
        {
            var jsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
            };
            jsonSerializerSettings.Converters.Add(new IsoDateTimeConverter 
            { 
                DateTimeFormat = "dd-MM-yyyy", 
                DateTimeStyles = DateTimeStyles.AdjustToUniversal 
            });
            
            var jsonString = JsonConvert.SerializeObject(reportModel, jsonSerializerSettings);
            
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
    }
}