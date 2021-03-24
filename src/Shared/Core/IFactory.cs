namespace Stardust.Flux.Core
{
    public interface IFactory<T>
    {
        T Create();
    }
}