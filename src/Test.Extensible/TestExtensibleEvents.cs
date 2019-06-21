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

        [Fact]
        public void ModuleHost_DefaultSortPriority()
        {
            // Given
            var events = new DummyEvents();
            events.OneWay += DummyHandler;

            var m1 = new DummyModBase(1);
            m1.Initialize(events);

            var m2 = new DummyModBase(2);
            m2.Initialize(events);

            var loader = new Mock<ILoader<IDummyEvents>>();
            var host = new TestModuleHost(loader.Object, events);

            // When/Then - Exception thrown if execution is out of sequence
            var args = new DummyEventArgs { ExecutionCount = 0 };
            host.InvokeOneWayEvent(args);
        }

        [Fact]
        public void ModuleHost_ShouldSortDelegatesByPriority()
        {
            // Given
            var events = new DummyEvents();
            var m0 = new DummyMod1();
            m0.Initialize(events);

            var m1 = new DummyMod1();
            m1.Initialize(events);

            var m2 = new DummyMod2(0);
            m2.Initialize(events);

            var loader = new Mock<ILoader<IDummyEvents>>();
            var host = new TestModuleHost(loader.Object, events);

            // When/Then - Exception thrown if execution is out of sequence
            var args = new DummyEventArgs { ExecutionCount = 0 };
            host.InvokeOneWayEvent(args);
        }

        [Fact]
        public void ModuleHost_ShouldSortCancelableDelegatesByPriority()
        {
            // Given
            var events = new DummyEvents();
            var m0 = new DummyMod1();
            m0.Initialize(events);

            var m1 = new DummyMod1();
            m1.Initialize(events);

            var m2 = new DummyMod2(0);
            m2.Initialize(events);

            var loader = new Mock<ILoader<IDummyEvents>>();
            var host = new TestModuleHost(loader.Object, events);

            // When/Then - Exception thrown if execution is out of sequence
            var args = new DummyCancelEventArgs { ExecutionCount = 0 };
            host.InvokeCanCancelEvent(args);
        }

        private void DummyHandler(DummyEventArgs e)
        {
            if (e.ExecutionCount > 0) throw new Exception();
            e.ExecutionCount++;
        }
    }

    public class DummyModBase : IModule<IDummyEvents>
    {
        public int ExpectedExecutionCount { get; set; }
        public DummyModBase(int expected)
        {
            ExpectedExecutionCount = expected;
        }

        public void Initialize(IDummyEvents events)
        {
            events.OneWay += DummyHandler;
            events.CanCancel += DummyCancelHandler;
        }

        private void DummyHandler(DummyEventArgs e)
        {
            if (ExpectedExecutionCount >= 0 && e.ExecutionCount != ExpectedExecutionCount) throw new Exception($"Expecting execution order of {ExpectedExecutionCount} nut was {e.ExecutionCount}");
            e.ExecutionCount++;
        }

        private void DummyCancelHandler(DummyCancelEventArgs e)
        {
            if (ExpectedExecutionCount >= 0 && e.ExecutionCount != ExpectedExecutionCount) throw new Exception($"Expecting execution order of {ExpectedExecutionCount} nut was {e.ExecutionCount}");
            e.ExecutionCount++;
        }
    }

    [ModulePriority(100)]
    public class DummyMod1 : DummyModBase
    {
        public DummyMod1(int expected = -1) : base(expected)
        {
        }
    }

    [ModulePriority(200)]
    public class DummyMod2 : DummyModBase
    {
        public DummyMod2(int expected = -1) : base(expected)
        {
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

        public void InvokeOneWayEvent(DummyEventArgs args = null)
        {
            try
            {
                InvokeModuleEvent(Events.OneWay, args ?? new DummyEventArgs());
            }
            catch (Exception)
            {
                throw;
            }
        }

        public DummyCancelEventArgs InvokeCanCancelEvent(DummyCancelEventArgs args = null)
        {
            try
            {
                return InvokeCancelableModuleEvent(Events.CanCancel, args ?? new DummyCancelEventArgs());
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
        public int ExecutionCount { get; set; }
    }

    public class DummyCancelEventArgs : CancelEventArgs
    {
        public int ExecutionCount { get; set; }
    }
}
