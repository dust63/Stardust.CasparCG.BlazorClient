using Microsoft.Extensions.Logging;
using Stardust.Flux.Contract;
using Stardust.Flux.Core;
using StarDust.CasparCG.net.Device;
using System;
using System.Collections.Generic;

namespace Stardust.Flux.CasparControler
{
    public abstract class ControlerBase
    {
        protected readonly ILogger logger;
        private readonly IFactory<ICasparDevice> casparDeviceFactory;
        private IDictionary<ServerDto, ICasparDevice> casparCgInstance = new Dictionary<ServerDto, ICasparDevice>(new ServerEqualityComparer());
        protected bool disposed;


        public ControlerBase(
            ILogger logger,
            IFactory<ICasparDevice> casparDeviceFactory) : base()
        {
            this.logger = logger;
            this.casparDeviceFactory = casparDeviceFactory;

        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                foreach (var instance in casparCgInstance)
                {
                    try
                    {
                        if (instance.Value != null && !instance.Value.IsConnected)
                            instance.Value.Disconnect();
                    }
                    catch (Exception e)
                    {
                        logger.LogError(e, "Error while removing caspar CG instance");
                    }
                    finally
                    {
                        casparCgInstance.Remove(instance.Key);
                    }

                }
            }

            disposed = true;
        }

        // Disposable types implement a finalizer.
        ~ControlerBase()
        {
            Dispose(false);
        }


        protected ICasparDevice GetCasparCgInstance(ServerDto server)
        {
            if (casparCgInstance.ContainsKey(server))
                return casparCgInstance[server];

            var instance = casparDeviceFactory.Create();
            instance.Connect(server.Hostname, server.Port);
            casparCgInstance.Add(server, instance);

            return instance;
        }
    }
}