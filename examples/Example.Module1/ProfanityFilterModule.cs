using Example.Extensibility.Events;
using Extensible.Interfaces;
using System.Linq;

namespace Example.Module1
{
    public class ProfanityFilterModule : IModule<ExampleEvents>
    {
        public void Initialize(ExampleEvents events)
        {
            events.OnMessageReceived += (e) => e.Message = FilterFilth(e.Message);
        }

        private string FilterFilth(string message)
        {
            var filth = new string[] { "fork" };
            foreach (var f in filth)
            {
                if(message.Contains(f))
                {
                    var replacement = $"{f.First()}{string.Empty.PadLeft(f.Length-2, '*')}{f.Last()}";
                    message = message.Replace(f, replacement);
                }
                
            }

            return message;
        }
    }
}
