using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace Toggles.Sources
{
    public class JsonLiveSource : SourceBase, IObserver<JObject>
    {
        private readonly IInnerLiveSource<JObject> _innerSource;
        private IDisposable _subscription;
        private JObject _currentState;

        public JsonLiveSource(IInnerLiveSource<JObject> innerSource)
        {
            _innerSource = innerSource;
        }

        public override void Start()
        {
            _subscription = _innerSource.Subscribe(this);
            _innerSource.Start();
        }

        public override void Stop()
        {
            _innerSource.Stop();
            _subscription?.Dispose();
        }

        public void OnCompleted()
        {
            
        }

        public void OnError(Exception error)
        {
            
        }

        public void OnNext(JObject value)
        {
            _currentState?.Properties().Except(value.Properties())
                .ToList()
                .ForEach(x =>
                {
                    if(x.Value.Type != JTokenType.Boolean)
                        return;

                    NotifyObservers(x.Name, false);
                });

            if (value.ToString() != _currentState?.ToString())
            {
                value.Properties()
                    .ToList()
                    .ForEach(x =>
                    {
                        if (x.Value.Type != JTokenType.Boolean)
                            return;

                        NotifyObservers(x.Name, x.Value.Value<bool>());
                    });
            }

            _currentState = value;
        }
    }
}