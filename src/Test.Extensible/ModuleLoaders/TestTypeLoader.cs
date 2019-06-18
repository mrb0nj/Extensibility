using Extensible.Interfaces;
using Extensible.Loaders;
using System;
using System.Linq;
using Test.Extensible.Shared;
using Xunit;

namespace Test.Extensible.ModuleLoaders
{
    public class TestTypeNameModuleLoader
    {
        [Fact]
        public void TypeLoader_ShouldLoadModule_WhenValidTypeNameIsProvided()
        {
            // Given
            var types = new string[]
            {
                "Test.Extensible.Module.TestModule, Test.Extensible.Module"
            };
            var loader = new TypeLoader<TestAppEvents>(types);

            // When
            var modules = loader.Load(new TestAppEvents());

            // Then
            Assert.Single(modules);
        }

        [Fact]
        public void TypeLoader_ShouldLoadModules_WhenMultipleTypeNamesProvided()
        {
            // Given
            var types = new string[]
            {
                "Test.Extensible.Module.TestModule, Test.Extensible.Module",
                "Test.Extensible.ModuleLoaders.TestTypeModule, Test.Extensible"
            };
            var loader = new TypeLoader<TestAppEvents>(types);

            // When
            var modules = loader.Load(new TestAppEvents());

            // Then
            Assert.Equal(2, modules.Count());
        }

        [Fact]
        public void TypeLoader_ShouldBindEvents_WhenValidTypeNameIsProvided()
        {
            // Given
            var types = new string[]
            {
                "Test.Extensible.Module.TestModule, Test.Extensible.Module"
            };
            var loader = new TypeLoader<TestAppEvents>(types);
            var events = new TestAppEvents();

            // When
            var modules = loader.Load(events);

            //Then
            Assert.NotNull(events.MyEvent);
        }

        [Fact]
        public void TypeLoader_ShouldNotBindEvents_WhenNoValidTypeNamesAreProvided()
        {
            // Given
            var types = new string[]
            {
            };
            var loader = new TypeLoader<TestAppEvents>(types);
            var events = new TestAppEvents();

            // When
            var modules = loader.Load(events);

            //Then
            Assert.Null(events.MyEvent);
        }
    }


    public class TestTypeModule : IModule<TestAppEvents>
    {
        public void Initialize(TestAppEvents events)
        {
            events.MyEvent += (e) => Console.WriteLine("Hello");
        }
    }
}