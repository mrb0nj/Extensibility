using Extensible.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

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
    [AttributeUsage(AttributeTargets.All)]
    public class ModulePriorityAttribute : Attribute
    {
        public int Priority { get; set; }
        public ModulePriorityAttribute(int priority)
        {
            Priority = priority;
        }
    }

    

    public class ModuleHost
    {
        protected void InvokeModuleEvent<T>(Action<T> moduleEvent, T args)
        {
            if (moduleEvent == null) throw new ArgumentNullException(nameof(moduleEvent));
            if (args == null) throw new ArgumentNullException(nameof(args));

            Invoke(moduleEvent, args);
        }

        protected T InvokeCancelableModuleEvent<T>(Action<T> moduleEvent, T args)
        {
            if (moduleEvent == null) throw new ArgumentNullException(nameof(moduleEvent));
            if (args == null) throw new ArgumentNullException(nameof(args));

            return Invoke(moduleEvent, args);
        }

        private static T Invoke<T>(Action<T> moduleEvent, T args)
        {
            var cancel = false;
            foreach (var d in moduleEvent.GetInvocationListByPriority())
            {
                if (cancel) break;

                var eventDelegate = d as Action<T>;
                if (eventDelegate == null) continue;

                eventDelegate(args);

                var eventArgs = args as CancelEventArgs;
                if (eventArgs != null)
                    cancel = eventArgs.Cancel;
            }

            return args;
        }
    }
}