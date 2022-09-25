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
using Microsoft.AspNetCore.Authorization;

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
        public async Task<IActionResult> GetPictureListForTouristRoute(Guid touristRouteId)
        {
            if (!await _touristRouteRepository.TouristRouteExistsAsync(touristRouteId))
            {
                return NotFound($"route {touristRouteId} was not found.");
            }
            var touristRoutePictures = await _touristRouteRepository.GetPicturesByTouristRouteIdAsync(touristRouteId);
            if (touristRoutePictures == null || touristRoutePictures.Count() == 0)
            {
                return NotFound("No pictures found.");
            }
            var touristRoutePicturesDto = _mapper.Map<IEnumerable<TouristRoutePictureDto>>(touristRoutePictures);
            return Ok(touristRoutePicturesDto);
        }

        [HttpGet("{pictureId}", Name = "GetPicture")]
        public async Task<IActionResult> GetPicture([FromRoute] Guid touristRouteId, [FromRoute] int pictureId)
        {
            if (!await _touristRouteRepository.TouristRouteExistsAsync(touristRouteId))
            {
                return NotFound($"route {touristRouteId} was not found.");
            }
            var picture = await _touristRouteRepository.GetPictureAsync(pictureId);
            if (picture == null)
            {
                return NotFound("No pictures found.");
            }
            return Ok(_mapper.Map<TouristRoutePictureDto>(picture));
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateTouristRoutePicture([FromRoute] Guid touristRouteId,
            [FromBody] TouristRoutePictureForCreationDto touristRoutePictureForCreationDto)
        {
            if (!await _touristRouteRepository.TouristRouteExistsAsync(touristRouteId))
            {
                return NotFound($"route {touristRouteId} was not found.");
            }
            var pictureModel = _mapper.Map<TouristRoutePicture>(touristRoutePictureForCreationDto);
            _touristRouteRepository.AddTouristRoutePicture(touristRouteId, pictureModel);
            if (await _touristRouteRepository.SaveAsync())
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

        [HttpDelete("{pictureId}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteTouristRoutePicture([FromRoute] Guid touristRouteId,
            [FromRoute] int pictureId)
        {
            if (!await _touristRouteRepository.TouristRouteExistsAsync(touristRouteId))
            {
                return NotFound($"route {touristRouteId} was not found.");
            }
            var picture = await _touristRouteRepository.GetPictureAsync(pictureId);
            _touristRouteRepository.DeleteTouristRoutePicture(picture);

            if (!await _touristRouteRepository.SaveAsync())
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            return NoContent();
        }
    }
}
