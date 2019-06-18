using Extensible;
using Extensible.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xunit;

namespace Test.Extensible
{
    public class TestExtensibleEvents
    {
        [Fact]
        public void ModuleHost_ShouldInvokeOneWayEvent()
        {
            // Given
            var triggered = false;
            var events = new DummyEvents();
            events.OneWay += (e) => triggered = true;

            var loader = new Mock<ILoader<IDummyEvents>>();
            var host = new TestModuleHost(loader.Object, events);

            // When
            host.InvokeOneWayEvent();

            // Then
            Assert.True(triggered);
        }

        [Fact]
        public void ModuleHost_ShouldNotInvokeOneWayEvent()
        {
            // Given
            var triggered = false;
            var events = new DummyEvents();
            events.OneWay += (e) => triggered = true;

            var loader = new Mock<ILoader<IDummyEvents>>();
            var host = new TestModuleHost(loader.Object, events);

            // When
            //host.InvokeOneWayEvent();

            // Then
            Assert.False(triggered);
        }

        [Fact]
        public void ModuleHost_ShouldInvokeCanCancelEvent()
        {
            // Given
            var triggered = false;
            var events = new DummyEvents();
            events.CanCancel += (e) => triggered = true;

            var loader = new Mock<ILoader<IDummyEvents>>();
            var host = new TestModuleHost(loader.Object, events);

            // When
            var result = host.InvokeCanCancelEvent();

            // Then
            Assert.True(triggered);
            Assert.False(result.Cancel);
        }

        [Fact]
        public void ModuleHost_ShouldInvokeCanCancelEventAndSetCancel()
        {
            // Given
            var triggered = false;
            var events = new DummyEvents();
            events.CanCancel += (DummyCancelEventArgs e) =>
            {
                triggered = true;
                e.Cancel = true;
            };

            var loader = new Mock<ILoader<IDummyEvents>>();
            var host = new TestModuleHost(loader.Object, events);

            // When
            var result = host.InvokeCanCancelEvent();

            // Then
            Assert.True(triggered);
            Assert.True(result.Cancel);
        }
    }

    public class TestModuleHost : ModuleHost<IDummyEvents>
    {
        public TestModuleHost(ILoader<IDummyEvents> moduleLoader, IDummyEvents events) : base(moduleLoader, events)
        {
        }

        public TestModuleHost(IList<ILoader<IDummyEvents>> moduleLoaders, IDummyEvents events) : base(moduleLoaders, events)
        {
        }

        public void InvokeOneWayEvent()
        {
            try
            {
                var args = new DummyEventArgs();
                InvokeModuleEvent(Events.OneWay, args);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public DummyCancelEventArgs InvokeCanCancelEvent()
        {
            try
            {
                var args = new DummyCancelEventArgs();
                return InvokeCancelableModuleEvent(Events.CanCancel, args);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }

    public interface IDummyEvents
    {
        Action<DummyCancelEventArgs> CanCancel { get; set; }
        Action<DummyEventArgs> OneWay { get; set; }
    }

    public class DummyEvents : IDummyEvents
    {
        public Action<DummyEventArgs> OneWay { get; set; }
        public Action<DummyCancelEventArgs> CanCancel { get; set; }
    }

    public class DummyEventArgs
    {

    }

    public class DummyCancelEventArgs : CancelEventArgs
    {

    }
}
