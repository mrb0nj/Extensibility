using Example.Extensibility.Events;
using Extensible.Interfaces;
using System;

namespace Example.Module2
{
    public class ArchiveModule : IModule<ExampleEvents>
    {
        public void Initialize(ExampleEvents events)
        {
            events.OnMessageCreated += (e) => Console.WriteLine($" => [ArchiveModule] Message created: Id={e.Message.Id}, MessageText={e.Message.MessageText}");
        }
    }
}
