using System.Collections.Generic;

namespace Toggles.Sources
{
    public static class DictionarySourceRegistration
    {
        public static SourcesRegister AddDictionarySource(this SourcesRegister register, int priority, IDictionary<string, bool> values)
        {
            register.AddSource<DictionarySource>(priority, provider => new DictionarySource(values));
            return register;
        }
    }
}