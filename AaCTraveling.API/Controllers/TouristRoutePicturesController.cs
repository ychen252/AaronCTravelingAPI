using AaCTraveling.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AaCTraveling.API.Dtos;
using AaCTraveling.API.Models;

namespace AaCTraveling.API.Controllers
{
    [Route("api/touristroutes/{touristRouteId}/pictures")]
    [ApiController]
    public class TouristRoutePicturesController : ControllerBase
    {
        private readonly ITouristRouteRepository _touristRouteRepository;
        private IMapper _mapper;

        public TouristRoutePicturesController(ITouristRouteRepository touristRouteRepository, IMapper mapper)
        {
            _touristRouteRepository = touristRouteRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetPictureListForTouristRoute(Guid touristRouteId)
        {
            if (!_touristRouteRepository.TouristRouteExists(touristRouteId))
            {
                return NotFound($"route {touristRouteId} was not found.");
            }
            var touristRoutePictures = _touristRouteRepository.GetPicturesByTouristRouteId(touristRouteId);
            if (touristRoutePictures == null || touristRoutePictures.Count() == 0)
            {
                return NotFound("No pictures found.");
            }
            var touristRoutePicturesDto = _mapper.Map<IEnumerable<TouristRoutePictureDto>>(touristRoutePictures);
            return Ok(touristRoutePicturesDto);
        }

        [HttpGet("{pictureId}", Name = "GetPicture")]
        public IActionResult GetPicture(Guid touristRouteId, int pictureId)
        {
            if (!_touristRouteRepository.TouristRouteExists(touristRouteId))
            {
                return NotFound($"route {touristRouteId} was not found.");
            }
            var picture = _touristRouteRepository.GetPicture(pictureId);
            if(picture == null)
            {
                return NotFound("No pictures found.");
            }
            return Ok(_mapper.Map<TouristRoutePictureDto>(picture));
        }

        [HttpPost]
        public IActionResult CreateTouristRoutePicture([FromRoute] Guid touristRouteId, 
            [FromBody] TouristRoutePictureForCreationDto touristRoutePictureForCreationDto)
        {
            if (!_touristRouteRepository.TouristRouteExists(touristRouteId))
            {
                return NotFound($"route {touristRouteId} was not found.");
            }
            var pictureModel = _mapper.Map<TouristRoutePicture>(touristRoutePictureForCreationDto);
            _touristRouteRepository.AddTouristRoutePicture(touristRouteId, pictureModel);
            if (_touristRouteRepository.Save())
            {
                var touristRoutePictureDtoToReturn = _mapper.Map<TouristRoutePictureDto>(pictureModel);
                return CreatedAtRoute("GetTouristRouteById", new
                {
                    touristRouteId = touristRoutePictureDtoToReturn.TouristRouteId,
                }, touristRoutePictureDtoToReturn);
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
