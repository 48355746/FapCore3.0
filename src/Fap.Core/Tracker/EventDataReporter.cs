using Fap.Core.DataAccess;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Core.Tracker
{
    public class EventDataReporter : IObserver<EventData>
    {
        private IDisposable unsubscriber;
        private ILogger<EventDataReporter> _logger;
        private IEventDataHandler _dataHandler;
        public EventDataReporter(ILogger<EventDataReporter> logger,IEventDataHandler dataHandler)
        {
            _logger = logger;
            _dataHandler = dataHandler;
        }
        public virtual void Subscribe(IObservable<EventData> provider)
        {
            if (provider != null)
                unsubscriber = provider.Subscribe(this);
        }

        public virtual void OnCompleted()
        {
            _logger.LogInformation("The EventData Tracker has completed.");
            this.Unsubscribe();
        }

        public virtual void OnError(Exception e)
        {
            _logger.LogError($"The EventData cannot be determined.{e.Message}");
        }

        public virtual void OnNext(EventData value)
        {
            _dataHandler.SaveEventData(value);
            _logger.LogInformation("{2}: The current EventData is {0}, {1}", value.ChangeDataType, value.ChangeData, value.EntityName);
        }

        public virtual void Unsubscribe()
        {
            unsubscriber.Dispose();
        }
    }
}
