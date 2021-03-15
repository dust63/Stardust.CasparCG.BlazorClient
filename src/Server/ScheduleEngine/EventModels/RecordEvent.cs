using System;
using System.Collections.Generic;
namespace Stardust.Flux.EventModels
{
    public abstract class RecordEvent
    {
        public string JobId;

        public Dictionary<string, object> ExtrasParams { get; set; }


        public RecordEvent AddExtraParams<T>(string key, T data)
        {
            if (ExtrasParams == null)
                ExtrasParams = new Dictionary<string, object>();
            ExtrasParams.Add(key, data);
            return this;
        }



        public override string ToString()
        {
            return JobId;
        }
    }

    public class RecordStartEvent : RecordEvent
    {


    }

    public class RecordStopEvent : RecordEvent
    {

    }


}