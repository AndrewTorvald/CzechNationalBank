using System;
using System.Threading.Tasks;
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
        private readonly ReportService _service;

        /// <inheritdoc />
        public ExchangeRatesController(ReportService service)
        {
            _service = service;
        }

        /// <summary>
        /// Получение отчета по курсу кроны
        /// </summary>
        [HttpGet("/Report")]
        public async Task<object> GetReport([FromQuery] DateTimeOffset date, [FromQuery] string format = "txt")
        {
            var exportFileModel = await _service.BuildExchangeRatesReport(date, format);
            
            return File(exportFileModel.Stream, "application/octet-stream", exportFileModel.FileName);
        }
    }
}