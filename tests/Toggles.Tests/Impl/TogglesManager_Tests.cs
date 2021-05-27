using System;
using System.Collections.Generic;
using System.Net.Mail;
using Toggles.Impl;
using Toggles.Sources;
using Xunit;

namespace Toggles.Tests.Impl
{
    public class TogglesManager_Tests
    {
        public class MockTwoSource : SourceBase
        {
            public override void Start()
            {
                NotifyObservers("one", false);
                NotifyObservers("two", true);
            }

            public override void Stop()
            {
            }
        }

        public class MockOneSource : SourceBase
        {
            public override void Start()
            {
                NotifyObservers("one", true);
                NotifyObservers("three", true);
            }

            public override void Stop()
            {
            }
        }

        [Fact]
        public void Should_return_the_prioritized_value()
        {
            var toggles = new ToggleManager(new (int priority, IToggleSource instance)[] {(0, new MockOneSource()), (1, new MockTwoSource())});

            Assert.False(toggles.IsOn("one"));
            Assert.True(toggles.IsOn("two"));
            Assert.True(toggles.IsOn("three"));
        }

        [Fact]
        public void Should_answer_false_when_no_toggleKey_is_found()
        {
            var manager = new ToggleManager();
            Assert.False(manager.IsOn("missing_key"));
        }
    }
}
