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
    public class RecordSlotController : ControllerBase
    {


        private readonly ILogger<RecordSlotController> _logger;
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public RecordSlotController(ILogger<RecordSlotController> logger, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _logger = logger;
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        [HttpGet]
        [Route("all")]
        public Task<IPagedList<RecordSlotDto>> Get(CancellationToken cancellationToken, int pageIndex = 0, int pageSize = 100)
        {
            var slotRepo = unitOfWork.GetRepository<RecordSlot>();
            return slotRepo
              .GetPagedListAsync(
                  orderBy: x => x.OrderBy(s => s.SlotId),
                  include: slot => slot.Include(x => x.Server),
                  pageIndex: pageIndex,
                  pageSize: pageSize,
                  disableTracking: true,
                  cancellationToken: cancellationToken)
              .ContinueWith(t => PagedList.From(t.Result, source => source.Select(x => mapper.Map<RecordSlotDto>(x))), TaskContinuationOptions.OnlyOnRanToCompletion);
        }

        [HttpGet]
        public ActionResult<RecordSlotDto> Get(CancellationToken cancellationToken, int slotId)
        {
            var slotRepo = unitOfWork.GetRepository<RecordSlot>();
            var slot = slotRepo
              .GetFirstOrDefault(
                  predicate: x => x.SlotId == slotId,
                  include: source => source.Include(x => x.Server),
                  disableTracking: true);

            return slot != null ? Ok(mapper.Map<RecordSlotDto>(slot)) : NotFound();
        }


        [HttpPost]
        public ActionResult<RecordSlotDto> Insert(CancellationToken cancellationToken, RecordSlotDto slot)
        {
            if (slot is null)
            {
                throw new ArgumentNullException(nameof(slot));
            }

            var slotRepo = unitOfWork.GetRepository<RecordSlot>();
            var entity = mapper.Map<RecordSlot>(slot);     
            slotRepo.InsertAsync(entity, cancellationToken);
            unitOfWork.SaveChanges();
            return entity != null ? Ok(mapper.Map<RecordSlotDto>(entity)) : NotFound();
        }

        [HttpPut]
        public ActionResult<RecordSlotDto> Update(CancellationToken cancellationToken, RecordSlotDto slot)
        {
            if (slot is null)
            {
                throw new ArgumentNullException(nameof(slot));
            }

            var slotRepo = unitOfWork.GetRepository<RecordSlot>();
            var entity = slotRepo.GetFirstOrDefault(
                    predicate: x => x.SlotId == slot.SlotId,
                    include: source => source.Include(x => x.Server),
                    disableTracking: false);

            if (entity == null)
                return NotFound();

            entity.UpdateValueFrom(slot);
            slotRepo.Update(entity);
            unitOfWork.SaveChanges();

            return entity != null ? Ok(mapper.Map<RecordSlotDto>(entity)) : NotFound();
        }

    }
}

