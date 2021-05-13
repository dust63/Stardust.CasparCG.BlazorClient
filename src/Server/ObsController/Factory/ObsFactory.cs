using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace ObsController.Factory
{
    public class ObsFactory
    {
        public ObsFactory()
        {
            _obsWebsocket = new Dictionary<string, OBSWebsocketDotNet.OBSWebsocket>();
        }
        private static Dictionary<string, OBSWebsocketDotNet.OBSWebsocket> _obsWebsocket;

        public static OBSWebsocketDotNet.OBSWebsocket GetInstance(IServiceProvider serviceProvider)
        {
            var factory = serviceProvider.GetService(typeof(ObsFactory)) as ObsFactory;
            return factory.GetInstanceInternal(serviceProvider);           
        }

        ///<summary>Get an instance of obs web socket. Avoid multiple connection</summary>
        ///<param name="serviceProdivder">Service provider</param>
        ///<return>An instance of obs connected</return>
        protected OBSWebsocketDotNet.OBSWebsocket GetInstanceInternal(IServiceProvider serviceProvider)
        {
            var configuration = serviceProvider.GetService(typeof(IConfiguration)) as IConfiguration;
            var obsUrl = configuration["Obs.Websocket.Url"];
            if(string.IsNullOrWhiteSpace(obsUrl))
            throw new InvalidOperationException("The Obs.Websocket.Url settings is not setted on the configuration file");
            var obsPassword = configuration["Obs.Websocket.Password"];

            if (_obsWebsocket.ContainsKey(obsUrl))
                return _obsWebsocket[obsUrl];

            _obsWebsocket.Add(obsUrl, new OBSWebsocketDotNet.OBSWebsocket());
            _obsWebsocket[obsUrl].Connect(obsUrl, obsPassword);

            return _obsWebsocket[obsUrl];
        }

        ~ObsFactory()
        {
            foreach (var obs in _obsWebsocket)
            {
                obs.Value?.Disconnect();
            }
            _obsWebsocket = null;
        }


    }
}