using System;
using System.Collections;
using System.Collections.Generic;

namespace Toggles.Sources
{
    internal class EnvironmentSource : DictionarySource
    {
        public EnvironmentSource(EnvironmentVariableTarget target, string prefix = null) : base(CreateDictionary(Environment.GetEnvironmentVariables(target), prefix)) { }

        private static IDictionary<string, bool> CreateDictionary(IDictionary dictionary, string prefix)
        {
            var values = new Dictionary<string, bool>();
            if (dictionary == null)
                return values;

            foreach (DictionaryEntry entry in dictionary)
            {
                var key = entry.Key as string;
                var val = entry.Value as string;

                if(string.IsNullOrEmpty(key))
                    continue;

                if(string.IsNullOrEmpty(val))
                    continue;

                var trimmedKey = key;
                if (string.IsNullOrEmpty(prefix) == false)
                {
                    if(key.Contains(prefix) == false)
                        continue;

                    trimmedKey = key.Replace(prefix, string.Empty);
                }

                if(bool.TryParse(val, out var actual))
                    values.Add(trimmedKey, actual);
            }

            return values;
        }
    }
}