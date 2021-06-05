using Elsa.ActivityResults;
using Elsa.Attributes;
using Elsa.Expressions;
using Elsa.Services;
using Elsa.Services.Models;
using OBSWebsocketDotNet;
using System;
using System.Linq;

namespace Stardust.Flux.WorkflowEngine.Activities
{
    [Activity(
     Category = "OBS",
     DisplayName = "Obs set ffmpeg settings",
     Description = "Set ffmpeg media source settings",
     Outcomes = new[] { "Done","SourceNotFound" })]
    public class ObsSetFFMPEGMediaSourceSettings : ObsBaseActivity
    {
        public ObsSetFFMPEGMediaSourceSettings(ObsWebsocketInstanceFactory obsWebSocketInstanceService) : base(obsWebSocketInstanceService)
        {

        }



      

        [ActivityProperty(Hint = "Source name to activate", Label = "Source Name", SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid })]
        public string SourceName { get; set; }

        [ActivityProperty(Hint = "The video filename", Label = "Media filename", SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid })]
        public string MediaFilename { get; set; }

        [ActivityProperty(Hint = "Put a black when video is ended", Label = "Clear at End", SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid })]
        public bool? ClearAtEnd { get; set; }

        [ActivityProperty(Hint = "Restart vidoe when layer is visible", Label = "Restart on activate", SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid })]
        public bool? RestartOnActivate { get; private set; }

        [ActivityProperty(Hint = "Loop video", Label = "Loop", SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid })]
        public bool? Looping { get; private set; }

        [ActivityProperty(Hint = "Use hardware decode", Label = "Hardware Decode", SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid })]
        public bool? HWDecode { get; private set; }

        protected override IActivityExecutionResult OnExecute(ActivityExecutionContext context)
        {
            base.OnExecute(context);
            var sourceSettings = OBSWebsocket.GetSourceSettings(SourceName);
            if (sourceSettings == null)
                return Outcome("SourceNotFound");
            var ffmpegSettings = sourceSettings.Settings.ToObject<FFMpegSourceSettings>();

            ffmpegSettings.LocalFile = string.IsNullOrWhiteSpace(MediaFilename) ? ffmpegSettings.LocalFile : MediaFilename;
            ffmpegSettings.ClearOnMediaEnd = ClearAtEnd ?? ffmpegSettings.ClearOnMediaEnd;
            ffmpegSettings.RestartOnActivate = RestartOnActivate ?? ffmpegSettings.RestartOnActivate;
            ffmpegSettings.Looping = Looping ?? ffmpegSettings.Looping;
            ffmpegSettings.HWDecode = HWDecode ?? ffmpegSettings.HWDecode;
            OBSWebsocket.SetMediaSourceSettings(new MediaSourceSettings { SourceName = SourceName,SourceType = sourceSettings.SourceKind, Media = ffmpegSettings });

            return Done(ffmpegSettings);
        }
   

       
    }
}
