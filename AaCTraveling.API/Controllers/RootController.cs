using AaCTraveling.API.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace AaCTraveling.API.Controllers
{
    [Route("api")]
    [ApiController]
    public class RootController : ControllerBase
    {
        [HttpGet(Name = "GetRoot")]
        public IActionResult GetRoot()
        {
            var links = new List<LinkDto>();
            links.Add(
                new LinkDto(Url.Link("GetRoot", null),
                "self",
                "GET"));
            links.Add(
                new LinkDto(Url.Link("GetTouristRoutes", null),
                "touristRoutes",
                "GET"));
            links.Add(
                new LinkDto(Url.Link("CreateTouristRoute", null),
                "createTouristRoute",
                "POST"));
            //get shopping cart
            links.Add(
                new LinkDto(Url.Link("GetShoppingCart", null),
                "shoppingCart",
                "GET"));
            //get orders
            links.Add(
                new LinkDto(Url.Link("GetOrders", null),
                "orders",
                "GET"));


            return Ok(links);
        }
    }
}
