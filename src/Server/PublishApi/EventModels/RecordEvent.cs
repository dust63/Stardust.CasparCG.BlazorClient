using System;
using System.Collections.Generic;
namespace Stardust.Flux.EventModels
{


    public interface RecordStartEvent
    {
        string JobId { get; set; }

        Dictionary<string, object> ExtrasParams { get; set; }

    }

    public class RecordStopEvent
    {
        public string JobId;

        public Dictionary<string, object> ExtrasParams { get; set; }
    }


}