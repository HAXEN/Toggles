using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace Toggles.Sources
{
    public class ConfigurationSource : SourceBase
    {
        private readonly string _featuresSection;
        private readonly IDictionary<string,string> _featuresSettings = new Dictionary<string, string>();

        public ConfigurationSource(IConfiguration configuration, string featuresSection)
        {
            _featuresSection = featuresSection.Replace('_', ':');

            var section = configuration.GetSection(_featuresSection);
            if (section.Exists())
                _featuresSettings = section.AsEnumerable().ToDictionary(k => k.Key, v => v.Value);
        }

        public override void Start()
        {
            if (_featuresSettings == null)
                return;

            foreach (var item in _featuresSettings)
            {
                if(item.Key == _featuresSection)
                    continue;

                var trimmedKey = item.Key.Replace($"{_featuresSection}:", "");

                if(bool.TryParse(item.Value, out var actual))
                {
                    NotifyObservers(trimmedKey, actual);
                    continue;
                }

                NotifyObservers(trimmedKey, false);
            }

            Observers.ForEach(x => x.OnCompleted());
        }

        public override void Stop()
        {
            
        }
    }
}
