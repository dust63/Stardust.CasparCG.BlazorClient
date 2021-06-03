namespace Stardust.Flux.Server.Services
{
    public interface ITempStorage
    {


        void Set<T>(string key, T value);

        T Get<T>(string key);
    }
}
