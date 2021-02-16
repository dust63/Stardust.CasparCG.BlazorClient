using System;
using Stardust.Flux.ScheduleEngine.DTO;
using Stardust.Flux.ScheduleEngine.Models;

namespace Stardust.Flux.ScheduleEngine.Factory
{
    public static class RecordJobFactory
    {

        public static RecordJob CreateEntity(BaseRecordJobDto request)
        {
            return new RecordJob
            {
                RecordJobId = Guid.NewGuid().ToString(),
                Duration = TimeSpan.FromSeconds(request.DurationSeconds),
                Filename = request.Filename,
                RecordSlotId = request.RecordSlotId,
                Name = request.Name,
            };
        }
        public static RecordJob CreateEntity(ScheduleRecordJobDto scheduleRecordRequest)
        {
            var job = CreateEntity((BaseRecordJobDto)scheduleRecordRequest);
            job.RecordType = RecordJobType.Schedule;
            job.ScheduleAt = scheduleRecordRequest.ScheduleAt;
            return job;
        }


        public static RecordJob CreateEntity(RecuringRecordJobDto scheduleRecordRequest)
        {
            var job = CreateEntity((BaseRecordJobDto)scheduleRecordRequest);
            job.RecordType = RecordJobType.Recuring;
            job.CronExpression = scheduleRecordRequest.CronExpression;
            job.StartRecordJobId = Guid.NewGuid().ToString();
            return job;
        }

        public static void UpdateEntity(RecordJob recordJob, ScheduleRecordJobDto recordRequest)
        {
            UpdateEntity(recordJob, (BaseRecordJobDto)recordRequest);
            if (recordRequest.ScheduleAt > DateTime.UtcNow)
            {
                recordJob.ScheduleAt = recordJob.ScheduleAt;
            }
        }


        public static void UpdateEntity(RecordJob recordJob, BaseRecordJobDto recordRequest)
        {
            recordJob.Name = recordRequest.Name;
            recordJob.Duration = TimeSpan.FromSeconds(recordRequest.DurationSeconds);
            recordJob.Filename = recordJob.Filename;
        }

        public static RecordResponseDto<ScheduleRecordJobDto> CreateScheduleResponseDto(RecordJob recordJob)
        {
            return new RecordResponseDto<ScheduleRecordJobDto>
            {
                StartRecordJobId = recordJob.StartRecordJobId,
                StopRecordJobId = recordJob.StopRecordJobId,
                Record = new ScheduleRecordJobDto
                {
                    Id = recordJob.RecordJobId,
                    DurationSeconds = recordJob.Duration.TotalSeconds,
                    Filename = recordJob.Filename,
                    Name = recordJob.Name,
                    RecordSlotId = recordJob.RecordSlotId,
                    ScheduleAt = recordJob.ScheduleAt ?? DateTime.MinValue
                }
            };
        }

        public static void UpdateEntity(RecordJob recordJob, RecuringRecordJobDto recordRequest)
        {
            UpdateEntity(recordJob, (BaseRecordJobDto)recordRequest);
            recordJob.CronExpression = recordJob.CronExpression;
        }

        public static RecordResponseDto<RecuringRecordJobDto> CreateRecuringResponseDto(RecordJob recordJob)
        {
            return new RecordResponseDto<RecuringRecordJobDto>
            {
                StartRecordJobId = recordJob.StartRecordJobId,
                StopRecordJobId = recordJob.StopRecordJobId,
                Record = new RecuringRecordJobDto
                {
                    Id = recordJob.RecordJobId,
                    DurationSeconds = recordJob.Duration.TotalSeconds,
                    Filename = recordJob.Filename,
                    Name = recordJob.Name,
                    RecordSlotId = recordJob.RecordSlotId,
                    CronExpression = recordJob.CronExpression
                }
            };
        }
    }
}