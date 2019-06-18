using Extensible.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Extensible
{
    public class ModuleHost<T> : ModuleHost, IModuleHost where T : class
    {
        protected T Events { get; set; }

        public ModuleHost(ILoader<T> moduleLoader, T events)
            : this(new List<ILoader<T>> { moduleLoader }, events)
        {
        }

        public ModuleHost(IList<ILoader<T>> moduleLoaders, T events)
        {
            Events = events;
            foreach (var moduleLoader in moduleLoaders)
            {
                moduleLoader.Load(events);
            }
        }

    }

    public class ModuleHost
    {
        protected void InvokeModuleEvent<T>(Action<T> moduleEvent, T args)
        {
            if (moduleEvent == null) throw new ArgumentNullException(nameof(moduleEvent));
            if (args == null) throw new ArgumentNullException(nameof(args));

            moduleEvent.Invoke(args);
        }

        protected T InvokeCancelableModuleEvent<T>(Action<T> moduleEvent, T args)
        {
            if (moduleEvent == null) throw new ArgumentNullException(nameof(moduleEvent));
            if (args == null) throw new ArgumentNullException(nameof(args));

            var cancel = false;

            foreach (var d in moduleEvent.GetInvocationList())
            {
                var eventDelegate = d as Action<T>;

                if (eventDelegate == null) continue;
                if (cancel) break;

                eventDelegate(args);

                var eventArgs = args as CancelEventArgs;
                if (eventArgs != null)
                    cancel = eventArgs.Cancel;
            }

            return args;
        }
    }
}