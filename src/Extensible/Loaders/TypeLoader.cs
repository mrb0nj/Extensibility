using Extensible.Interfaces;
using System;
using System.Collections.Generic;

namespace Extensible.Loaders
{
    public class TypeLoader<T> : ILoader<T> where T : class
    {
        private readonly string[] _types;

        public TypeLoader(string[] types)
        {
            _types = types;
        }

        public IEnumerable<IModule<T>> Load(T events)
        {
            var modules = new List<IModule<T>>();
            foreach (var type in _types)
            {
                try
                {
                    var instance = Activator.CreateInstance(Type.GetType(type));
                    if (instance is IModule<T> module)
                    {
                        module.Initialize(events);
                        modules.Add(module);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception createing instance of {type}: {ex}");
                }
            }

            return modules;
        }
    }
}
