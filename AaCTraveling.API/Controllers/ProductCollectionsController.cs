using Microsoft.AspNetCore.Mvc;

namespace AaCTraveling.API.Controllers
{
    [Route("api/productCollections")]
    [ApiController]
    public class ProductCollectionsController : ControllerBase
    {
        public IActionResult GetRecomendations()
        {
            return Ok();
        }
    }
}
