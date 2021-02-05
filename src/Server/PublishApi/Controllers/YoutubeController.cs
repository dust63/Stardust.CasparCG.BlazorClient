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
        public Task<IDictionary<string, string>> GetAccount(CancellationToken cancellationToken, int pageIndex = 0, int pageSize = 100)
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
        public async Task<IActionResult> AddChannelAccess(string name, string accountId)
        {
            var url = await _authService.GetAuthorizationUrl(HttpContext, accountId ?? Guid.NewGuid().ToString(), name, YouTubeService.Scope.Youtube);
            return Redirect(url);
        }

        /// <summary>
        /// Remove the grants information stored in db for a given accountId
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Account/Revoke")]
        public async Task<IActionResult> RevokeChannelAccess(string accountId)
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
        public async Task<IActionResult> DeleteChannelAccess(string accountId)
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
                await _youtubeService.UploadFile(HttpContext, uploadRequest, cancellationToken);
                return Ok();
            }
            catch (NoAccountFoundException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        [Route("Categories")]
        public async Task<IActionResult> GetCategories(CancellationToken cancellationToken, string accountId, string regionCode = "US")
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
        public async Task<IActionResult> GetChannelInfo(CancellationToken cancellationToken, string accountId)
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

        [HttpPost]
        [Route("Broadcast/Insert")]
        public async Task<IActionResult> InsertBroadcast(CancellationToken cancellationToken, BroadcastRequestDto broadcastRequest)
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
        //TODO ADD MEHTOD FOR INSERT LIVE STREAM
        //TODO ADD MEHTOD TO REMOVE LIVE STREAM


    }
}
