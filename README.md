# Extensible

This package is heavily inpired by the extensibility patterns outlined in Miguel Castro's excellent [Pluralsight Course](https://www.pluralsight.com/courses/developing-extensible-software) AND the Extensibility work done on [csharpfritz'](https://github.com/csharpfritz) [CoreWiki](https://github.com/csharpfritz/CoreWiki) project and aims to slightly reduce the boilerplate code required project to project... yes, i'm incredibly lazy!

Extensible is compatible with .NET Framework versions 3.5, 4.0 and 4.5 while currently there is only support for .NET Standard 2.0 (Happilly accept help to get 1.0 running)

## Installation

```
PM> Install-Package Extensible
```

## Example usage

With simple Events class

```
public class ExampleEvents
{
    public Action<OnMessageReceivedEventArgs> OnMessageReceived { get; set; }
}

public class OnMessageReceivedEventArgs : CancelEventArgs
{
    public string Message { get; set; }

    public OnMessageReceivedEventArgs(string message)
    {
        Message = message;
    }
}
```

And a simple Host class where we extend the `ModuleHost<T>` base class

```
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
}
```

We can easily create an instance of this Host an Invoke events. Any modules loaded by the `DirectoryLoader` will be invoked.

```
var path = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName, "Modules");
var events = new ExampleEvents();
var host = new ExampleEventsHost(path, events);

var onReceivedEventArgs = new OnMessageReceivedEventArgs("Some text...");
host.InvokeOnMessageReceived(onReceivedEventArgs);
```

## Loaders

Loaders allow Extensible to discover 'Modules' that can hook into extensibility events.

### DirectoryLoader

The Director loader will scan a directory for Modules implementing the `IModule<T>` interface. This can be used to create a plugin style architecture where the types to be loader are not known ahead of time.

### ReferenceLoader

The Reference loader will scan the current assemly and any referenced assemblies for Modules implmenting the `IModule<T>` interface.

### TypeLoader

The Type loader will load and initialize an array of specific types that implement the `IModule<T>` interface. I use this to specify modules in config to use with DI.

## Modules

Here is a very basic example of a Module

```
public class ArchiveModule : IModule<ExampleEvents>
{
    public void Initialize(ExampleEvents events)
    {
        events.OnMessageCreated += (e) => Console.WriteLine($" => [ArchiveModule] Message created: Id={e.Message.Id}, MessageText={e.Message.MessageText}");
    }
}
```

A full working example can be seen in the examples directory.

## Contributing

Happy to accept ideas, suggestions and pull requests :)
