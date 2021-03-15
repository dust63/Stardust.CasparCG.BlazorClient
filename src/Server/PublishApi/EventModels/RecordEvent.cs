using System;
using System.Collections.Generic;
namespace Stardust.Flux.PublishApi.EventModels
{


    public class RecordStartEvent
    {
        public string JobId;

        public Dictionary<string, object> ExtrasParams { get; set; }

    }

    public class RecordStopEvent
    {
        public string JobId;

        public Dictionary<string, object> ExtrasParams { get; set; }
    }


}