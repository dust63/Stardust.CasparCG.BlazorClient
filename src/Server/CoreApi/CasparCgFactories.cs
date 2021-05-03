using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using StarDust.CasparCG.net.Connection;
using StarDust.CasparCG.net.Device;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace Stardust.Flux.CoreApi
{
    public class CasparCgDeviceFactory : ICasparCgDeviceFactory, IDisposable
    {

        public Dictionary<CasparCGConnectionSettings, ICasparDevice> casparInstances;
        private bool isDisposed;
        private readonly IHttpContextAccessor httpContextAccessor;

        public CasparCgDeviceFactory(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
            casparInstances = new Dictionary<CasparCGConnectionSettings, ICasparDevice>(new ConnectionComparer());
        }

        public ICasparDevice CreateDevice(string host, int port)
        {
            var connectionSettings = new CasparCGConnectionSettings(host, port);

            if (casparInstances.ContainsKey(connectionSettings))
                return casparInstances[connectionSettings];

            var device = httpContextAccessor.HttpContext.RequestServices.GetRequiredService<ICasparDevice>();
            device.ConnectionSettings = connectionSettings;
            casparInstances.Add(connectionSettings, device);
            return device;
        }

        public void RemoveDevice(string host, int port)
        {
            var connectionSettings = new CasparCGConnectionSettings(host, port);

            if (!casparInstances.ContainsKey(connectionSettings))
                return;

            casparInstances[connectionSettings].Disconnect();
            casparInstances.Remove(connectionSettings);
        }



        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (isDisposed) return;

            if (disposing)
            {
                // free managed resources
                var connectedInstances = casparInstances.Where(x => x.Value.IsConnected);
                foreach (var instance in connectedInstances)
                {
                    instance.Value.Disconnect();
                }
                connectedInstances = null;
            }

            isDisposed = true;
        }

        ~CasparCgDeviceFactory()
        {
            // Finalizer calls Dispose(false)
            Dispose(false);
        }

        private class ConnectionComparer : IEqualityComparer<CasparCGConnectionSettings>
        {
            public bool Equals(CasparCGConnectionSettings x, CasparCGConnectionSettings y)
            {
                if (x == null)
                    return false;
                if (y == null)
                    return false;

                return x.Hostname?.ToLower() == y.Hostname?.ToLower() && x.Port == y.Port;
            }

            public int GetHashCode([DisallowNull] CasparCGConnectionSettings obj)
            {
                unchecked
                {

                    int hash = 17;
                    // Suitable nullity checks etc, of course :)
                    hash = hash * 23 + obj.Hostname?.GetHashCode() ?? 0;
                    hash = hash * 23 + obj.Port.GetHashCode();
                    return hash;
                }
            }
        }

    }




}
