namespace CzechNationalBank.Web.Infrastructure.Reports.Abstractions.Builders
{
    /// <summary>
    /// Билдер отчета в .txt
    /// </summary>
    /// <typeparam name="TModel">Тип модели данных отчета</typeparam>
    public interface ITxtReportBuilder<in TModel> : ISpecificFormatReportBuilder<TModel> 
    {
    }
}