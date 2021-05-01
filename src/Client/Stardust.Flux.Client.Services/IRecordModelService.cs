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

        Task<IList<ScheduledRecord>> GetAllScheduledRecords(DateTime currentDate, TimeSpan pastOffset, TimeSpan futurOffset);
    }


    public class RecordModelService : IRecordModelService
    {
        private readonly IRecordClientApi recordApi;

        public RecordModelService(IRecordClientApi recordApi)
        {
            this.recordApi = recordApi;
        }

        public async Task<IList<ScheduledRecord>> GetAllScheduledRecords(DateTime currentDate, TimeSpan pastOffset, TimeSpan futurOffset)
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
                ManageRecuringRecords(records, recuringRecords.Content, pastOffset, futurOffset);

            return records.Where(x => x.StartTime >= DateTime.UtcNow.Subtract(pastOffset) && x.EndTime <= DateTime.UtcNow.Add(futurOffset)).ToList();
        }

        private void ManageRecuringRecords(List<ScheduledRecord> records, List<RecuringEventResponse<RecordParameters>> recuringEvents, TimeSpan pastOffset, TimeSpan futurOffset)
        {
            foreach (var recuringEvent in recuringEvents)
            {
                var cronExpression = Cronos.CronExpression.Parse(recuringEvent.CronExpression);
                var recordOccurences = cronExpression.GetOccurrences(DateTime.UtcNow.Subtract(pastOffset), DateTime.UtcNow.Add(futurOffset)).Select(x => CreateModel(recuringEvent, x));
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
                Title = recordDto.Name,
                Mode = ParseMode(recordDto.CronExpression)
            };
        }

        static ScheduledRecord CreateModel(ScheduleEventDto<RecordParameters> recordDto)
        {
            return new ScheduledRecord
            {
                Id = recordDto.Id,
                StartTime = recordDto.ScheduleAt,
                EndTime = recordDto.ScheduleAt.AddSeconds(recordDto.DurationSeconds),
                Title = recordDto.Name,
                Mode = ScheduledRecord.ProgramMode.Normal
            };
        }


        static ScheduledRecord.ProgramMode ParseMode(string cronExpression)
        {
            var splitCron = cronExpression.Split();

            if(splitCron.Skip(2).Take(3).All(x=> x == "*"))           
                return ScheduledRecord.ProgramMode.Daily;      
                

            return ScheduledRecord.ProgramMode.Weekly;
        }

        static string GetCronExpression(DateTime startTime,DayOfWeek dayOfWeek, ScheduledRecord.ProgramMode mode)
        {
            if (mode == ScheduledRecord.ProgramMode.Daily)
                return $"{startTime.Minute} {startTime.Hour} * * *";

            return $"{startTime.Minute} {startTime.Hour} * * {dayOfWeek}";
        }
    }
}
