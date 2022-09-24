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
using Microsoft.AspNetCore.JsonPatch;
using AaCTraveling.API.Helper;
using Microsoft.AspNetCore.Authorization;

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
        public async Task<IActionResult> GetTouristRoutes([FromQuery] TouristRouteResourceParameters parameters)
        {

            var routesFromRepo = await _touristRouteRepository.GetTouristRoutesAsync(parameters.Keyword,
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
        public async Task<IActionResult> GetTouristRouteById([FromRoute] Guid touristRouteId)
        {
            var touristRoute = await _touristRouteRepository.GetTouristRouteAsync(touristRouteId);
            if (touristRoute == null)
            {
                return NotFound($"Route {touristRouteId} Not Found.");
            }
            var touristRouteDto = _mapper.Map<TouristRouteDto>(touristRoute);
            return Ok(touristRouteDto);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateTouristRoute([FromBody] TouristRouteForCreationDto touristRouteForCreationDto)
        {
            var touristRouteModel = _mapper.Map<TouristRoute>(touristRouteForCreationDto);
            _touristRouteRepository.AddTouristRoute(touristRouteModel);
            if (await _touristRouteRepository.SaveAsync())
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

        [HttpPut("{touristRouteId:Guid}")]
        public async Task<IActionResult> UpdateTouristRoute([FromRoute] Guid touristRouteId,
            [FromBody] TouristRouteForUpdateDto touristRouteForUpdateDto)
        {
            var touristRoute = await _touristRouteRepository.GetTouristRouteAsync(touristRouteId);
            if (touristRoute == null)
            {
                return NotFound($"route {touristRouteId} was not found.");
            }

            _mapper.Map(touristRouteForUpdateDto, touristRoute);
            if (!await _touristRouteRepository.SaveAsync())
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            return NoContent();
        }

        [HttpPatch("{touristRouteId:Guid}")]
        public async Task<IActionResult> PartiallyUpdateTouristRoute([FromRoute] Guid touristRouteId,
            [FromBody] JsonPatchDocument<TouristRouteForUpdateDto> patchDocument)
        {
            var touristRoute = await _touristRouteRepository.GetTouristRouteAsync(touristRouteId);
            if (touristRoute == null)
            {
                return NotFound($"route {touristRouteId} was not found.");
            }
            var touristRouteForPatch = _mapper.Map<TouristRouteForUpdateDto>(touristRoute);
            patchDocument.ApplyTo(touristRouteForPatch, ModelState);
            if (!TryValidateModel(touristRouteForPatch))
            {
                return ValidationProblem(ModelState);
            }

            _mapper.Map(touristRouteForPatch, touristRoute);
            if (! await _touristRouteRepository.SaveAsync())
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            return NoContent();
        }

        [HttpDelete("{touristRouteId:Guid}")]
        public async Task<IActionResult> DeleteTouristRoute([FromRoute] Guid touristRouteId)
        {
            var touristRoute = await _touristRouteRepository.GetTouristRouteAsync(touristRouteId);
            if (touristRoute == null)
            {
                return NotFound($"route {touristRouteId} was not found.");
            }

            _touristRouteRepository.DeleteTouristRoute(touristRoute);
            if (! await _touristRouteRepository.SaveAsync())
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            return NoContent();
        }

        [HttpDelete("({touristRouteIds})")]
        public async Task<IActionResult> DeleteTouristRouteByIds([ModelBinder(BinderType = typeof(ArrayModelBinder))]
        [FromRoute] IEnumerable<Guid> touristRouteIds)
        {
            if (touristRouteIds == null)
            {
                return BadRequest("touristRouteIds is null");
            }
            var touristRoutes = await _touristRouteRepository.GetTouristRoutesByIdsAsync(touristRouteIds);
            _touristRouteRepository.DeleteTouristRoutes(touristRoutes);

            if (! await _touristRouteRepository.SaveAsync())
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            return NoContent();
        }

    }
}
