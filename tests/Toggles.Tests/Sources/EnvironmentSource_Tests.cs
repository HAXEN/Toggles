using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Toggles.Sources;
using Xunit;

namespace Toggles.Tests.Sources
{
    public class EnvironmentSource_Tests
    {
        [Fact]
        public async void Should_notify_based_on_environments()
        {
            Environment.SetEnvironmentVariable("application_features_one", "true");
            Environment.SetEnvironmentVariable("application_features_two", "false");

            var observerMock = new Mock<IObserver<NewToggleValue>>();
            var tokenSource = new CancellationTokenSource();

            observerMock.Setup(x => x.OnCompleted()).Callback(() => tokenSource.Cancel());

            var source = new EnvironmentSource(EnvironmentVariableTarget.Process, "application_features_");

            source.Subscribe(observerMock.Object);
            source.Start();

            await Task.Delay(200, tokenSource.Token).ContinueWith(t => t);

            observerMock.Verify(x => x.OnNext(new NewToggleValue{SourceType = typeof(EnvironmentSource), Key = "one", Active = true}));
            observerMock.Verify(x => x.OnNext(new NewToggleValue { SourceType = typeof(EnvironmentSource), Key = "two", Active = false }));
        }
    }
}
