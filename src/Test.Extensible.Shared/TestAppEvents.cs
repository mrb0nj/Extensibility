using System;

namespace Test.Extensible.Shared
{
    public class TestAppEvents
    {
        public Action<string> MyEvent { get; set; }
    }
}