using System.Reflection;
using System.Security.AccessControl;
using System.Collections.ObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OBSWebsocketDotNet;
using OBSWebsocketDotNet.Types;
using Stardust.Flux.ObsController.Dto;

namespace Stardust.Flux.ObsController.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ObsStudioController : ControllerBase
    {

        private readonly ILogger<ObsStudioController> _logger;
        private readonly OBSWebsocket _obsWebsocket;

        public ObsStudioController(ILogger<ObsStudioController> logger, OBSWebsocket obsWebsocket)
        {
            _logger = logger;
            _obsWebsocket = obsWebsocket;
        }

        [HttpGet]
        [Route("/scene/current")]
        public ActionResult<OBSScene> Get()
        {

            OBSScene scene = _obsWebsocket.GetCurrentScene();
            if (scene is null)
                return NoContent();
            return Ok(scene);
        }


        [HttpGet]
        [Route("/scene/list")]
        public IActionResult GetSceneList()
        {

            var scenes = _obsWebsocket.GetSceneList();
            return Ok(scenes.Scenes.Select(x => x.Name).ToList());
        }

        [HttpGet]
        [Route("/scene/items/list")]
        public IActionResult GetSceneItemsList(string sceneName)
        {
            var scenes = _obsWebsocket.GetSceneItemList(sceneName);
            return Ok(scenes);
        }

        [HttpDelete]
        [Route("/scene/item")]
        public IActionResult DeleteSceneItem(SceneItemStub sceneItem, string sceneName = null)
        {
            _obsWebsocket.DeleteSceneItem(sceneItem, sceneName);
            return Ok();
        }

        [HttpPost]
        [Route("/scene/item/properties")]
        public IActionResult SetSceneItemProperties(SceneItemProperties sceneItemProperties, string sceneName = null)
        {
            _obsWebsocket.SetSceneItemProperties(sceneItemProperties, sceneName);
            return Ok();
        }


        [HttpPost]
        [Route("/scene/item/visibility")]
        public IActionResult SetSceneItemVisibility(string sourceName, bool isVisible, string sceneName = null)
        {
            _obsWebsocket.SetSourceRender(sourceName, isVisible, sceneName);
            return Ok();
        }

        [HttpPost]
        [Route("/scene/activate/name/{sceneName}")]
        public IActionResult ActiveSceneByName([FromRoute] string sceneName)
        {
            _obsWebsocket.SetCurrentScene(sceneName);
            return Ok();
        }

        [HttpPost]
        [Route("/scene/activate/index/{sceneIndex}")]
        public IActionResult ActiveSceneByIndex([FromRoute] int sceneIndex)
        {
            var sceneToActive = _obsWebsocket.GetSceneList().Scenes.ElementAtOrDefault(sceneIndex - 1);
            if (sceneToActive.Name == null)
                return NotFound();
            _obsWebsocket.SetCurrentScene(sceneToActive.Name);
            return Ok(sceneToActive);
        }



        [HttpGet]
        [Route("/streaming/start")]
        public IActionResult StartStreaming()
        {

            _obsWebsocket.StartStreaming();
            return Ok();
        }

        [HttpGet]
        [Route("/streaming/stop")]
        public IActionResult StopStreaming()
        {
            _obsWebsocket.StopStreaming();
            return Ok();
        }


        [HttpGet]
        [Route("/recording/start")]

        public async Task<ActionResult<RecordStatus>> StartRecording()
        {
            _obsWebsocket.StartRecording();
            RecordingStatus status = await WaitForStatusToBe(s => s.IsRecording);
            return Ok(ToRecordStatusModel(status));
        }

        private async Task<RecordingStatus> WaitForStatusToBe(Func<RecordingStatus, bool> conditionToMet, int maxRetry = 5)
        {
            RecordingStatus status;
            var i = 0;
            do
            {
                await Task.Delay((int)Math.Pow(i, 2) * 100);
                status = _obsWebsocket.GetRecordingStatus();
                i++;
            }
            while (!conditionToMet(status) && i <= maxRetry);
            return status;
        }

        [HttpGet]
        [Route("/recording/folder")]
        public ActionResult<string> GetRecordingFolder()
        {
            return Ok(_obsWebsocket.GetRecordingFolder());
        }


        [HttpPost]
        [Route("/recording/folder/{folderName}")]
        public ActionResult<string> SetRecordingFolder(string folderName)
        {
            _obsWebsocket.SetRecordingFolder(folderName);
            return Ok(_obsWebsocket.GetRecordingFolder());
        }

        [HttpPost]
        [Route("/recording/start/{folder}")]
        public async Task<ActionResult<RecordStatus>> StartRecording(string folder)
        {
            if (string.IsNullOrWhiteSpace(folder))
            {
                throw new ArgumentException($"'{nameof(folder)}' cannot be null or blank.", nameof(folder));
            }
            _obsWebsocket.SetRecordingFolder(folder);
            _obsWebsocket.StartRecording();
            var status = await WaitForStatusToBe(s => s.IsRecording);
            return Ok(ToRecordStatusModel(status));
        }


        [HttpGet]
        [Route("/recording/stop")]
        public async Task<IActionResult> StopRecording()
        {
            var beforeStopStatus = _obsWebsocket.GetRecordingStatus();
            _obsWebsocket.StopRecording();

            var actualStatus = await WaitForStatusToBe(s => !s.IsRecording);
            beforeStopStatus.IsRecording = actualStatus.IsRecording;
            return Ok(ToRecordStatusModel(beforeStopStatus));
        }

        [HttpGet]
        [Route("/recording/status")]
        public ActionResult<RecordStatus> RecordingStatus()
        {
            var status = _obsWebsocket.GetRecordingStatus();
            return base.Ok(ToRecordStatusModel(status));

        }

        private static RecordStatus ToRecordStatusModel(RecordingStatus status)
        {
            return new RecordStatus
            {
                IsRecording = status.IsRecording,
                RecordElapsed = status.RecordTimecode,
                RecordFileName = status.RecordingFilename
            };
        }

        [HttpGet]
        [Route("/media/waitforend")]
        public async Task<string> WaitForMediaEnded()
        {

            var tcs = new TaskCompletionSource<string>();
            _obsWebsocket.MediaEnded += (sender, src, e) =>
            {
                tcs.SetResult(src);
            };

            var result = await tcs.Task;
            return result;
        }

        [HttpGet]
        [Route("/source/settings")]
        public IActionResult GetSourceSettings(string sourceName, string sourceType = null)
        {

            var srcSettings = _obsWebsocket.GetSourceSettings(sourceName, sourceType);
            return Ok(srcSettings);
        }



        [HttpGet]
        [Route("/source/media/settings")]
        public IActionResult GetMediaSourceSettings(string sourceName)
        {

            var srcSettings = _obsWebsocket.GetMediaSourceSettings(sourceName);
            return Ok(srcSettings);
        }

        [HttpPost]
        [Route("/source/media/settings")]
        public IActionResult SetMediaSourceSettings(MediaSourceSettings settings)
        {

            _obsWebsocket.SetMediaSourceSettings(settings);
            return Ok();
        }
    }
}
