using StarDust.CasparCG.net.Device;
using StarDust.CasparCG.net.Connection;
using System;
using System.Linq;
using StartDust.Blazor.CasparCGClient.Core;

namespace StartDust.Blazor.CasparCGClient.Domain
{
    public class CasparCGRecordSource : IRecordSource
    {
        public CasparCGSlot Slot { get; set; }
              
        public uint CurrentRecordChannel { get;private set; }

        public uint CurrentConsumerIndex { get;private set; }

        public ICasparDevice CasparCGDevice { get; }

        public CasparCGConnectionSettings ConnectionSettings { get; set; }
      

        public CasparCGRecordSource(ICasparDevice casparCgDevice)
        {
            CasparCGDevice = casparCgDevice;
        }

        public void Start()
        {
            var filePath = $"Capture-{DateTime.Now.ToString("yyyy-MM-dd-hhmmss")}.mp4";
            Start(filePath);
        }

        public void Start(string filePath)
        {
            CurrentRecordChannel = 1;
            StartInternal(CasparCGDevice.Channels.First(x=> x.ID == CurrentRecordChannel), filePath);
        }

        public void Start(uint channel, string filePath)
        {
            if (channel <= 0)
                throw new ArgumentOutOfRangeException(nameof( channel));
            CurrentRecordChannel =channel;
            StartInternal(CasparCGDevice.Channels.Single(x => x.ID == channel), filePath);
        }

        public void Start(string filePath, TimeSpan duration)
        {
            throw new NotImplementedException();
        }

        protected void StartInternal(ChannelManager channel, string filePath)
        {
            if (CurrentConsumerIndex > 0)
                throw new RecordPendingException();
            Connect();
            CurrentConsumerIndex = 1;
            channel.Add(StarDust.CasparCG.net.Models.ConsumerType.File, CurrentConsumerIndex, string.Join(" ", filePath, Slot.CodecOptions));
        }

        private void Connect()
        {
            if (CasparCGDevice.IsConnected)
                return;

            if (ConnectionSettings == null)
                throw new ConnectionSettingsException();

            CasparCGDevice.ConnectionSettings = ConnectionSettings;
            CasparCGDevice.Connect();
        }

        public void Stop()
        {
            if (CurrentRecordChannel <= 0)
                return;
            StopInternal(CasparCGDevice.Channels.Single(x => x.ID == CurrentRecordChannel));
        }
        protected void StopInternal(ChannelManager channel)
        {
            Connect();
            channel.Remove(1);
            CurrentRecordChannel = 0;
            CurrentConsumerIndex = 0;
        }
    }
}
