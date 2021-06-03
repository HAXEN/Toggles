using Newtonsoft.Json.Linq;
using Toggles.Sources;

namespace Toggles.Consul
{
    internal class ConsulKvSource : JsonLiveSource
    {
        public ConsulKvSource(IInnerLiveSource<JObject> innerSource) : base(innerSource) { }
    }
}
