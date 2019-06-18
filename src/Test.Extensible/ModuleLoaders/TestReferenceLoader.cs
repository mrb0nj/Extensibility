using Extensible.Interfaces;
using Extensible.Loaders;
using System;
using Xunit;

namespace Test.Extensible.ModuleLoaders
{
    public class TestReferenceModuleLoader
    {
        [Fact]
        public void ReferenceLoader_ShouldNotLoadModules_IfNoneInCurrentAssembly()
        {
            // Given
            var loader = new ReferenceLoader<TestReferenceEventsInvalid>();

            // When
            var modules = loader.Load(new TestReferenceEventsInvalid());

            // Then
            Assert.Empty(modules);
        }

        [Fact]
        public void ReferenceLoader_ShouldLoadModules_IfDefinedInCurrentAssembly()
        {
            // Given
            var loader = new ReferenceLoader<TestReferenceEvents>();

            // When
            var modules = loader.Load(new TestReferenceEvents());

            // Then
            Assert.Single(modules);
        }

        [Fact]
        public void ReferenceLoader_ShouldWillLoadModules_IfReferencedAssembliesAreNotLoaded()
        {
            // Given
            var loader = new ReferenceLoader<TestReferenceEvents>();

            // When
            var modules = loader.Load(new TestReferenceEvents());

            // Then
            Assert.Single(modules);
        }

        [Fact]
        public void ReferenceLoader_ShouldBindEvents_IfModulesAvailable()
        {
            // Given
            var loader = new ReferenceLoader<TestReferenceEvents>();
            var events = new TestReferenceEvents();

            // When
            var modules = loader.Load(events);

            //Then
            Assert.NotNull(events.MyEvent);
        }

        [Fact]
        public void ReferenceLoader_ShouldNotBindEvents_IfNoModulesAvailable()
        {
            // Given
            var loader = new ReferenceLoader<TestReferenceEventsInvalid>();
            var events = new TestReferenceEventsInvalid();

            // When
            var modules = loader.Load(events);

            //Then
            Assert.Null(events.MyEvent);
        }
    }

    public class TestReferenceEvents
    {
        public Action<string> MyEvent { get; set; }
    }

    public class TestReferenceEventsInvalid
    {
        public Action<string> MyEvent { get; set; }
    }

    public class TestRefereceModule : IModule<TestReferenceEvents>
    {
        public void Initialize(TestReferenceEvents events)
        {
            events.MyEvent += (e) => Console.WriteLine("Hello");
        }
    }
}