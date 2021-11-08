using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AaCTraveling.API.Controllers {

    [Route("api/manuallyaddedapi")]
    [ApiController]
    public class ManuallyAddedAPI {
        [HttpGet]
        public IEnumerable<string> Get() {
            return new[] { "THIS IS FROM", "MANUALLY ADDED API" };
        }
    }
}
