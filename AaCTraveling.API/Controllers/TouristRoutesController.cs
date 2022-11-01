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
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Net.Http.Headers;
using System.Dynamic;

namespace AaCTraveling.API.Controllers
{
    [Route("api/touristroutes")]
    [ApiController]
    public class TouristRoutesController : ControllerBase
    {
        private readonly ITouristRouteRepository _touristRouteRepository;
        private readonly IMapper _mapper;
        private readonly IPropertyMappingService _propertyMappingService;
        //private readonly IUrlHelper _urlHelper;

        public TouristRoutesController(ITouristRouteRepository touristRouteRepository,
            IMapper mapper,
            IPropertyMappingService propertyMappingService)
        {
            _touristRouteRepository = touristRouteRepository;
            _mapper = mapper;
            _propertyMappingService = propertyMappingService;
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
                        pageSize = paginationResourceParameters.PageSize,
                        orderBy = parameters.OrderBy,
                        fields = parameters.Fields
                    }),
                ResourceUriType.NextPage => Url.Link("GetTouristRoutes",
                    new
                    {
                        keyword = parameters.Keyword,
                        rating = parameters.Rating,
                        pageNumber = paginationResourceParameters.PageNumber + 1,
                        pageSize = paginationResourceParameters.PageSize,
                        orderBy = parameters.OrderBy,
                        fields = parameters.Fields
                    }),
                _ => Url.Link("GetTouristRoutes",
                    new
                    {
                        keyword = parameters.Keyword,
                        rating = parameters.Rating,
                        pageNumber = paginationResourceParameters.PageNumber,
                        pageSize = paginationResourceParameters.PageSize,
                        orderBy = parameters.OrderBy,
                        fields = parameters.Fields
                    }),
            };
        }

        //api/touristroutes?keyword={keyword}
        [Produces("application/json",
            "application/vnd.aac.hateoas+json",
            "application/vnd.aac.touristroute.simplify+json",
            "application/vnd.aac.touristroute.simplify.hateoas+json")]
        [HttpGet(Name = "GetTouristRoutes")]
        [HttpHead]
        public async Task<IActionResult> GetTouristRoutes([FromQuery] TouristRouteResourceParameters parameters,
            [FromQuery] PaginationResourceParameters paginationResourceParameters,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            if (!MediaTypeHeaderValue.TryParse(mediaType, out var parsedMediaType))
            {
                return BadRequest("Media type is not supported");
            }

            if (!_propertyMappingService.AreMappingPropertiesExisting<TouristRouteDto, TouristRoute>(parameters.OrderBy))
            {
                return BadRequest("Invalid input in the sorting parameters.");
            }

            if (!_propertyMappingService.ArePropertiesExisting<TouristRouteDto>(parameters.Fields))
            {
                return BadRequest("Invalid input in the fields parameters.");
            }

            var routesFromRepo = await _touristRouteRepository.GetTouristRoutesAsync(parameters.Keyword,
                parameters.RatingOperatorType,
                parameters.RatingValue,
                paginationResourceParameters.PageSize,
                paginationResourceParameters.PageNumber,
                parameters.OrderBy);

            if (routesFromRepo == null || routesFromRepo.Count() == 0)
            {
                return NotFound("No Tourist Routes Available.");
            }

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

            bool isHateoas = parsedMediaType.SubTypeWithoutSuffix.EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase);
            var primaryMediaType = isHateoas ? 
                parsedMediaType.SubTypeWithoutSuffix.Substring(0, parsedMediaType.SubTypeWithoutSuffix.Length - 8) 
                : parsedMediaType.SubTypeWithoutSuffix;

            //var touristRoutesDto = _mapper.Map<IEnumerable<TouristRouteDto>>(routesFromRepo);
            //var ShapedDtos = touristRoutesDto.ShapeData(parameters.Fields);

            IEnumerable<object> touristRouteDto;
            IEnumerable<ExpandoObject> shapedDtoList;

            if (primaryMediaType == "vnd.aac.touristroute.simplify")
            {
                touristRouteDto = _mapper.Map<IEnumerable<TouristRouteSlimDto>>(routesFromRepo);
                shapedDtoList = ((IEnumerable<TouristRouteSlimDto>) touristRouteDto).ShapeData(parameters.Fields);
                
            }
            else
            {
                touristRouteDto = _mapper.Map<IEnumerable<TouristRouteDto>>(routesFromRepo);
                shapedDtoList = ((IEnumerable<TouristRouteDto>) touristRouteDto).ShapeData(parameters.Fields);
                
            }

            if (isHateoas)
            {
                var links = CreateLinkForTouristRouteList(parameters, paginationResourceParameters);
                var shapedDtosWithLinks = shapedDtoList.Select(dto =>
                {
                    var dtoAsDictionary = dto as IDictionary<string, object>;
                    var touristRouteLinks = CreateLinksForTouristRoute((Guid) dtoAsDictionary["Id"], parameters.Fields);
                    dtoAsDictionary.Add("links", touristRouteLinks);
                    return dtoAsDictionary;
                });

                var linkedCollectionResource = new
                {
                    value = shapedDtosWithLinks,
                    links
                };

                return Ok(linkedCollectionResource);
            }
            else
            {
                return Ok(shapedDtoList);
            }
        }

        private IEnumerable<LinkDto> CreateLinkForTouristRouteList(TouristRouteResourceParameters parameters,
            PaginationResourceParameters paginationResourceParameters)
        {
            var links = new List<LinkDto>();
            links.Add(new LinkDto(GenerateTouristRouteResourceUrl(parameters, paginationResourceParameters, ResourceUriType.CurrentPage), "self", "GET"));
            links.Add(new LinkDto(GenerateTouristRouteResourceUrl(parameters, paginationResourceParameters, ResourceUriType.NextPage), "nextPage", "GET"));
            links.Add(new LinkDto(GenerateTouristRouteResourceUrl(parameters, paginationResourceParameters, ResourceUriType.PreviousPage), "previousPage", "GET"));
            links.Add(new LinkDto(Url.Link("CreateTouristRoute",null),"create_tourist_route","POST"));

            return links;
        }

        //api/touristroutes/{touristRouteId}
        [HttpGet("{touristRouteId:Guid}", Name = "GetTouristRouteById")]
        [HttpHead("{touristRouteId:Guid}", Name = "GetTouristRouteById")]
        public async Task<IActionResult> GetTouristRouteById([FromRoute] Guid touristRouteId, [FromQuery] string fields)
        {
            if (!_propertyMappingService.ArePropertiesExisting<TouristRouteDto>(fields))
            {
                return BadRequest("Invalid input in the fields parameters.");
            }

            var touristRoute = await _touristRouteRepository.GetTouristRouteAsync(touristRouteId);
            if (touristRoute == null)
            {
                return NotFound($"Route {touristRouteId} Not Found.");
            }
            var touristRouteDto = _mapper.Map<TouristRouteDto>(touristRoute);
            //return Ok(touristRouteDto.ShapeData(fields));
            var linkDtos = CreateLinksForTouristRoute(touristRouteId, fields);

            var result = touristRouteDto.ShapeData(fields) as IDictionary<string, object>;
            result.Add("links", linkDtos);

            return Ok(result);
        }

        private IEnumerable<LinkDto> CreateLinksForTouristRoute(Guid touristRouteId, string fields)
        {
            var links = new List<LinkDto>();
            if (string.IsNullOrWhiteSpace(fields))
            {
                links.Add(new LinkDto(Url.Link("GetTouristRouteById", new { touristRouteId }),
                    "self",
                    "GET"));
            }
            else
            {
                links.Add(new LinkDto(Url.Link("GetTouristRouteById", new { touristRouteId, fields }),
                    "self",
                    "GET"));
            }

            links.Add(new LinkDto(Url.Link("DeleteTouristRoute", new { touristRouteId }),
                "delete_tourist_route",
                "DELETE"));
            links.Add(new LinkDto(Url.Link("UpdateTouristRoute", new { touristRouteId }),
                "update_tourist_route",
                "PUT"));
            links.Add(new LinkDto(Url.Link("PartiallyUpdateTouristRoute", new { touristRouteId }),
                "partially_update_tourist_route",
                "PATCH"));
            links.Add(new LinkDto(Url.Link("CreateTouristRoute", new { }),
                "create_tourist_route",
                "POST"));

            //Tourist Pictures
            links.Add(new LinkDto(Url.Link("GetPictureListForTouristRoute", new { touristRouteId }),
                "get_pictures",
                "GET"));
            links.Add(new LinkDto(Url.Link("CreateTouristRoutePicture", new { touristRouteId }),
                "create_picture",
                "POST"));

            return links;
        }

        [HttpPost(Name = "CreateTouristRoute")]
        [Authorize]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateTouristRoute([FromBody] TouristRouteForCreationDto touristRouteForCreationDto)
        {
            var touristRouteModel = _mapper.Map<TouristRoute>(touristRouteForCreationDto);
            await _touristRouteRepository.AddTouristRouteAsync(touristRouteModel);
            if (await _touristRouteRepository.SaveAsync())
            {
                var touristRouteDtoToReturn = _mapper.Map<TouristRouteDto>(touristRouteModel);
                var links = CreateLinksForTouristRoute(touristRouteDtoToReturn.Id, null);

                var result = touristRouteDtoToReturn.ShapeData(null) as IDictionary<string, object>;

                return CreatedAtRoute("GetTouristRouteById", new
                {
                    touristRouteId = result["Id"],
                }, result);
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPut("{touristRouteId:Guid}", Name = "UpdateTouristRoute")]
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

        [HttpPatch("{touristRouteId:Guid}", Name = "PartiallyUpdateTouristRoute")]
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
            if (!await _touristRouteRepository.SaveAsync())
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            return NoContent();
        }

        [HttpDelete("{touristRouteId:Guid}", Name = "DeleteTouristRoute")]
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
            if (!await _touristRouteRepository.SaveAsync())
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

            if (!await _touristRouteRepository.SaveAsync())
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            return NoContent();
        }

    }
}
