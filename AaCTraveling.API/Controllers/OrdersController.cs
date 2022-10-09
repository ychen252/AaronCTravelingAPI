using AaCTraveling.API.Dtos;
using AaCTraveling.API.ResourceParameters;
using AaCTraveling.API.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AaCTraveling.API.Controllers
{
    [Route("api/orders")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly ITouristRouteRepository _touristRouteRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly IHttpClientFactory _httpClientFactory;

        public OrdersController(ITouristRouteRepository touristRouteRepository,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper,
            IHttpClientFactory httpClientFactory)
        {
            _touristRouteRepository = touristRouteRepository;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetOrders([FromQuery] PaginationResourceParameters paginationResourceParameters)
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var orders = await _touristRouteRepository.GetOrdersByUserIdAsync(userId,
                paginationResourceParameters.PageNumber,
                paginationResourceParameters.PageSize);

            return Ok(_mapper.Map<IEnumerable<OrderDto>>(orders));

        }

        [HttpGet("{orderId}")]
        [Authorize]
        public async Task<IActionResult> GetOrderById([FromRoute] Guid orderId)
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var order = await _touristRouteRepository.GetOrderAsync(orderId);

            if(order == null || order.UserId != userId)
            {
                return NotFound("No authorization to access to the order for this user.");
            }

            return Ok(_mapper.Map<OrderDto>(order));

        }

        [HttpPost("{orderId}/placeOrder")]
        [Authorize]
        public async Task<IActionResult> PlaceOrder([FromRoute] Guid orderId)
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var order = await _touristRouteRepository.GetOrderAsync(orderId);

            if (order == null || order.UserId != userId)
            {
                return NotFound("No authorization to access to the order for this user.");
            }

            order.PaymentProcessing();
            await _touristRouteRepository.SaveAsync();

            //fake call service agent to process
            var httpClient = _httpClientFactory.CreateClient();
            //call whatever payment gateway
            //var res = await httpClient.PostAsync(XXXXXXXX);


            bool isApproved = true;
            string transactionMetadata = "id: aaaaaa, approved: true";

            var rand = new Random();
            if(rand.NextDouble() > 0.8)
            {
                isApproved = false;
                transactionMetadata = "id: aaaaaa, approved: false";
            }

            if (isApproved)
            {
                order.PaymentApproved();
            }
            else
            {
                order.PaymentReject();
            }

            order.TransactionMetadata = transactionMetadata;
            await _touristRouteRepository.SaveAsync();

            return Ok(_mapper.Map<OrderDto>(order));

        }
    }
}
