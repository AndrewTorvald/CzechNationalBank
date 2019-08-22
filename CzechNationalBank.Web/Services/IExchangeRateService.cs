using System;
using System.Threading.Tasks;
using CzechNationalBank.Web.Infrastructure.Reports.Abstractions;
using CzechNationalBank.Web.Services.Models;

namespace CzechNationalBank.Web.Services
{
    /// <summary>
    /// Сервис валютных курсов
    /// </summary>
    public interface IExchangeRateService
    {
        /// <summary>
        /// Создание отчета по курсу кроны в заданный месяц/год
        /// </summary>
        /// <param name="date">Дата по которой производится выборка данных</param>
        /// <param name="format">Формат отчета</param>
        Task<ExportFileModel> BuildReport(DateTimeOffset date, ExportFormatOption format);
    }
}