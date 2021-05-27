using System;
using System.Collections.Generic;
using System.Linq;

namespace Toggles
{
    public class SourcesRegister
    {
        private readonly List<(int priority, Type source, Func<IServiceProvider, IToggleSource> factory)> _sources = new();

        internal SourcesRegister() { }

        public IEnumerable<(int priority, Type source, Func<IServiceProvider, IToggleSource> factory)> Sources => _sources;

        public SourcesRegister AddSource<TSource>(int priority, Func<IServiceProvider, IToggleSource> create)
            where TSource : class, IToggleSource
        {
            if (create == null)
                throw new ArgumentNullException(nameof(create), "Factory must be supplied.");

            ValidateParams(priority, typeof(TSource));

            _sources.Add((priority, typeof(TSource), create));

            return this;
        }

        public SourcesRegister AddSource<TSource>(int priority)
            where TSource : class, IToggleSource
        {
            ValidateParams(priority, typeof(TSource));

            _sources.Add((priority, typeof(TSource), null));

            return this;
        }

        private void ValidateParams(int priority, Type sourceType)
        {
            if (priority < 0)
                throw new ArgumentOutOfRangeException(nameof(priority), "Priority can not be a negative value.");

            if (_sources.Any(x => x.priority == priority))
                throw new ArgumentOutOfRangeException(nameof(priority), $"There is already one source registered with priority({priority})");

            if (_sources.Any(x => x.source == sourceType))
                throw new ArgumentOutOfRangeException("TSource", "There is already a source of the same type registered.");
        }
    }
}