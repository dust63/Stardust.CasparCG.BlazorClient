using System.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Stardust.Flux.PublishApi.Youtube;
using Google.Apis.Auth.OAuth2.Responses;
using Microsoft.Extensions.Options;
using Google.Apis.YouTube.v3;


namespace Stardust.Flux.PublishApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class YoutubeController : ControllerBase
    {

        private readonly ILogger<YoutubeController> _logger;
        private readonly YoutubeAppService youtubeService;

        private readonly YoutubeApiOptions _apiOptions;

        public YoutubeController(ILogger<YoutubeController> logger, IOptions<YoutubeApiOptions> apiOptions, YoutubeAppService youtubeService)
        {
            _logger = logger;
            _apiOptions = apiOptions.Value;
            this.youtubeService = youtubeService;
        }


        [HttpGet]
        [Route("Account")]
        public Task<IDictionary<string, string>> GetAccount(CancellationToken cancellationToken, int pageIndex = 0, int pageSize = 100)
        {
            return youtubeService.GetAccountInfo(pageIndex, pageSize, cancellationToken);
        }

        [HttpPost]
        [Route("Account")]
        public Task<string> AddAccount(string name, CancellationToken cancellationToken)
        {
            return youtubeService.AddChannelAccount(name, cancellationToken);
        }

        [HttpPost]
        public Task Upload([FromQuery] string filePath, string accountId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException($"'{nameof(filePath)}' need to be defined.", nameof(filePath));
            }

            return youtubeService.UploadFile(filePath, accountId, cancellationToken);
        }




    }
}
