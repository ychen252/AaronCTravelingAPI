using AaCTraveling.API.Dtos;
using AaCTraveling.API.Services;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AaCTraveling.API.Controllers
{
    [Route("api/productCollections")]
    [ApiController]
    public class ProductCollectionsController : ControllerBase
    {
        private readonly ITouristRouteRepository _touristRouteRepository;
        private readonly IMapper _mapper;

        //ctor
        public ProductCollectionsController(ITouristRouteRepository touristRouteRepository, IMapper mapper)
        {
            _touristRouteRepository = touristRouteRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetRecomendations()
        {
            var touristRoutes = await _touristRouteRepository.GetTouristRoutesAsync("", "", null, 30, 1, "rating desc");
            var touristRouteDtos = _mapper.Map<IEnumerable<TouristRouteDto>>(touristRoutes);

            return Ok(new List<ProductCollectionDto>
            {
                new ProductCollectionDto
                {
                    Id = 1,
                    Title = "Recomended for you",
                    Description = "This collection is recomended for you",
                    TouristRoutes = touristRouteDtos.Skip(0).Take(9)
                },
                new ProductCollectionDto
                {
                    Id = 2,
                    Title = "New Product",
                    Description = "This collection of new product",
                    TouristRoutes = touristRouteDtos.Skip(9).Take(9)
                },
                new ProductCollectionDto
                {
                    Id = 3,
                    Title = "Hight Rating",
                    Description = "Hight Rating Collection",
                    TouristRoutes = touristRouteDtos.Skip(18).Take(9)
                },
            });
        }
    }
}
