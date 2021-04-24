using Stardust.Flux.Client.Models;
using Stardust.Flux.Client.Services;
using Stardust.Flux.Contract.DTO.Schedule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stardust.Flux.Client.Services
{
    public interface IRecordModelService
    {

        Task<IList<ScheduledRecord>> GetAllScheduledRecords();
    }


    public class RecordModelService : IRecordModelService
    {
        private readonly IRecordClientApi recordApi;

        public RecordModelService(IRecordClientApi recordApi)
        {
            this.recordApi = recordApi;
        }

        public async Task<IList<ScheduledRecord>> GetAllScheduledRecords()
        {
            var records = new List<ScheduledRecord>();
            var manualRecords = await recordApi.GetManualRecords(0);
            var scheduledRecords = await recordApi.GetSchedule(0);
            var recuringRecords = await recordApi.GetRecuring(0);

            if (manualRecords.IsSuccessStatusCode)
                records.AddRange(manualRecords.Content.Select(x => x).Select(x => CreateModel(x)));

            if (scheduledRecords.IsSuccessStatusCode)
                records.AddRange(scheduledRecords.Content.Select(x => x).Select(x => CreateModel(x)));

            if (recuringRecords.IsSuccessStatusCode)
                ManageRecuringRecords(records, recuringRecords.Content);

            return records;
        }

        private void ManageRecuringRecords(List<ScheduledRecord> records, List<RecuringEventResponse<RecordParameters>> recuringEvents)
        {
            foreach (var recuringEvent in recuringEvents)
            {
                var cronExpression = Cronos.CronExpression.Parse(recuringEvent.CronExpression);
                var recordOccurences = cronExpression.GetOccurrences(DateTime.UtcNow.AddMonths(-6), DateTime.UtcNow.AddYears(3)).Select(x => CreateModel(recuringEvent, x));
                records.AddRange(recordOccurences);
            }

        }

        static ScheduledRecord CreateModel(RecuringEventResponse<RecordParameters> recordDto, DateTime startDate)
        {
            return new ScheduledRecord
            {
                Id = recordDto.Id,
                StartTime = startDate,
                EndTime = startDate.AddSeconds(recordDto.DurationSeconds),
                Subject = recordDto.Name,
                IsRecuring = true
            };
        }

        static ScheduledRecord CreateModel(ScheduleEventDto<RecordParameters> recordDto)
        {
            return new ScheduledRecord
            {
                Id = recordDto.Id,
                StartTime = recordDto.ScheduleAt,
                EndTime = recordDto.ScheduleAt.AddSeconds(recordDto.DurationSeconds),
                Subject = recordDto.Name

            };
        }
    }
}
