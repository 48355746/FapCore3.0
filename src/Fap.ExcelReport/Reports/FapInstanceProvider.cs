using ExcelReportGenerator.Rendering.Providers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.ExcelReport.Reports
{
    public class FapInstanceProvider : DefaultInstanceProvider
    {
        private readonly object _defaultInstance;
        private readonly IServiceProvider _serviceProvider;

        public FapInstanceProvider(IServiceProvider serviceProvider, object defaultInstance = null) : base(defaultInstance)
        {
            _defaultInstance = defaultInstance;
            _serviceProvider = serviceProvider;
        }

        public override object GetInstance(Type type)
        {
            if (type == null || typeof(ReportBase).IsAssignableFrom(type))
            {
                return _defaultInstance;
            }
            return _serviceProvider.GetService(type);
        }

        public override T GetInstance<T>()
        {
            return (T)GetInstance(typeof(T));
        }
    }
}
