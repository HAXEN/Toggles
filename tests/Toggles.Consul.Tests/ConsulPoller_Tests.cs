using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Toggles.Consul.Tests
{
    public class ConsulPoller_Tests
    {
        [Fact(Skip = "Need a server and manual work.")]
        public async void Should_call_Consul()
        {
            var observerMock = new Mock<IObserver<JObject>>();
            var loggerMock = new Mock<ILogger<ConsulPoller>>();
            var source = new CancellationTokenSource();

            observerMock.Setup(x => x.OnNext(It.IsAny<JObject>())).Callback<JObject>((o) => Trace.WriteLine($"consul: {o}"));
            observerMock.Setup(x => x.OnError(It.IsAny<Exception>())).Callback(() => source.Cancel());

            var poller = new ConsulPoller(new ConsulSettings()
            {
                Host = "https://consul.service.consul",
                Port = "8501",
                FeaturesKey = "services/aw-coworker-test-hax/namespaces/default/features",
                Token = "daeabd32-bc1e-91ad-a801-b80f163d0d87",
                DisableServerCertificateValidation = true,
            }, loggerMock.Object);

            poller.Subscribe(observerMock.Object);

            poller.Start();

            await Task.Delay(60000, source.Token).ContinueWith(t=>t);

            poller.Stop();

            observerMock.Verify(x => x.OnNext(It.Is<JObject>(o => o.ToString() == JObject.Parse("{\"one\":true,\"two\":true}").ToString())));
        }
    }
}
