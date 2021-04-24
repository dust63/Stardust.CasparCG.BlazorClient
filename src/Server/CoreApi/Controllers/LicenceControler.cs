using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stardust.Flux.CoreApi.Controllers
{
    public class LicenceControler : Controller
    {

        private LicenceConfig LicenceConfig { get; set; }
        public LicenceControler(IOptions<LicenceConfig> licenceConfig )
        {

            LicenceConfig = licenceConfig.Value;
        }
       
        [HttpGet("/syncfusionlicence")]
        public IActionResult GetSyncFusionLicence()
        {
            return Ok(LicenceConfig.SyncFusionLicence);
        }
    }
}
