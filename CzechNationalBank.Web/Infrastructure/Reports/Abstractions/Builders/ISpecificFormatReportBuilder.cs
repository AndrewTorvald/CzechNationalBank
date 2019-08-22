using System.Threading.Tasks;
using CzechNationalBank.Web.Services.Models;

namespace CzechNationalBank.Web.Infrastructure.Reports.Abstractions.Builders
{
    /// <summary>
    /// Билдер отчета
    /// </summary>
    /// <typeparam name="TModel">Тип модели данных отчета</typeparam>
    public interface ISpecificFormatReportBuilder<in TModel> 
    {
        /// <summary>
        /// Построение модели отчета
        /// </summary>
        /// <param name="model">Модель данных отчета</param>
        Task<ExportFileModel> BuildReport(TModel model);
    }
}