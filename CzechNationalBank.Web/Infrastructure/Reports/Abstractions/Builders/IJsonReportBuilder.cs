namespace CzechNationalBank.Web.Infrastructure.Reports.Abstractions.Builders
{
    /// <summary>
    /// Билдер отчета в .json
    /// </summary>
    /// <typeparam name="TModel">Тип модели данных отчёта</typeparam>
    public interface IJsonReportBuilder<in TModel> : ISpecificFormatReportBuilder<TModel> 
    {
    }
}