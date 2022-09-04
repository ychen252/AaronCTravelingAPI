using AaCTraveling.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AaCTraveling.API.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class TouristRoutesController : ControllerBase {
        private ITouristRouteRepository _touristRouteRepository;
        public TouristRoutesController(ITouristRouteRepository touristRouteRepository) {
            _touristRouteRepository = touristRouteRepository;
        }

        [HttpGet]
        public IActionResult GetTouristRoutes() {
            var routesFromRepo = _touristRouteRepository.GetTouristRoutes();
            if(routesFromRepo == null || routesFromRepo.Count() <= 0) {
                return NotFound("No Tourist Routes Available.");
            }
            return Ok(routesFromRepo);
        }

        //api/touristroutes/{touristRouteId}
        [HttpGet("{touristRouteId}")]
        public IActionResult GetTouristRouteById(Guid touristRouteId) {
            var touristRouteFromRepo = _touristRouteRepository.GetTouristRoute(touristRouteId);
            if(touristRouteFromRepo == null) {
                return NotFound($"Route {touristRouteId} Not Found.");
            }
            return Ok(touristRouteFromRepo);
        }
    }
}
