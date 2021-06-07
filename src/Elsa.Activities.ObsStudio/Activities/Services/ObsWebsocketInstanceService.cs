using Elsa.Models;
using Elsa.Services;
using Elsa.Services.Models;
using Microsoft.Extensions.DependencyInjection;
using OBSWebsocketDotNet;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;

namespace Elsa.Activities.ObsStudio.Activities
{

    public class ObsWebsocketMediaListener
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private OBSWebsocket _obsWebsocket;
        private WorkflowInstance _workflowInstance;
        private string _activityId;
        private string _sourceToTrigger;

        public ObsWebsocketMediaListener(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }



        public void Listen(OBSWebsocket obsWebsocket, ActivityExecutionContext context, string sourceToTrigger)
        {
            _obsWebsocket = obsWebsocket;
            _obsWebsocket.MediaEnded += OnMediaEnded;
            _activityId = context.ActivityId;
            _workflowInstance = context.WorkflowInstance;
            _sourceToTrigger = sourceToTrigger;       

        }

        private async void OnMediaEnded(OBSWebsocket sender, string sourceName, string sourceKind)
        {
            if (!string.IsNullOrEmpty(_sourceToTrigger) && sourceName != _sourceToTrigger)
                return;

            _obsWebsocket.MediaEnded -= OnMediaEnded;
            using var scope = _serviceScopeFactory.CreateScope();
            var interruptor = scope.ServiceProvider.GetService<IWorkflowTriggerInterruptor>();
            await interruptor.InterruptActivityAsync(_workflowInstance, _activityId, sourceName);
        }
    }


    public class ObsWebsocketInstanceFactory
    {

        private IDictionary<WorkflowInstance, OBSWebsocketDotNet.OBSWebsocket> _instances = new Dictionary<WorkflowInstance, OBSWebsocketDotNet.OBSWebsocket>(new WorkflowInstanceComparer());

        private WorkflowStatus[] _canDropStatus = { WorkflowStatus.Cancelled, WorkflowStatus.Faulted, WorkflowStatus.Finished };


        private Timer _timerDrop;



        public ObsWebsocketInstanceFactory()
        {
            IntializeTimer();

        }

        public OBSWebsocketDotNet.OBSWebsocket GetInstance(WorkflowInstance workflowInstance)
        {
            if (_instances.ContainsKey(workflowInstance))
                return _instances[workflowInstance];

            return null;

        }


        public OBSWebsocketDotNet.OBSWebsocket GetOrStoreInstance(WorkflowInstance workflowInstance, string url, string password)
        {
            if (_instances.ContainsKey(workflowInstance))
                return _instances[workflowInstance];


            var socket = new OBSWebsocketDotNet.OBSWebsocket();
            socket.Connect(url, password);
            _instances.Add(workflowInstance, socket);

            return socket;
        }

        public void DropInstance(WorkflowInstance workflowInstance)
        {
            var socket = GetInstance(workflowInstance);

            if (socket == null)
                return;


            if (socket.IsConnected)
                socket.Disconnect();

            _instances.Remove(workflowInstance);
        }





        private void OnTimerDrop(object state)
        {
            foreach (var item in _instances)
            {
                if (_canDropStatus.Contains(item.Key.WorkflowStatus))
                    DropInstance(item.Key);
            }
        }


        private void IntializeTimer()
        {
            if (_timerDrop != null)
                return;

            _timerDrop = new Timer(OnTimerDrop, null, TimeSpan.FromSeconds(15), TimeSpan.FromSeconds(1));
        }




    }

    internal class WorkflowInstanceComparer : IEqualityComparer<WorkflowInstance>
    {
        public bool Equals(WorkflowInstance x, WorkflowInstance y)
        {
            if (x == null)
                return false;

            if (y == null)
                return false;

            return x.Id == y.Id;
        }

        public int GetHashCode([DisallowNull] WorkflowInstance obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}
