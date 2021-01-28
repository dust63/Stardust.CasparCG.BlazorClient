using System;

namespace StartDust.Blazor.CasparCGClient.Core
{
    public interface IRecordSource
    {
        /// <summary>
        /// Slot to record
        /// </summary>
        CasparCGSlot Slot { get; set; }


        /// <summary>
        /// Start to record
        /// </summary>
        void Start();

        /// <summary>
        /// Start to record for the given file path
        /// </summary>
        /// <param name="filePath"></param>
        void Start(string filePath);

        /// <summary>
        /// Start to record for the given file and to a given duration
        /// </summary>
        /// <param name="filePath">path of the file to record</param>
        /// <param name="duration">how many time to record</param>
        void Start(string filePath,TimeSpan duration);

        /// <summary>
        /// Start to record for the given file and to a given duration
        /// </summary>
        /// <param name="channel">channel to record</param>
        /// <param name="filePath">path of the file to record</param>     
        void Start(uint channel, string filePath);

        /// <summary>
        /// Stop the record
        /// </summary>
        void Stop();

    }
}
