using Extensible.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Extensible.Loaders
{
    public class DirectoryLoader<T> : ILoader<T> where T : class
    {
        private readonly string[] _paths;
        private readonly bool _recursive;
        private readonly string _moduleFilter;
        private const string ModuleFilter = "*.dll";

        public DirectoryLoader(string path, bool recursive = false, string moduleFilter = ModuleFilter) :
            this(new string[] { path }, recursive, moduleFilter)
        {
        }

        public DirectoryLoader(string[] paths, bool recursive = false, string moduleFilter = ModuleFilter)
        {
            _paths = paths;
            _recursive = recursive;
            _moduleFilter = moduleFilter;
        }

        public IEnumerable<IModule<T>> Load(T events)
        {
            var modules = new List<IModule<T>>();
            foreach (var path in _paths)
            {
                if (!Directory.Exists(path)) throw new ArgumentException($"Directory {path} does not exist");
                modules.AddRange(LoadDirectory(path, events));
            }
            return modules;
        }

        private IEnumerable<IModule<T>> LoadDirectory(string path, T events)
        {
            var modules = new List<IModule<T>>();

            var assemblies = Directory.GetFiles(path, _moduleFilter);
            foreach (var assembly in assemblies)
            {
                try
                {
                    var m = Assembly.LoadFile(assembly);
                    var moduleClasses = m.GetTypes().Where(t => t.GetInterfaces().Contains(typeof(IModule<T>)));
                    foreach (var c in moduleClasses)
                    {
                        var module = InitializeModule(m, c);
                        module.Initialize(events);
                        modules.Add(module);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            if (_recursive)
            {
                var directories = Directory.GetDirectories(path);
                foreach (var directory in directories)
                {
                    modules.AddRange(LoadDirectory(directory, events));
                }
            }

            return modules;
        }


        private IModule<T> InitializeModule(Assembly assembly, Type type)
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