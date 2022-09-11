using AaCTraveling.API.Dtos;
using AaCTraveling.API.Models;
using AaCTraveling.API.ResourceParameters;
using AaCTraveling.API.Services;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AaCTraveling.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TouristRoutesController : ControllerBase
    {
        private readonly ITouristRouteRepository _touristRouteRepository;
        private readonly IMapper _mapper;

        public TouristRoutesController(ITouristRouteRepository touristRouteRepository, IMapper mapper)
        {
            _touristRouteRepository = touristRouteRepository;
            _mapper = mapper;
        }

        //api/touristroutes?keyword={keyword}
        [HttpGet]
        [HttpHead]
        public IActionResult GetTouristRoutes([FromQuery] TouristRouteResourceParameters parameters)
        {

            var routesFromRepo = _touristRouteRepository.GetTouristRoutes(parameters.Keyword,
                parameters.RatingOperatorType,
                parameters.RatingValue);
            if (routesFromRepo == null || routesFromRepo.Count() == 0)
            {
                return NotFound("No Tourist Routes Available.");
            }
            var touristRoutesDto = _mapper.Map<IEnumerable<TouristRouteDto>>(routesFromRepo);
            return Ok(touristRoutesDto);
        }

        //api/touristroutes/{touristRouteId}
        [HttpGet("{touristRouteId:Guid}", Name = "GetTouristRouteById")]
        [HttpHead("{touristRouteId:Guid}", Name = "GetTouristRouteById")]
        public IActionResult GetTouristRouteById([FromRoute] Guid touristRouteId)
        {
            var touristRoute = _touristRouteRepository.GetTouristRoute(touristRouteId);
            if (touristRoute == null)
            {
                return NotFound($"Route {touristRouteId} Not Found.");
            }
            var touristRouteDto = _mapper.Map<TouristRouteDto>(touristRoute);
            return Ok(touristRouteDto);
        }

        [HttpPost]
        public IActionResult CreateTouristRoute([FromBody] TouristRouteForCreationDto touristRouteForCreationDto)
        {
            var touristRouteModel = _mapper.Map<TouristRoute>(touristRouteForCreationDto);
            _touristRouteRepository.AddTouristRoute(touristRouteModel);
            if (_touristRouteRepository.Save())
            {
                var touristRouteDtoToReturn = _mapper.Map<TouristRouteDto>(touristRouteModel);
                return CreatedAtRoute("GetTouristRouteById", new
                {
                    touristRouteId = touristRouteDtoToReturn.Id,
                }, touristRouteDtoToReturn);
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
