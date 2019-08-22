using System;
using System.Threading.Tasks;
using CzechNationalBank.Web.Infrastructure.Reports.Abstractions;
using CzechNationalBank.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace CzechNationalBank.Web.Controllers
{
    /// <summary>
    /// API Валютных курсов
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ExchangeRatesController : ControllerBase
    {
        private readonly IExchangeRateService _service;

        /// <inheritdoc />
        public ExchangeRatesController(IExchangeRateService service)
        {
            _service = service;
        }

        /// <summary>
        /// Получение отчета по курсу кроны
        /// </summary>
        [HttpGet("Report")]
        public async Task<FileStreamResult> GetReport([FromQuery] DateTimeOffset date, [FromQuery] ExportFormatOption format = ExportFormatOption.Txt)
        {
            var exportFileModel = await _service.BuildReport(date, format);
            
            return File(exportFileModel.Stream, "application/octet-stream", exportFileModel.FileName);
        }
    }
}