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

namespace Stardust.Flux.CoreApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ServerController : ControllerBase
    {
        private readonly ILogger<ServerController> _logger;
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public ServerController(ILogger<ServerController> logger, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _logger = logger;
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        [HttpGet]
        [Route("all")]
        public Task<IPagedList<ServerDto>> Get(CancellationToken cancellationToken, int pageIndex = 0, int pageSize = 100)
        {
            var serverRepo = unitOfWork.GetRepository<Server>();
            return serverRepo
              .GetPagedListAsync(
                  orderBy: x => x.OrderBy(s => s.ServerId),
                  pageIndex: pageIndex,
                  pageSize: pageSize,
                  disableTracking: true,
                  cancellationToken: cancellationToken)
              .ContinueWith(t => PagedList.From(t.Result, source => source.Select(x => mapper.Map<ServerDto>(x))), TaskContinuationOptions.OnlyOnRanToCompletion);
        }

        [HttpGet]
        public ActionResult<ServerDto> Get(CancellationToken cancellationToken, int serverId)
        {
            var serverRepo = unitOfWork.GetRepository<Server>();
            var server = serverRepo
              .GetFirstOrDefault(
                  predicate: x => x.ServerId == serverId,
                  disableTracking: true);

            return server != null ? Ok(mapper.Map<ServerDto>(server)) : NotFound();
        }


        [HttpPost]
        public ActionResult<ServerDto> Insert(CancellationToken cancellationToken, ServerDto server)
        {
            if (server is null)
            {
                throw new ArgumentNullException(nameof(server));
            }

            var serverRepo = unitOfWork.GetRepository<Server>();
            var entity = mapper.Map<Server>(server);
            serverRepo.InsertAsync(entity, cancellationToken);
            unitOfWork.SaveChanges();
            return entity != null ? Ok(mapper.Map<ServerDto>(entity)) : NotFound();
        }

        [HttpPut]
        public ActionResult<ServerDto> Update(CancellationToken cancellationToken, ServerDto server)
        {
            if (server is null)
            {
                throw new ArgumentNullException(nameof(server));
            }

            var serverRepo = unitOfWork.GetRepository<Server>();
            var entity = serverRepo.GetFirstOrDefault(
                    predicate: x => x.ServerId == server.ServerId,
                    disableTracking: false);

            if (entity == null)
                return NotFound();

            entity.UpdateValueFrom(server);

            serverRepo.Update(entity);
            unitOfWork.SaveChanges();
            return entity != null ? Ok(mapper.Map<ServerDto>(entity)) : NotFound();
        }

    }
}

