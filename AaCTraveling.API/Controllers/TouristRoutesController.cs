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
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace AaCTraveling.API.Controllers
{
    [Route("api/touristroutes")]
    [ApiController]
    public class TouristRoutesController : ControllerBase
    {
        private readonly ITouristRouteRepository _touristRouteRepository;
        private readonly IMapper _mapper;
        //private readonly IUrlHelper _urlHelper;

        public TouristRoutesController(ITouristRouteRepository touristRouteRepository, IMapper mapper)
        {
            _touristRouteRepository = touristRouteRepository;
            _mapper = mapper;
            //_urlHelper = urlHelperFactory.GetUrlHelper(actionContextAccessor.ActionContext);
        }

        private string GenerateTouristRouteResourceUrl(TouristRouteResourceParameters parameters,
             PaginationResourceParameters paginationResourceParameters,
             ResourceUriType type)
        {
            return type switch
            {
                ResourceUriType.PreviousPage => Url.Link("GetTouristRoutes",
                    new
                    {
                        keyword = parameters.Keyword,
                        rating = parameters.Rating,
                        pageNumber = paginationResourceParameters.PageNumber - 1,
                        pageSize = paginationResourceParameters.PageSize
                    }),
                ResourceUriType.NextPage => Url.Link("GetTouristRoutes",
                    new
                    {
                        keyword = parameters.Keyword,
                        rating = parameters.Rating,
                        pageNumber = paginationResourceParameters.PageNumber + 1,
                        pageSize = paginationResourceParameters.PageSize
                    }),
                _ => Url.Link("GetTouristRoutes",
                    new
                    {
                        keyword = parameters.Keyword,
                        rating = parameters.Rating,
                        pageNumber = paginationResourceParameters.PageNumber,
                        pageSize = paginationResourceParameters.PageSize
                    }),
            };
        }

        //api/touristroutes?keyword={keyword}
        [HttpGet(Name = "GetTouristRoutes")]
        [HttpHead]
        public async Task<IActionResult> GetTouristRoutes([FromQuery] TouristRouteResourceParameters parameters,
            [FromQuery] PaginationResourceParameters paginationResourceParameters)
        {

            var routesFromRepo = await _touristRouteRepository.GetTouristRoutesAsync(parameters.Keyword,
                parameters.RatingOperatorType,
                parameters.RatingValue,
                paginationResourceParameters.PageSize,
                paginationResourceParameters.PageNumber);

            if (routesFromRepo == null || routesFromRepo.Count() == 0)
            {
                return NotFound("No Tourist Routes Available.");
            }
            
            var touristRoutesDto = _mapper.Map<IEnumerable<TouristRouteDto>>(routesFromRepo);


            //test
            var previousPageLink = routesFromRepo.HasPrevious ? 
                GenerateTouristRouteResourceUrl(parameters, paginationResourceParameters, ResourceUriType.PreviousPage) : null;
            var nextPageLink = routesFromRepo.HasNext ?
                GenerateTouristRouteResourceUrl(parameters, paginationResourceParameters, ResourceUriType.NextPage) : null;

            //x-pagination
            var paginationMetaData = new
            {
                previousPageLink,
                nextPageLink,
                totalCount = routesFromRepo.TotalCount,
                pageSize = routesFromRepo.PageSize,
                currentPage = routesFromRepo.CurrentPage,
                totalPages = routesFromRepo.TotalPages
            };

            Response.Headers.Add("x-pagination", Newtonsoft.Json.JsonConvert.SerializeObject(paginationMetaData));

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
        //[Authorize(AuthenticationSchemes = "Bearer")]
        [Authorize]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateTouristRoute([FromBody] TouristRouteForCreationDto touristRouteForCreationDto)
        {
            var touristRouteModel = _mapper.Map<TouristRoute>(touristRouteForCreationDto);
            await _touristRouteRepository.AddTouristRouteAsync(touristRouteModel);
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
        [Authorize]
        [Authorize(Roles = "Admin")]
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
        [Authorize]
        [Authorize(Roles = "Admin")]
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
        [Authorize]
        [Authorize(Roles = "Admin")]
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
        [Authorize]
        [Authorize(Roles = "Admin")]
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
