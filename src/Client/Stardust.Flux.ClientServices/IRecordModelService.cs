using Stardust.Flux.Client.Models;
using Stardust.Flux.Client.Services;
using Stardust.Flux.Contract.DTO.Schedule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stardust.Flux.ClientServices
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

            if (manualRecords.IsSuccessStatusCode)
                records.AddRange(manualRecords.Content.Select(x => x).Select(x => CreateModel(x)));

            if (scheduledRecords.IsSuccessStatusCode)
                records.AddRange(scheduledRecords.Content.Select(x => x).Select(x => CreateModel(x)));

            return records;
        }


        static ScheduledRecord CreateModel(ScheduleEventDto<RecordParameters> recordDto)
        {
            return new ScheduledRecord
            {
                Id = recordDto.Id,
                Start = recordDto.ScheduleAt,
                End = recordDto.ScheduleAt.AddSeconds(recordDto.DurationSeconds),
                Name = recordDto.Name

            };
        }
    }
}
