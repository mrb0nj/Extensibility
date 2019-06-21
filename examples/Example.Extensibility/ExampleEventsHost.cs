using Example.Extensibility.Events;
using Example.Extensibility.Models;
using Extensible;
using Extensible.Loaders;
using System;

namespace Example.Extensibility
{
    public class ExampleEventsHost : ModuleHost<ExampleEvents>
    {
        public ExampleEventsHost(string path, ExampleEvents events) : base(new DirectoryLoader<ExampleEvents>(path), events)
        {
        }

        public void InvokeOnMessageReceived(OnMessageReceivedEventArgs args)
        {
            try
            {
                if(Events.OnMessageReceived != null)
                {
                    InvokeCancelableModuleEvent(Events.OnMessageReceived, args);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"InvokeOnMessageReceived({args.Message}) threw an exception with message: {ex.Message}");
            }
        }

        public void InvokeOnMessageCreated(OnMessageCreatedEventArgs args)
        {
            try
            {
                if (Events.OnMessageCreated != null)
                {
                    InvokeModuleEvent(Events.OnMessageCreated, args);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"InvokeOnMessageReceived() threw an exception with message: {ex.Message}");
            }
        }
    }
}
