using Example.Extensibility.Events;
using Extensible.Interfaces;

namespace Example.Module2
{
    public class BlacklistModule : IModule<ExampleEvents>
    {
        public void Initialize(ExampleEvents events)
        {
            events.OnMessageReceived += (e) => e.Cancel = CancelIfMessageMentionsMe(e.Message);
        }

        private bool CancelIfMessageMentionsMe(string message)
        {
            return message.Contains("@mrb0nj");
        }
    }
}
