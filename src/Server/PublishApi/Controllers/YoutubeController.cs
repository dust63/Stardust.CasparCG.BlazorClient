using System.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Stardust.Flux.PublishApi.Youtube;

using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Google.Apis.YouTube.v3;
using System.Net;
using Microsoft.AspNetCore.Authentication.Cookies;
using Google.Apis.YouTube.v3.Data;

namespace Stardust.Flux.PublishApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class YoutubeController : ControllerBase
    {
        private readonly ILogger<YoutubeController> _logger;
        private readonly YoutubeAppService youtubeService;
        private readonly YoutubeApiOptions _apiOptions;
        private readonly AuthenticateService authService;

        public YoutubeController(ILogger<YoutubeController> logger, AuthenticateService authService, YoutubeAppService youtubeService)
        {
            this.authService = authService;
            _logger = logger;

            this.youtubeService = youtubeService;
        }


        [HttpGet]
        [Route("Account")]
        public Task<IDictionary<string, string>> GetAccount(CancellationToken cancellationToken, int pageIndex = 0, int pageSize = 100)
        {
            return youtubeService.GetAccountInfo(pageIndex, pageSize, cancellationToken);
        }

        [HttpGet]
        [Route("AddAccount")]
        public async Task<IActionResult> AddChannelAccess(string name, string channelId)
        {
            var url = await authService.GetAuthorizationUrl(HttpContext, channelId ?? Guid.NewGuid().ToString(), name, YouTubeService.Scope.Youtube);
            return Redirect(url.AbsoluteUri.ToString());
        }

        [HttpPost]
        [Route("RevokeAccount")]
        public Task RevokeChannelAccess(string channelId)
        {
            if (channelId is null)
            {
                throw new ArgumentNullException(nameof(channelId));
            }
            return authService.RevokeToken(channelId, YouTubeService.Scope.Youtube);
        }

        [HttpGet]
        [Route("Authorized")]
        public Task<string> Authorized(string code)
        {
            return authService.GetYoutubeAuthenticationToken(this, code);
        }

        [HttpPost]
        [Route("Upload")]
        public Task Upload(UploadRequest uploadRequest, CancellationToken cancellationToken)
        {
            if (uploadRequest is null)
            {
                throw new ArgumentNullException(nameof(uploadRequest));
            }

            if (uploadRequest.ChannelId is null)
            {
                throw new ArgumentNullException(nameof(uploadRequest.ChannelId));
            }

            if (string.IsNullOrWhiteSpace(uploadRequest.FilePath))
            {
                throw new ArgumentException($"'{nameof(uploadRequest.FilePath)}' need to be defined.");
            }

            return youtubeService.UploadFile(uploadRequest, cancellationToken);
        }

        [HttpGet]
        [Route("Categories")]
        public Task<IList<VideoCategory>> GetCategories(CancellationToken cancellationToken, string channelId, string regionCode = "US")
        {
            if (channelId is null)
            {
                throw new ArgumentNullException(nameof(channelId));
            }

            return youtubeService.GetCategories(regionCode, channelId, cancellationToken);
        }


    }
}
