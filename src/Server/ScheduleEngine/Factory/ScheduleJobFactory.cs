using System;
using Stardust.Flux.ScheduleEngine.DTO;
using Stardust.Flux.ScheduleEngine.Models;

namespace Stardust.Flux.ScheduleEngine.Factory
{
    public static class EventJobFactory
    {

        public static Event CreateEntity<TParam>(BaseEventDto request, TParam extraParams)
        {
            return new Event
            {
                EventId = Guid.NewGuid().ToString(),
                Duration = TimeSpan.FromSeconds(request.DurationSeconds),
                ExtraParams = extraParams,
                ParamType = typeof(TParam).FullName,
                Name = request.Name,                
            };
        }


        public static Event CreateEntity<TParam>(ScheduleEventDto<TParam> scheduleeventRequest)
        {
            var job = CreateEntity(scheduleeventRequest, scheduleeventRequest.ExtraParams);
            job.EventType = EventType.Schedule;
            job.ScheduleAt = scheduleeventRequest.ScheduleAt;
            return job;
        }


        public static Event CreateEntity<TParam>(RecuringEventDto<TParam> scheduleeventRequest)
        {
            var job = CreateEntity(scheduleeventRequest, scheduleeventRequest.ExtraParams);
            job.EventType = EventType.Recuring;
            job.CronExpression = scheduleeventRequest.CronExpression;
            job.StartRecordJobId = Guid.NewGuid().ToString();
            return job;
        }

        public static void UpdateEntity<TParam>(Event recordJob, ScheduleEventDto<TParam> eventRequest)
        {
            UpdateEntity(recordJob, (BaseEventDto)eventRequest, eventRequest.ExtraParams);
            if (eventRequest.ScheduleAt > DateTime.UtcNow)
            {
                recordJob.ScheduleAt = recordJob.ScheduleAt;
            }
        }


        public static void UpdateEntity<TParam>(Event recordJob, BaseEventDto eventRequest, TParam extraParams)
        {
            recordJob.Name = eventRequest.Name;
            recordJob.Duration = TimeSpan.FromSeconds(eventRequest.DurationSeconds);
            recordJob.ExtraParams = extraParams;
        }




        public static EventResponseDto<ScheduleEventDto<TParam>> CreateScheduleResponseDto<TParam>(Event recordJob)
        {
            return new EventResponseDto<ScheduleEventDto<TParam>>
            {
                StartRecordJobId = recordJob.StartRecordJobId,
                StopRecordJobId = recordJob.StopRecordJobId,
                Record = new ScheduleEventDto<TParam>
                {
                    Id = recordJob.EventId,
                    DurationSeconds = recordJob.Duration.TotalSeconds,
                    ExtraParams = (TParam)recordJob.ExtraParams,
                    Name = recordJob.Name,                 
                    ScheduleAt = recordJob.ScheduleAt ?? DateTime.MinValue
                }
            };
        }

        public static void UpdateEntity<TParam>(Event recordJob, RecuringEventDto<TParam> eventRequest)
        {
            UpdateEntity(recordJob, (BaseEventDto)eventRequest, eventRequest.ExtraParams);
            recordJob.CronExpression = recordJob.CronExpression;
        }

        public static EventResponseDto<RecuringEventDto<TParam>> CreateRecuringResponseDto<TParam>(Event recordJob)
        {
            return new EventResponseDto<RecuringEventDto<TParam>>
            {
                StartRecordJobId = recordJob.StartRecordJobId,
                StopRecordJobId = recordJob.StopRecordJobId,
                Record = new RecuringEventDto<TParam>
                {
                    Id = recordJob.EventId,
                    DurationSeconds = recordJob.Duration.TotalSeconds,
                    Name = recordJob.Name,                 
                    CronExpression = recordJob.CronExpression,
                    ExtraParams = (TParam)recordJob.ExtraParams
                    
                }
            };
        }
    }
}