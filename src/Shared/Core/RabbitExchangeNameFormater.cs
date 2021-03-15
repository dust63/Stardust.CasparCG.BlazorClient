using MassTransit.Topology;
namespace Stardust.Flux.Core
{
    public class RabbitExchangeNameFormater : IEntityNameFormatter
    {
        public string FormatEntityName<T>()
        {
            return typeof(T).Name;
        }
    }

}