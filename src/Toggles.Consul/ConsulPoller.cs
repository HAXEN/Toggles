using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Toggles.Sources;

namespace Toggles.Consul
{
    internal class ConsulPoller : ObservableBase<JObject>, IInnerLiveSource<JObject>
    {
        private readonly ConsulSettings _settings;
        private readonly ILogger<ConsulPoller> _logger;
        private CancellationTokenSource _cancellationSource;

        public ConsulPoller(ConsulSettings settings, ILogger<ConsulPoller> logger)
        {
            _settings = settings;
            _logger = logger;
        }

        public void Start()
        {
            _cancellationSource = new CancellationTokenSource();

            Task.Factory.StartNew(() => PollConsul(_cancellationSource.Token), TaskCreationOptions.LongRunning);
        }

        public void Stop()
        {
            _cancellationSource?.Cancel();
        }

        private HttpClient CreateClient()
        {
            var handler = new HttpClientHandler();

            if (_settings.DisableServerCertificateValidation)
                handler.ServerCertificateCustomValidationCallback = (message, certificate2, arg3, arg4) => true;
            else
                handler.ServerCertificateCustomValidationCallback = null;

            var client = new HttpClient(handler);
            client.Timeout = TimeSpan.FromMinutes(15);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("Keep-Alive", "true");
            
            return client;
        }

        private HttpRequestMessage CreateRequest(ulong lastIndex)
        {
            var url = new UriBuilder(_settings.Host);
            url.Port = int.Parse(_settings.Port);
            url.Path = $"/v1/kv/{_settings.FeaturesKey}";
            if (lastIndex > 0)
            {
                url.Query = $"index={lastIndex}&wait=10m";
            }
            var request = new HttpRequestMessage(HttpMethod.Get, url.Uri);

            if (!string.IsNullOrEmpty(_settings.Token))
            {
                _logger.LogDebug($"Setting token ('{_settings.Token}')");
                request.Headers.Add("X-Consul-Token", _settings.Token);
            }

            return request;
        }

        private async Task PollConsul(CancellationToken token)
        {
            ulong lastIndex = 0;
            var waitBetween = TimeSpan.Zero;

            using var client = CreateClient();
            while (token.IsCancellationRequested == false)
            {
                await Task.Delay(waitBetween, _cancellationSource.Token).ContinueWith(t => t);
                try
                {
                    var request = CreateRequest(lastIndex);
                    var response = await client.SendAsync(request, _cancellationSource.Token).ConfigureAwait(false);

                    if (response.IsSuccessStatusCode)
                    {
                        ulong responseIndex = 0;
                        if (response.Headers.Contains("X-Consul-Index"))
                            responseIndex = ulong.Parse(response.Headers.GetValues("X-Consul-Index").First());

                        if(responseIndex == lastIndex)
                            continue;

                        lastIndex = responseIndex;

                        var responseContent = await response.Content.ReadAsStringAsync(_cancellationSource.Token);
                        var kvResponse = JsonConvert.DeserializeObject<KVResponse[]>(responseContent);
                        
                        var jsonString = Encoding.UTF8.GetString(Convert.FromBase64String(kvResponse.First().Value));
                        var jObj = JObject.Parse(jsonString);
                        Observers.ForEach(x => x.OnNext(jObj));
                        waitBetween = TimeSpan.FromMilliseconds(50);
                    }
                    else
                    {
                        throw new ApplicationException($"Consul returned faulty response. '{response.StatusCode}'");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in ConsulPoller.");
                    Observers.ForEach(x => x.OnError(ex));
                    if(waitBetween == TimeSpan.Zero)
                        waitBetween = TimeSpan.FromMilliseconds(50);

                    waitBetween = waitBetween * 10;
                    if (waitBetween > TimeSpan.FromMinutes(5))
                        waitBetween = TimeSpan.FromMinutes(5);
                }

            }
        }
    }

    public class KVResponse
    {
        public int LockIndex { get; set; }
        public string Key { get; set; }
        public int Flags { get; set; }
        public ulong CreateIndex { get; set; }
        public ulong ModifyIndex { get; set; }
        public string Value { get; set; }   
    }
}
