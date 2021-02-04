using Stardust.Flux.Contract;
using Stardust.Flux.CoreApi.Models.Entity;

namespace Stardust.Flux.CoreApi.Factory
{
    public static class ServerFactory
    {

        public static void UpdateValueFrom(this Server server, ServerDto serverDto)
        {
            server.Hostname = serverDto.Hostname;
            server.Port = serverDto.Port;
            server.Name = serverDto.Name;
        }

    }
}