using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Arch.EntityFrameworkCore.UnitOfWork;
using Arch.EntityFrameworkCore.UnitOfWork.Collections;
using AutoMapper;
using Crosscutting.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Stardust.Flux.Contract;
using Stardust.Flux.InputSlotApi.Models.Entity;

namespace InputSlotApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SlotController : ControllerBase
    {


        private readonly ILogger<SlotController> _logger;
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public SlotController(ILogger<SlotController> logger, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _logger = logger;
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        [HttpGet]
        [Route("/all")]
        public Task<IPagedList<SlotDto>> Get(CancellationToken cancellationToken, int pageIndex = 0, int pageSize = 100)
        {
            var slotRepo = unitOfWork.GetRepository<Slot>();
            return slotRepo
              .GetPagedListAsync(
                  orderBy: x => x.OrderBy(s => s.SlotId),
                  include: slot => slot.Include(x => x.AdditionalsData),
                  pageIndex: pageIndex,
                  pageSize: pageSize,
                  disableTracking: true,
                  cancellationToken: cancellationToken)
              .ContinueWith(t => PagedList.From(t.Result, source => source.Select(x => mapper.Map<SlotDto>(x))), TaskContinuationOptions.OnlyOnRanToCompletion);
        }

        [HttpGet]
        public ActionResult<SlotDto> Get(CancellationToken cancellationToken, int slotId)
        {
            var slotRepo = unitOfWork.GetRepository<Slot>();
            var slot = slotRepo
              .GetFirstOrDefault(
                  predicate: x => x.SlotId == slotId,
                  include: source => source.Include(x => x.AdditionalsData),
                  disableTracking: true);

            return slot != null ? Ok(mapper.Map<SlotDto>(slot)) : NotFound();
        }


        [HttpPost]
        public ActionResult<SlotDto> Insert(CancellationToken cancellationToken, SlotDto slot)
        {
            if (slot is null)
            {
                throw new ArgumentNullException(nameof(slot));
            }

            var slotRepo = unitOfWork.GetRepository<Slot>();
            var entity = mapper.Map<Slot>(slot);
            var addtionalsData = slot?.AdditionalsData.Select(x => mapper.Map<AdditionalSlotData>(x)) ?? Enumerable.Empty<AdditionalSlotData>();
            foreach (var additional in addtionalsData)
            {
                entity.AdditionalsData.Add(additional);
            }
            slotRepo.InsertAsync(entity, cancellationToken);
            unitOfWork.SaveChanges();
            return entity != null ? Ok(mapper.Map<SlotDto>(entity)) : NotFound();
        }

        [HttpPut]
        public ActionResult<SlotDto> Update(CancellationToken cancellationToken, SlotDto slot)
        {
            if (slot is null)
            {
                throw new ArgumentNullException(nameof(slot));
            }



            var slotRepo = unitOfWork.GetRepository<Slot>();
            var entity = slotRepo.GetFirstOrDefault(
                    predicate: x => x.SlotId == slot.SlotId,
                    include: source => source.Include(x => x.AdditionalsData),
                    disableTracking: false);

            if (entity == null)
                return NotFound();

            var addtionalsData = slot?.AdditionalsData ?? new Dictionary<string, string>();
            entity.AdditionalsData.MergeChildren(
                addtionalsData, (src, dst) => string.Equals(src.Key, dst.Key, StringComparison.OrdinalIgnoreCase),
                (src) => new AdditionalSlotData { Key = src.Key, Value = src.Value },
                (src, dst) => dst.Value = src.Value);

            slotRepo.Update(entity);
            unitOfWork.SaveChanges();
            return entity != null ? Ok(mapper.Map<SlotDto>(entity)) : NotFound();
        }

    }
}

