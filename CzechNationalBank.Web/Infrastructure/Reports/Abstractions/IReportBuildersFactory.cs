using CzechNationalBank.Web.Infrastructure.Reports.Abstractions.Builders;

namespace CzechNationalBank.Web.Infrastructure.Reports.Abstractions
{
    /// <summary>
    /// Фабрика билдеров отчетов разного формата
    /// </summary>
    public interface IReportBuildersFactory
    {
        /// <summary>
        /// Получение билдера
        /// </summary>
        /// <param name="exportFormat">Формат выгрузки</param>
        /// <typeparam name="TModel">Тип модели данных отчета</typeparam>
        ISpecificFormatReportBuilder<TModel> DefineBuilder<TModel>(ExportFormatOption exportFormat);
    }
}