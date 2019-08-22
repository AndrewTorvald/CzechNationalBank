using System.IO;
using System.Text;
using System.Threading.Tasks;
using CzechNationalBank.Web.Infrastructure.Reports.Abstractions.Builders;
using CzechNationalBank.Web.Services.Models;

namespace CzechNationalBank.Web.Infrastructure.Reports.Builders
{
    /// <inheritdoc />
    public class TxtReportBuilder : ITxtReportBuilder<ExchangeRatesReportModel>
    {
        /// <inheritdoc />
        public async Task<ExportFileModel> BuildReport(ExchangeRatesReportModel reportModel)
        {
            var textBuilder = new StringBuilder();

            textBuilder.AppendLine($"Year: {reportModel.Meta.Year}, month: {reportModel.Meta.Month}");
            textBuilder.AppendLine($"Week periods:");
            
            reportModel.WeeksInformation.ForEach(weekInfo =>
            {
                textBuilder.Append($"{weekInfo.PeriodBegin.Day}..{weekInfo.PeriodEnd.Day}:");
                
                weekInfo.CurrenciesInformation.ForEach(currency =>
                {
                    textBuilder.Append($" {currency.Code} - max: {currency.Max}, min: {currency.Min}, median: {currency.Median};");
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
    }
}