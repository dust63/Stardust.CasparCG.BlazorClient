using System.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Stardust.Flux.PublishApi.Youtube;
using Google.Apis.YouTube.v3;

namespace Stardust.Flux.PublishApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class YoutubeController : ControllerBase
    {
        private readonly ILogger<YoutubeController> _logger;
        private readonly YoutubeAppService _youtubeService;
        private readonly AuthenticateService _authService;

        public YoutubeController(ILogger<YoutubeController> logger,
        AuthenticateService authService,
        YoutubeAppService youtubeService)
        {
            _authService = authService;
            _logger = logger;
            _youtubeService = youtubeService;
        }


        [HttpGet]
        [Route("Account")]
        public Task<IDictionary<string, string>> GetAccount(CancellationToken cancellationToken,
        [FromQuery] int pageIndex = 0,
        [FromQuery] int pageSize = 100)
        {
            return _youtubeService.GetAccountsInfo(pageIndex, pageSize, cancellationToken);
        }

        /// <summary>
        /// Ask user to grant acces to his channel. The grants information are stored in db
        /// </summary>
        /// <param name="name"></param>
        /// <param name="accountId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Account/Grant")]
        public IActionResult AddChannelAccess([FromQuery] string name, [FromQuery] string accountId)
        {
            var url = _authService.GetAuthorizationUrl(HttpContext, accountId ?? Guid.NewGuid().ToString(), name, YouTubeService.Scope.Youtube);
            return Redirect(url);
        }

        /// <summary>
        /// Remove the grants information stored in db for a given accountId
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Account/Revoke")]
        public async Task<IActionResult> RevokeChannelAccess([FromQuery] string accountId)
        {
            if (accountId is null)
            {
                throw new ArgumentNullException(nameof(accountId));
            }
            try
            {
                await _authService.RevokeToken(accountId, YouTubeService.Scope.Youtube);
                return Ok(accountId);
            }
            catch (NoAccountFoundException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        [Route("Account/Delete")]
        public async Task<IActionResult> DeleteChannelAccess([FromQuery] string accountId)
        {
            if (accountId is null)
            {
                throw new ArgumentNullException(nameof(accountId));
            }
            try
            {
                await _authService.DeleteToken(accountId, YouTubeService.Scope.Youtube);
                return Ok();
            }
            catch (NoAccountFoundException e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Used for oauth return token
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Authorized")]
        public Task<string> Authorized(string code)
        {
            return _authService.GetYoutubeAuthenticationToken(this, code);
        }

        [HttpPost]
        [Route("Upload")]
        public async Task<IActionResult> Upload(UploadRequest uploadRequest, CancellationToken cancellationToken)
        {
            if (uploadRequest is null)
            {
                throw new ArgumentNullException(nameof(uploadRequest));
            }

            if (uploadRequest.AccountId is null)
            {
                throw new ArgumentNullException(nameof(uploadRequest.AccountId));
            }

            if (string.IsNullOrWhiteSpace(uploadRequest.FilePath))
            {
                throw new ArgumentException($"'{nameof(uploadRequest.FilePath)}' need to be defined.");
            }

            try
            {
                var jobId = await _youtubeService.UploadFile(HttpContext, uploadRequest, cancellationToken);
                return Ok(jobId);
            }
            catch (NoAccountFoundException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        [Route("Categories")]
        public async Task<IActionResult> GetCategories(CancellationToken cancellationToken,
        [FromQuery] string accountId,
        [FromQuery] string regionCode = "US")
        {
            if (accountId is null)
            {
                throw new ArgumentNullException(nameof(accountId));
            }

            try
            {
                var categories = await _youtubeService.GetCategories(HttpContext, regionCode, accountId, cancellationToken);
                return Ok(categories);
            }
            catch (NoAccountFoundException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        [Route("ChannelInfo")]
        public async Task<IActionResult> GetChannelInfo(CancellationToken cancellationToken,
         [FromQuery] string accountId)
        {
            if (accountId is null)
            {
                throw new ArgumentNullException(nameof(accountId));
            }

            try
            {
                var channelInfo = await _youtubeService.GetChannelInfo(HttpContext, accountId, cancellationToken);
                return Ok(channelInfo);
            }
            catch (NoAccountFoundException e)
            {
                return BadRequest(e.Message);
            }
        }


        [HttpGet]
        [Route("Broadcast/List")]
        public async Task<IActionResult> GetBroadcasts(CancellationToken cancellationToken,
          [FromQuery] string accountId,
           [FromQuery] string pageToken,
            [FromQuery] int pageSize = 100)
        {
            if (string.IsNullOrEmpty(accountId))
            {
                throw new ArgumentException($"« {nameof(accountId)} » ne peut pas être vide ou avoir la valeur Null.", nameof(accountId));
            }

            try
            {
                var broadcast = await _youtubeService.GetLiveBroadcasts(HttpContext, accountId, pageToken, pageSize, cancellationToken);
                return Ok(broadcast);
            }
            catch (NoAccountFoundException e)
            {
                return BadRequest(e.Message);
            }

        }


        [HttpPost]
        [Route("Broadcast")]
        public async Task<IActionResult> InsertBroadcast(
            CancellationToken cancellationToken,
            [FromBody] BroadcastRequestDto broadcastRequest)
        {
            if (broadcastRequest is null)
            {
                throw new ArgumentNullException(nameof(broadcastRequest));
            }

            if (broadcastRequest.AccountId is null)
            {
                throw new ArgumentNullException(nameof(broadcastRequest.AccountId));
            }

            try
            {
                var broadcast = await _youtubeService.InsertBroadcast(HttpContext, broadcastRequest, cancellationToken);
                return Ok(broadcast);
            }
            catch (NoAccountFoundException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete]
        [Route("Broadcast")]
        public async Task<IActionResult> DeleteBroadcast(CancellationToken cancellationToken,
          [FromQuery] string accountId,
          [FromQuery] string broadcastId
        )
        {
            if (string.IsNullOrEmpty(accountId))
            {
                throw new ArgumentException($"« {nameof(accountId)} » ne peut pas être vide ou avoir la valeur Null.", nameof(accountId));
            }

            if (string.IsNullOrEmpty(broadcastId))
            {
                throw new ArgumentException($"« {nameof(broadcastId)} » ne peut pas être vide ou avoir la valeur Null.", nameof(broadcastId));
            }

            try
            {
                var broadcast = await _youtubeService.DeleteBroadcast(HttpContext, accountId, broadcastId, cancellationToken);
                return Ok(broadcast);
            }
            catch (NoAccountFoundException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut]
        [Route("Broadcast")]
        public async Task<IActionResult> UpdateBroadcast(CancellationToken cancellationToken,
        [FromBody] BroadcastRequestDto broadcastRequest)
        {
            if (broadcastRequest is null)
            {
                throw new ArgumentNullException(nameof(broadcastRequest));
            }

            if (broadcastRequest.AccountId is null)
            {
                throw new ArgumentNullException(nameof(broadcastRequest.AccountId));
            }

            try
            {
                var broadcast = await _youtubeService.UpdateBroadcast(HttpContext, broadcastRequest, cancellationToken);
                return Ok(broadcast);
            }
            catch (NoAccountFoundException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut]
        [Route("Broadcast/Privacy")]
        public async Task<IActionResult> UpdateBroadcastPrivacyStatus(CancellationToken cancellationToken,
          [FromQuery] string accountId,
          [FromQuery] string broadcastId,
          [FromQuery] PrivacyStatus privacyStatus)
        {
            if (string.IsNullOrEmpty(accountId))
            {
                throw new ArgumentException($"« {nameof(accountId)} » ne peut pas être vide ou avoir la valeur Null.", nameof(accountId));
            }

            if (string.IsNullOrEmpty(broadcastId))
            {
                throw new ArgumentException($"« {nameof(broadcastId)} » ne peut pas être vide ou avoir la valeur Null.", nameof(broadcastId));
            }

            try
            {
                var broadcast = await _youtubeService.UpdateBroadcastStatus(HttpContext, accountId, broadcastId, privacyStatus, cancellationToken);
                return Ok(broadcast);
            }
            catch (NoAccountFoundException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        [Route("Broadcast/Bind")]
        public async Task<IActionResult> InsertBroadcast(CancellationToken cancellationToken,
        [FromQuery] string accountId,
        [FromQuery] string broadcastId,
        [FromQuery] string streamId)
        {
            if (accountId is null)
            {
                throw new ArgumentNullException(nameof(accountId));
            }

            if (broadcastId is null)
            {
                throw new ArgumentNullException(nameof(broadcastId));
            }

            if (streamId is null)
            {
                throw new ArgumentNullException(nameof(streamId));
            }

            try
            {
                var broadcast = await _youtubeService.BindLiveToBroadcast(HttpContext, accountId, broadcastId, streamId, cancellationToken);
                return Ok(broadcast);
            }
            catch (NoAccountFoundException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        [Route("LiveStream")]
        public async Task<IActionResult> InsertLiveStream(CancellationToken cancellationToken, [FromBody] LiveStreamRequestDto request)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (request.AccountId is null)
            {
                throw new ArgumentNullException(nameof(request.AccountId));
            }

            try
            {
                var liveStream = await _youtubeService.InsertLiveStream(HttpContext, request, cancellationToken);
                return Ok(liveStream);
            }
            catch (NoAccountFoundException e)
            {
                return BadRequest(e.Message);
            }
        }


        [HttpGet]
        [Route("LiveStream")]
        public async Task<IActionResult> GetLiveStream(CancellationToken cancellationToken,
        [FromQuery] string accountId,
        [FromQuery] string streamId)
        {
            if (accountId is null)
            {
                throw new ArgumentNullException(nameof(accountId));
            }

            if (streamId is null)
            {
                throw new ArgumentNullException(nameof(streamId));
            }

            try
            {
                var liveStream = await _youtubeService.GetLiveStream(HttpContext, accountId, streamId, cancellationToken);
                return Ok(liveStream);
            }
            catch (NoAccountFoundException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        [Route("LiveStream/List")]
        public async Task<IActionResult> GetLiveStreams(CancellationToken cancellationToken,
        [FromQuery] string accountId,
        [FromQuery] string pageToken,
        [FromQuery] int pageSize = 100)
        {
            if (accountId is null)
            {
                throw new ArgumentNullException(nameof(accountId));
            }


            try
            {
                var liveStream = await _youtubeService.GetLiveStreams(HttpContext, accountId, pageToken, pageSize, cancellationToken);
                return Ok(liveStream);
            }
            catch (NoAccountFoundException e)
            {
                return BadRequest(e.Message);
            }
        }


        [HttpDelete]
        [Route("LiveStream")]
        public async Task<IActionResult> DeleteLiveStreams(CancellationToken cancellationToken,
        [FromQuery] string accountId,
        [FromQuery] string liveStreamId)
        {
            if (accountId is null)
            {
                throw new ArgumentNullException(nameof(accountId));
            }

            try
            {
                var liveStream = await _youtubeService.DeleteLiveStream(HttpContext, accountId, liveStreamId, cancellationToken);
                return Ok(liveStream);
            }
            catch (NoAccountFoundException e)
            {
                return BadRequest(e.Message);
            }
        }







    }
}
