using Example.Extensibility;
using Example.Extensibility.Events;
using Example.Extensibility.Models;
using System;
using System.IO;

namespace Example.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var path = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName, "Modules");
            var events = new ExampleEvents();
            var host = new ExampleEventsHost(path, events);
            string line;
            int id = 1;
            while ((line = Prompt()) != "exit")
            {
                Console.WriteLine($"Receiving new message '{line}' ... beep boop...");

                var onReceivedEventArgs = new OnMessageReceivedEventArgs(line);
                host.InvokeOnMessageReceived(onReceivedEventArgs);
                
                if(onReceivedEventArgs.Cancel)
                {
                    Console.WriteLine(" => Message Aborted!");
                    continue;
                }

                // Module could augment the message
                line = onReceivedEventArgs.Message;

                Console.WriteLine($"Message received: {line}");

                // Do something constructive
                var message = new Message { MessageText = line, Id = id++ };

                var onMessageCreatedEventArgs = new OnMessageCreatedEventArgs(message);
                host.InvokeOnMessageCreated(onMessageCreatedEventArgs);

                // All done, moving on...
            }
        }

        static string MessagePrompt = "Enter a message: ";
        static string Prompt()
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write(MessagePrompt);
            Console.ResetColor();
            return Console.ReadLine();
        }
    }
}
