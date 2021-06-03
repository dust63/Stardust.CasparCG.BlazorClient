using Newtonsoft.Json;
using System;
using System.Collections.Generic;


namespace Stardust.Flux.EventModels
{
    public abstract class EventMessage
    {
        public string JobId;

        public TimeSpan Duration { get; set; }

        public Dictionary<string, object> ExtrasParams { get; set; }


        public EventMessage AddExtraParams<T>(string key, T data)
        {
            if (ExtrasParams == null)
                ExtrasParams = new Dictionary<string, object>();
            ExtrasParams.Add(key, data);
            return this;
        }

        public EventMessage SerializeAndAddExtraParams<T>(object toSerialize)
        {
            if (ExtrasParams == null)
                ExtrasParams = new Dictionary<string, object>();
            var json = JsonConvert.SerializeObject(toSerialize);
            var dictionnary = JsonConvert.DeserializeObject< Dictionary<string, object>>(json);
            foreach (var item in dictionnary)
            {
                if (ExtrasParams.ContainsKey(item.Key))
                    ExtrasParams[item.Key] = item.Value;
                else
                    ExtrasParams.Add(item.Key, item.Value);
            }
            return this;
        }

        public override string ToString()
        {
            return JobId;
        }
    }

    public class EventStartMessage : EventMessage
    {


    }

    public class EventStopMessage : EventMessage
    {

    }


}