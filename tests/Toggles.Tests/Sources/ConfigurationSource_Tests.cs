using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Toggles.Sources;
using Xunit;

namespace Toggles.Tests.Sources
{
    public class ConfigurationSource_Tests
    {

        [Fact]
        public async void Should_publish_values_from_config()
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes("{\"config\":{\"namespace\":{\"features\":{\"one\":true, \"two\":false}}}}"));
            stream.Position = 0;
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonStream(stream)
                .Build();

            var source = new ConfigurationSource(configuration, "config_namespace_features");
            var observerMock = new Mock<IObserver<NewToggleValue>> ();

            source.Subscribe(observerMock.Object);

            source.Start();

            await Task.Delay(100);

            observerMock.Verify(x => x.OnNext(new NewToggleValue { SourceType = typeof(ConfigurationSource), Key = "one", Active = true}));
            observerMock.Verify(x => x.OnNext(new NewToggleValue { SourceType = typeof(ConfigurationSource), Key = "two", Active = false }));
        }

        [Fact(Skip = "Fix so it does not fail other tests")]
        public async void Should_be_able_to_register()
        {
            var services = new ServiceCollection();

            IConfiguration configuration = new ConfigurationBuilder().Build();

            await using (var provider = services
                .AddSingleton(configuration)
                .AddToggles(sources => sources.AddConfigurationSource(3, "features_section"))
                .BuildServiceProvider())
            {
                var actual = Assert.Single(provider.GetRequiredService<IToggle>().Sources,
                    x => x.sourceType == typeof(ConfigurationSource));

                Assert.Equal(3, actual.priority);
            }
        }

        [Fact]
        public void Should_be_able_to_create_instance()
        {
            IConfiguration configuration = new ConfigurationBuilder().Build();

            var source = new ConfigurationSource(configuration, "features_section");

            Assert.NotNull(source);
        }
    }
}
