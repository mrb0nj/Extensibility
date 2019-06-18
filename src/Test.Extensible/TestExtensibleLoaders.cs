using Xunit;
using Moq;
using System.Collections.Generic;
using Extensible.Interfaces;
using Test.Extensible.Shared;
using Extensible;

namespace Test.Extensible
{
    public class TestExtensibleLoaders
    {
        [Fact]
        public void ModuleHost_ShouldLoad_ASingleModuleLoader()
        {
            // Given
            var loader = new Mock<ILoader<TestAppEvents>>();
            var events = new TestAppEvents();

            // When
            var moduleHost = new ModuleHost<TestAppEvents>(loader.Object, events);

            // Then
            loader.Verify(m => m.Load(events));
        }

        [Fact]
        public void ModuleHost_ShouldLoad_MultipleModuleLoaders()
        {
            // Given
            var loader1 = new Mock<ILoader<TestAppEvents>>();
            var loader2 = new Mock<ILoader<TestAppEvents>>();
            var loaders = new List<ILoader<TestAppEvents>>()
            {
                loader1.Object,
                loader2.Object
            };

            var events = new TestAppEvents();

            // When
            var moduleHost = new ModuleHost<TestAppEvents>(loaders, events);

            // Then
            loader1.Verify(m => m.Load(events));
            loader2.Verify(m => m.Load(events));
        }
    }
}
