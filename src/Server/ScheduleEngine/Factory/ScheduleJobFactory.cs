using System;
using Stardust.Flux.ScheduleEngine.DTO;
using Stardust.Flux.ScheduleEngine.Models;

namespace Stardust.Flux.ScheduleEngine.Factory
{
    public static class EventJobFactory
    {

        public static Event CreateEntity(BaseEventDto request)
        {
            return new Event
            {
                EventId = Guid.NewGuid().ToString(),
                Duration = TimeSpan.FromSeconds(request.DurationSeconds),
                ExtraParams = request.ExtraParams,
                RecordSlotId = request.RecordSlotId,
                Name = request.Name,
            };
        }
        public static Event CreateEntity(ScheduleEventDto scheduleRecordRequest)
        {
            var job = CreateEntity((BaseEventDto)scheduleRecordRequest);
            job.RecordType = EventType.Schedule;
            job.ScheduleAt = scheduleRecordRequest.ScheduleAt;
            return job;
        }


        public static Event CreateEntity(RecuringEventDto scheduleRecordRequest)
        {
            var job = CreateEntity((BaseEventDto)scheduleRecordRequest);
            job.RecordType = EventType.Recuring;
            job.CronExpression = scheduleRecordRequest.CronExpression;
            job.StartRecordJobId = Guid.NewGuid().ToString();
            return job;
        }

        public static void UpdateEntity(Event recordJob, ScheduleEventDto recordRequest)
        {
            UpdateEntity(recordJob, (BaseEventDto)recordRequest);
            if (recordRequest.ScheduleAt > DateTime.UtcNow)
            {
                recordJob.ScheduleAt = recordJob.ScheduleAt;
            }
        }


        public static void UpdateEntity(Event recordJob, BaseEventDto recordRequest)
        {
            recordJob.Name = recordRequest.Name;
            recordJob.Duration = TimeSpan.FromSeconds(recordRequest.DurationSeconds);
            recordJob.ExtraParams = recordRequest.ExtraParams;
        }




        public static EventResponseDto<ScheduleEventDto> CreateScheduleResponseDto(Event recordJob)
        {
            return new EventResponseDto<ScheduleEventDto>
            {
                StartRecordJobId = recordJob.StartRecordJobId,
                StopRecordJobId = recordJob.StopRecordJobId,
                Record = new ScheduleEventDto
                {
                    Id = recordJob.EventId,
                    DurationSeconds = recordJob.Duration.TotalSeconds,
                    ExtraParams = recordJob.ExtraParams,
                    Name = recordJob.Name,
                    RecordSlotId = recordJob.RecordSlotId,
                    ScheduleAt = recordJob.ScheduleAt ?? DateTime.MinValue
                }
            };
        }

        public static void UpdateEntity(Event recordJob, RecuringEventDto recordRequest)
        {
            UpdateEntity(recordJob, (BaseEventDto)recordRequest);
            recordJob.CronExpression = recordJob.CronExpression;
        }

        public static EventResponseDto<RecuringEventDto> CreateRecuringResponseDto(Event recordJob)
        {
            return new EventResponseDto<RecuringEventDto>
            {
                StartRecordJobId = recordJob.StartRecordJobId,
                StopRecordJobId = recordJob.StopRecordJobId,
                Record = new RecuringEventDto
                {
                    Id = recordJob.EventId,
                    DurationSeconds = recordJob.Duration.TotalSeconds,                   
                    Name = recordJob.Name,
                    RecordSlotId = recordJob.RecordSlotId,
                    CronExpression = recordJob.CronExpression
                }
            };
        }
    }
}