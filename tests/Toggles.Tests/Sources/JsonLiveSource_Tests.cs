using Moq;
using Newtonsoft.Json.Linq;
using System;
using Toggles.Sources;
using Xunit;

namespace Toggles.Tests.Sources
{
    public class JsonLiveSource_Tests
    {
        [Fact]
        public void Should_not_notify_when_json_is_not_changed()
        {
            var innerSource = new InnerSourceMock();
            var source = new JsonLiveSource(innerSource);

            var observerMock = new Mock<IObserver<NewToggleValue>>();

            source.Subscribe(observerMock.Object);

            source.Start();

            innerSource.Send(new { one = true, two = false });
            innerSource.Send(new { one = true, two = false });

            observerMock.Verify(x => x.OnNext(new NewToggleValue { SourceType = typeof(JsonLiveSource), Key = "one", Active = true }));
            observerMock.Verify(x => x.OnNext(new NewToggleValue { SourceType = typeof(JsonLiveSource), Key = "two", Active = false }));
        }

        [Fact]
        public void Should_notify_false_when_property_is_not_longer_present()
        {
            var innerSource = new InnerSourceMock();
            var source = new JsonLiveSource(innerSource);

            var observerMock = new Mock<IObserver<NewToggleValue>>();

            source.Subscribe(observerMock.Object);

            source.Start();

            innerSource.Send(new { one = true, two = false });
            innerSource.Send(new { two = false });

            observerMock.Verify(x => x.OnNext(new NewToggleValue { SourceType = typeof(JsonLiveSource), Key = "one", Active = true }));
            observerMock.Verify(x => x.OnNext(new NewToggleValue { SourceType = typeof(JsonLiveSource), Key = "two", Active = false }));
            observerMock.Verify(x => x.OnNext(new NewToggleValue { SourceType = typeof(JsonLiveSource), Key = "one", Active = false }));
        }

        [Fact]
        public void Should_notify_bool_values_from_json()
        {
            var innerSource = new InnerSourceMock();
            var source = new JsonLiveSource(innerSource);

            var observerMock = new Mock<IObserver<NewToggleValue>>();

            source.Subscribe(observerMock.Object);

            source.Start();

            innerSource.Send(new {one=true, two=false});

            observerMock.Verify(x => x.OnNext(new NewToggleValue{SourceType = typeof(JsonLiveSource), Key = "one", Active = true}));
            observerMock.Verify(x => x.OnNext(new NewToggleValue { SourceType = typeof(JsonLiveSource), Key = "two", Active = false }));
        }

        public class InnerSourceMock : IInnerLiveSource<JObject>, IDisposable
        {
            private IObserver<JObject> _observer;

            public IDisposable Subscribe(IObserver<JObject> observer)
            {
                _observer = observer;
                return this;
            }

            public void Dispose()
            {
            }

            public void Send(object obj)
            {
                _observer.OnNext(JObject.FromObject(obj));
            }

            public void Start()
            {
                
            }

            public void Stop()
            {
                
            }
        }
    }
}
