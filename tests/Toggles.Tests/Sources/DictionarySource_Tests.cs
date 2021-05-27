using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Toggles.Sources;
using Xunit;

namespace Toggles.Tests.Sources
{
    public class DictionarySource_Tests
    {
        [Fact]
        public async void Should_notify_about_values()
        {
            var dictionary = new Dictionary<string, bool>()
            {
                {"one", true},
                {"two", false}
            };

            var source = new DictionarySource(dictionary);
            var tokenSource = new CancellationTokenSource();

            var observerMock = new Mock<IObserver<NewToggleValue>>();
            observerMock.Setup(x => x.OnCompleted()).Callback(() => tokenSource.Cancel());

            source.Subscribe(observerMock.Object);
            source.Start();

            await Task.Delay(100, tokenSource.Token).ContinueWith(t => t);

            observerMock.Verify(x => x.OnNext(new NewToggleValue{SourceType = typeof(DictionarySource), Key = "one", Active = true}));
            observerMock.Verify(x => x.OnNext(new NewToggleValue { SourceType = typeof(DictionarySource), Key = "two", Active = false }));
        }
    }
}
