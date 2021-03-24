using Stardust.Flux.Contract.CoreApi;
using System;
using StarDust.CasparCG.net.Device;
using System.Threading;
using System.Threading.Tasks;
using Stardust.Flux.Contract;
using Stardust.Flux.Core;
using StarDust.CasparCG.net.Models;
using Microsoft.Extensions.Logging;

namespace Stardust.Flux.CasparControler
{
    public interface IListStreamControler
    {
        /// <summary>
        /// Launch record on specific slot
        /// </summary>
        /// <param name="slotId"></param>
        /// <param name="fileName"></param>
        /// <param name="codecParams"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task StartStreaming(int slotId, string url, CancellationToken cancellationToken);

        /// <summary>
        /// Stop record on specific slot
        /// </summary>
        /// <param name="slotId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task StopStreaming(int slotId, CancellationToken cancellationToken);
    }

    public class LiveStreamControler : ControlerBase, IListStreamControler, IDisposable
    {
        private readonly ILiveSlotApi slotClientApi;

        public LiveStreamControler(
            ILogger<RecordControler> logger, 
            ILiveSlotApi slotClientApi, 
            IFactory<ICasparDevice> casparDeviceFactory):base(logger, casparDeviceFactory)
        {          
            this.slotClientApi = slotClientApi;         
        }

  
        /// <inheritdoc/>         
        public async Task StartStreaming(int slotId,string url, CancellationToken cancellationToken)
        {
            var slot = await GetSlotInfo(slotId, cancellationToken);
            if (slot is null)
                throw new SlotNotFoundException(slotId);

            logger.LogInformation($"Create intance of CasparCG Device. Server: ${slot.Server}");
            var casparCg = GetCasparCgInstance(slot.Server);
            logger.LogInformation($"Guessing channel: {slot.Channel} on CasparCG Device");
            var channel = casparCg.Channels[slot.Channel];

            channel.Remove((uint)slot.SlotId);

            var streamParams = ConstructParameters(slot);

            logger.LogInformation($"Adding stream consumer. ServerId: {slot.ServerId}, Channel: {slot.Channel}, Url: {url}, ConsumerType: {ConsumerType.Stream},Parameters: {streamParams}");
            channel.Add(ConsumerType.Stream, (uint?)slot.SlotId, $"{url} {streamParams}");
            logger.LogInformation($"Stream consumer added. ServerId: {slot.ServerId}, Channel: {slot.Channel}, Url: {url}");
        }




        public async Task StopStreaming(int slotId, CancellationToken cancellationToken)
        {
           
            var slot = await GetSlotInfo(slotId, cancellationToken);
            var casparCg = GetCasparCgInstance(slot.Server);
            var channel = casparCg.Channels[slot.Channel];

            logger.LogInformation($"Removing stream consumer. ServerId: {slot.ServerId}, Channel: {slot.Channel}");
            channel.Remove((uint)slot.SlotId);
            logger.LogInformation($"Stream consumer removed. ServerId: {slot.ServerId}, Channel: {slot.Channel}");
        }

        private async Task<LiveStreamSlotDto> GetSlotInfo(int slotId, CancellationToken cancellationToken)
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

        private static string ConstructParameters(LiveStreamSlotDto slot)
        {
            string vCodecParam = null;
            string aCodecParam = null;
            string outputParam = null;
            string encParam = null;

            if (slot.VideoCodec != null)
                vCodecParam = $"-codec:v {slot.VideoCodec}".Trim();

            if (slot.AudioCodec != null)
                aCodecParam = $"-codec:a {slot.AudioCodec}".Trim();

            if (slot.OutputFormat != null)
                outputParam = $"-format {slot.OutputFormat}".Trim();

            if (slot.EncodingOptions != null)
                encParam = slot.OutputFormat.Trim();

            return string.Join(" ", vCodecParam, aCodecParam, outputParam, encParam);
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
