using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Core.Tracker
{
    public class EventDataTracker : IObservable<EventData>
    {
        public EventDataTracker()
        {
            observers = new List<IObserver<EventData>>();
        }

        private List<IObserver<EventData>> observers;

        public IDisposable Subscribe(IObserver<EventData> observer)
        {
            if (!observers.Contains(observer))
                observers.Add(observer);
            return new Unsubscriber(observers, observer);
        }

        private class Unsubscriber : IDisposable
        {
            private List<IObserver<EventData>> _observers;
            private IObserver<EventData> _observer;

            public Unsubscriber(List<IObserver<EventData>> observers, IObserver<EventData> observer)
            {
                this._observers = observers;
                this._observer = observer;
            }

            public void Dispose()
            {
                if (_observer != null && _observers.Contains(_observer))
                    _observers.Remove(_observer);
            }
        }

        public void TrackEventData(Nullable<EventData> eventData)
        {
            foreach (var observer in observers)
            {
                if (!eventData.HasValue)
                    observer.OnError(new EventDataUnknownException());
                else
                    observer.OnNext(eventData.Value);
            }
        }

        public void EndTransmission()
        {
            foreach (var observer in observers.ToArray())
                if (observers.Contains(observer))
                    observer.OnCompleted();

            observers.Clear();
        }
    }
    public class EventDataUnknownException : Exception
    {
        internal EventDataUnknownException()
        { }
    }
}
