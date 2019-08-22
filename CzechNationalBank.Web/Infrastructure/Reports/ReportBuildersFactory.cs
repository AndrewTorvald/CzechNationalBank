using System;
using CzechNationalBank.Web.Infrastructure.Reports.Abstractions;
using CzechNationalBank.Web.Infrastructure.Reports.Abstractions.Builders;
using Microsoft.Extensions.DependencyInjection;

namespace CzechNationalBank.Web.Infrastructure.Reports
{
    /// <inheritdoc />
    public class ReportBuildersFactory : IReportBuildersFactory
    {
        private readonly IServiceProvider _serviceProvider;

        /// <summary/>
        public ReportBuildersFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <inheritdoc />
        public ISpecificFormatReportBuilder<TModel> DefineBuilder<TModel>(ExportFormatOption exportFormat)
        {
            switch (exportFormat)
            {
                case ExportFormatOption.Txt:
                    return _serviceProvider.GetService<ITxtReportBuilder<TModel>>();
                case ExportFormatOption.Json:
                    return _serviceProvider.GetService<IJsonReportBuilder<TModel>>();
                default:
                    throw new ArgumentOutOfRangeException(nameof(exportFormat), exportFormat, null);
            }
        }
    }
}