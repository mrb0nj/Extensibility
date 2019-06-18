using Extensible.Interfaces;
using System;
using Test.Extensible.Shared;

namespace Test.Extensible.Module
{
    public class TestModule : IModule<TestAppEvents>
    {
        public void Initialize(TestAppEvents events)
        {
            events.MyEvent += (e) => Console.WriteLine("Hello");
        }
    }
}