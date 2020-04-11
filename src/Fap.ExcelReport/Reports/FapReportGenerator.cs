using ExcelReportGenerator.Rendering;
using ExcelReportGenerator.Rendering.Providers;
using ExcelReportGenerator.Rendering.Providers.VariableProviders;
using Fap.Core.Infrastructure.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.ExcelReport.Reports
{
    public class FapReportGenerator : DefaultReportGenerator
    {
        private IInstanceProvider _instanceProvider;
        private readonly IServiceProvider _serviceProvider;
        public FapReportGenerator(IServiceProvider serviceProvider, object report) : base(report)
        {
            _serviceProvider = serviceProvider;
        }
        public override Type SystemFunctionsType => typeof(FapSystemFunctions);
        public override SystemVariableProvider SystemVariableProvider => new FapSystemVariableProvider();
        public override IInstanceProvider InstanceProvider
        {
            get { return _instanceProvider ?? (_instanceProvider = new FapInstanceProvider(_serviceProvider, Report)); }
        }
        public override ITypeProvider TypeProvider
        {
            get
            {
                return new DefaultTypeProvider(new[] { Report.GetType().Assembly }, defaultType: Report.GetType());
            }
        }
    }
}
