using Stardust.Flux.Contract.CoreApi;
using System;
using StarDust.CasparCG.net.Device;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Stardust.Flux.Contract;
using Stardust.Flux.Core;
using StarDust.CasparCG.net.Models;
using Microsoft.Extensions.Logging;

namespace Stardust.Flux.CasparControler
{

    public interface IRecordControler
    {
        /// <summary>
        /// Launch record on specific slot
        /// </summary>
        /// <param name="slotId"></param>
        /// <param name="fileName"></param>
        /// <param name="codecParams"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task StartRecord(int slotId, string fileName,CancellationToken cancellationToken);

        /// <summary>
        /// Stop record on specific slot
        /// </summary>
        /// <param name="slotId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task StopRecord(int slotId, CancellationToken cancellationToken);
    }





    public class RecordControler : ControlerBase, IRecordControler, IDisposable
    {
        private readonly IRecordSlotApi slotClientApi;

        public RecordControler(
            ILogger<RecordControler> logger, 
            IRecordSlotApi slotClientApi, 
            IFactory<ICasparDevice> casparDeviceFactory):base(logger, casparDeviceFactory)
        {          
            this.slotClientApi = slotClientApi;         
        }

  
        /// <inheritdoc/>         
        public async Task StartRecord(int slotId, string fileName, CancellationToken cancellationToken)
        {
            var slot = await GetSlotInfo(slotId, cancellationToken);
            if (slot is null)
                throw new SlotNotFoundException(slotId);

            logger.LogInformation($"Create intance of CasparCG Device. Server: ${slot.Server}");
            var casparCg = GetCasparCgInstance(slot.Server);
            logger.LogInformation($"Guessing channel: {slot.Channel} on CasarCG Device");
            var channel = casparCg.Channels[slot.Channel];

            channel.Remove((uint)slot.SlotId);

            var recordParams = ConstructParameters(slot);

            logger.LogInformation($"Adding record consumer. ServerId: {slot.ServerId}, Channel: {slot.Channel}, Filename: {fileName}, ConsumerType: {ConsumerType.File},Parameters: {recordParams}");
            channel.Add(ConsumerType.File, (uint?)slot.SlotId, $"{fileName} {recordParams}");
            logger.LogInformation($"Record consumer added. ServerId: {slot.ServerId}, Channel: {slot.Channel}, Filename: {fileName}");
        }




        public async Task StopRecord(int slotId, CancellationToken cancellationToken)
        {
           
            var slot = await GetSlotInfo(slotId, cancellationToken);
            var casparCg = GetCasparCgInstance(slot.Server);
            var channel = casparCg.Channels[slot.Channel];

            logger.LogInformation($"Removing record consumer. ServerId: {slot.ServerId}, Channel: {slot.Channel}");
            channel.Remove((uint)slot.SlotId);
            logger.LogInformation($"Record consumer removed. ServerId: {slot.ServerId}, Channel: {slot.Channel}");
        }

        private async Task<RecordSlotDto> GetSlotInfo(int slotId, CancellationToken cancellationToken)
        {
            try
            {
                logger.LogInformation($"Guessing information for slot: {slotId}");
                return await slotClientApi.Get(cancellationToken, slotId);
            }
            catch (Exception e)
            {

                logger.LogError(e, $"Error when getting slot information. Id:{slotId}");
                throw;
            }
        }

        private static string ConstructParameters(RecordSlotDto slot)
        {
            string vCodecParam = null;
            string aCodecParam = null;          
            string encParam = null;

            if (slot.VideoCodec != null)
                vCodecParam = $"-codec:v {slot.VideoCodec}".Trim();

            if (slot.AudioCodec != null)
                aCodecParam = $"-codec:a {slot.AudioCodec}".Trim();

          
            if (slot.EncodingOptions != null)
                encParam = slot.EncodingOptions.Trim();

            return string.Join(" ", vCodecParam, aCodecParam, encParam);
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposing)
            {
                base.Dispose(false);
            }
        }
    }

}
