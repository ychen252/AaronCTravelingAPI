using AaCTraveling.API.Dtos;
using AaCTraveling.API.Helper;
using AaCTraveling.API.Models;
using AaCTraveling.API.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AaCTraveling.API.Controllers
{
    [ApiController]
    [Route("api/shoppingCart")]
    public class ShoppingCartController : ControllerBase
    {

        //private readonly IShoppingCartRepository _shoppingCartRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITouristRouteRepository _touristRouteRepository;
        private readonly IMapper _mapper;
        
        //constructor
        public ShoppingCartController(IHttpContextAccessor httpContextAccessor,
            ITouristRouteRepository touristRouteRepository,
            IMapper mapper)
        {
            _httpContextAccessor = httpContextAccessor;
            _touristRouteRepository = touristRouteRepository;
            _mapper = mapper;
        }
        
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetShoppingCart()
        {
            //get user
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            //use userId to get shopping cart
            var shoppingCart = await _touristRouteRepository.GetShoppingCartByUserIdAsync(userId);
            
            return Ok(_mapper.Map<ShoppingCartDto>(shoppingCart));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddShoppingCartItem([FromBody] AddShoppingCartItemDto 
            addShoppingCartItemDto)
        {
            //get user
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            //use userId to get shopping cart
            var shoppingCart = await _touristRouteRepository.GetShoppingCartByUserIdAsync(userId);

            //add lineItem
            var touristRoute = await _touristRouteRepository.GetTouristRouteAsync(addShoppingCartItemDto.TouristRouteId);
            
            if(touristRoute == null)
            {
                return NotFound($"Route {addShoppingCartItemDto.TouristRouteId} Not Found.");
            }

            var lineItem = new LineItem()
            {
                ShoppingCartId = shoppingCart.Id,
                TouristRouteId = touristRoute.Id,
                //TouristRoute = touristRoute,
                OriginalPrice = touristRoute.OriginalPrice,
                DiscountPercent = touristRoute.DiscountPercent
            };

            await _touristRouteRepository.AddShoppingCartItemAsync(lineItem);
            await _touristRouteRepository.SaveAsync();

            return Ok(_mapper.Map<ShoppingCartDto>(shoppingCart));
            
        }

        [HttpDelete("items/{itemId}")]
        [Authorize]
        public async Task<IActionResult> DeleteShoppingCartItem([FromRoute] int itemId)
        {
            var lineItem = await _touristRouteRepository.GetShoppingCartItemByItemIdAsync(itemId);

            if (lineItem == null)
            {
                return NotFound("This shopping cart item not found");
            }

            _touristRouteRepository.DeleteShoppingCartItem(lineItem);
            await _touristRouteRepository.SaveAsync();

            return NoContent();
        }

        [HttpDelete("items/({itemIds})")]
        [Authorize]
        public async Task<IActionResult> DeleteShoppingCartItems([ModelBinder(BinderType = typeof(ArrayModelBinder))] 
        [FromRoute] IEnumerable<int> itemIds)
        {
            var lineItems = await _touristRouteRepository.GetShoppingCartItemsByItemIdsAsync(itemIds);

            if (lineItems == null)
            {
                return NotFound("These shopping cart items not found");
            }

            _touristRouteRepository.DeleteShoppingCartItems(lineItems);
            await _touristRouteRepository.SaveAsync();

            return NoContent();
        }

        [HttpPost("checkout")]
        [Authorize]
        public async Task<IActionResult> Checkout()
        {
            //get user
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            //use userId to get shopping cart
            var shoppingCart = await _touristRouteRepository.GetShoppingCartByUserIdAsync(userId);

            //create order
            var order = new Order()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                State = OrderStateEnum.Pending,
                CreateDateUTC = DateTime.UtcNow,
                OrderItems = shoppingCart.ShoppingCartItems
            };

            shoppingCart.ShoppingCartItems = null;

            await _touristRouteRepository.AddOrderAsync(order);
            await _touristRouteRepository.SaveAsync();

            return Ok(_mapper.Map<OrderDto>(order));
        }

    }
}
