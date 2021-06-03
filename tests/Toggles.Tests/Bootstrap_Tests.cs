using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using Xunit;

namespace Toggles.Tests
{
    public class Bootstrap_Tests
    {
        public class MockedSource : IToggleSource, IDisposable
        {
            public void Dispose()
            {
            }

            public void Start()
            {
                
            }

            public void Stop()
            {
                
            }

            public IDisposable Subscribe(IObserver<NewToggleValue> observer)
            {
                return this;
            }
        }

        private readonly ServiceCollection _services;

        public Bootstrap_Tests()
        {
            _services = new ServiceCollection();
        }


        [Fact]
        public void Should_be_able_to_register_Type()
        {
            var services = new ServiceCollection();
            
            using var provider = services
                                .AddToggles(sources => sources
                                    .AddSource<MockedSource>(0))
                                .BuildServiceProvider();

            Assert.NotNull(provider.GetRequiredService<IToggle>().Sources);
            Assert.NotEmpty(provider.GetRequiredService<IToggle>().Sources);
            Assert.Single(provider.GetRequiredService<IToggle>().Sources,
                x => x.sourceType == typeof(MockedSource));
        }

        [Fact]
        public void Should_register_a_singleton_instance()
        {

            var provider = _services
                .AddToggles()
                .BuildServiceProvider();

            Assert.Same(provider.GetRequiredService<IToggle>(), provider.GetRequiredService<IToggle>());
        }
    }
}
