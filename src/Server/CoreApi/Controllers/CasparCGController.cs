using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Arch.EntityFrameworkCore.UnitOfWork;
using Arch.EntityFrameworkCore.UnitOfWork.Collections;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Stardust.Flux.Contract;
using Stardust.Flux.CoreApi.Factory;
using Stardust.Flux.CoreApi.Models.Entity;
using StarDust.CasparCG.net.Device;
using StarDust.CasparCG.net.Models;
using StarDust.CasparCG.net.Models.Media;

namespace Stardust.Flux.CoreApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CasparCGController : ControllerBase
    {
        private readonly ILogger<CasparCGController> _logger;
        private readonly IUnitOfWork unitOfWork;
        private readonly ICasparCgDeviceFactory casparDeviceFactory;

        public CasparCGController(ILogger<CasparCGController> logger, IUnitOfWork unitOfWork,  ICasparCgDeviceFactory casparDeviceFactory )
        {
            _logger = logger;
            this.unitOfWork = unitOfWork;
            this.casparDeviceFactory = casparDeviceFactory;
        }

        /// <summary>
        /// Get template
        /// </summary>
        /// <param name="serverId">id of the server configured</param>
        /// <returns></returns>
        [HttpGet]
        [Route("template/list")]
        public async Task<IList<TemplateBaseInfo>> GetTemplates([FromQuery] int serverId)
        {          

            ICasparDevice device = await GetDeviceInstanceAndConnect(serverId);
            var templates = await device.GetTemplatesAsync();
            return templates.OfType<TemplateBaseInfo>().ToList();
        }

        private async Task<ICasparDevice> GetDeviceInstanceAndConnect(int serverId)
        {
            var serverRepo = unitOfWork.GetRepository<Server>();
            var server = serverRepo.Find(serverId);

            if (server == null)
                throw new ServerNotFoundException(serverId);

            var device = casparDeviceFactory.CreateDevice(server.Hostname, server.Port);
            if (!device.IsConnected)
                await Task.Run(() => device.Connect(server.Hostname, server.Port));
            return device;
        }


        /// <summary>
        /// Get template
        /// </summary>
        /// <param name="serverId">id of the server configured</param>
        /// <returns></returns>
        [HttpGet]
        [Route("movie/list")]
        public async Task<IList<MediaInfo>> GetMovies([FromQuery] int serverId)
        {   
            var device = await GetDeviceInstanceAndConnect(serverId);
            var movies = await device.GetMediafilesAsync();
            return movies;
        }


    }
}

