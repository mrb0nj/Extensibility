using System.Collections.Generic;

namespace Extensible.Interfaces
{
    public interface ILoader<T> where T : class
    {
        IEnumerable<IModule<T>> Load(T events);
    }
}