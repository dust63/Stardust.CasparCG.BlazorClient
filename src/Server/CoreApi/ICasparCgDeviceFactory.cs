using StarDust.CasparCG.net.Device;

namespace Stardust.Flux.CoreApi
{
    public interface ICasparCgDeviceFactory
    {
        ICasparDevice CreateDevice(string host, int port);

        void RemoveDevice(string host, int port);
    }
}