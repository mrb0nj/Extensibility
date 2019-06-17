using Extensible.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Extensible.Loaders
{
    public class ReferenceLoader<T> : ILoader<T> where T : class
    {
        public IEnumerable<IModule<T>> Load(T events)
        {
            var modules = new List<IModule<T>>();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            Console.WriteLine(assemblies.Count());
            foreach (var assembly in assemblies)
            {
                Console.WriteLine(assembly.GetName(true));
                try
                {

                    var moduleClasses = assembly.GetTypes().Where(t => t.GetInterfaces().Contains(typeof(IModule<T>)));
                    foreach (var c in moduleClasses)
                    {
                        var module = InitializeModule(assembly, c, events);
                        module.Initialize(events);
                        modules.Add(module);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return modules;
        }

        private IModule<T> InitializeModule(Assembly assembly, Type type, T events)
        {
            if (!type.GetInterfaces().Contains(typeof(IModule<T>))) return default;

            try
            {
                var instance = Activator.CreateInstance(type);
                if (instance is IModule<T> module)
                {
                    return instance as IModule<T>;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }


            return default;
        }
    }
}