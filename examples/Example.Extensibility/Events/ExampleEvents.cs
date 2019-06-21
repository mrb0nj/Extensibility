using Example.Extensibility.Models;
using System;
using System.ComponentModel;

namespace Example.Extensibility.Events
{
    public class ExampleEvents
    {
        public Action<OnMessageReceivedEventArgs> OnMessageReceived { get; set; }
        public Action<OnMessageCreatedEventArgs> OnMessageCreated { get; set; }
    }

    public class OnMessageReceivedEventArgs : CancelEventArgs
    {
        public string Message { get; set; }

        public OnMessageReceivedEventArgs(string message)
        {
            Message = message;
        }
    }

    public class OnMessageCreatedEventArgs
    {
        public Message Message { get; set; }

        public OnMessageCreatedEventArgs(Message message)
        {
            Message = message;
        }
    }
}
