using System.Collections.Generic;

namespace Toggles.Sources
{
    internal class DictionarySource : SourceBase
    {
        private readonly IDictionary<string, bool> _dictionary;

        public DictionarySource(IDictionary<string, bool> dictionary)
        {
            _dictionary = dictionary;
        }

        public override void Start()
        {
            foreach (var item in _dictionary)
            {
                NotifyObservers(item.Key, item.Value);
            }
            NotifyCompleted();
        }

        public override void Stop()
        {
            
        }
    }
}