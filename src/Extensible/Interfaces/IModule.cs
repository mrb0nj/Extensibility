namespace Extensible.Interfaces
{
    public interface IModule<T> where T : class
    {
        void Initialize(T events);
    }
}