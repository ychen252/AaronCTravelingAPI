using AaCTraveling.API.Database;
using AaCTraveling.API.Helper;
using AaCTraveling.API.Models;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;


namespace AaCTraveling.API.Services
{
    public class TouristRouteRepository : ITouristRouteRepository
    {
        private readonly AppDbContext _context;
        public TouristRouteRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddOrderAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
        }

        public async Task AddShoppingCartItemAsync(LineItem lineItem)
        {
            await _context.LineItems.AddAsync(lineItem);
        }

        public async Task AddTouristRouteAsync(TouristRoute touristRoute)
        {
            if(touristRoute == null)
            {
                throw new ArgumentNullException(nameof(touristRoute));
            }
            
            await _context.TouristRoutes.AddAsync(touristRoute);
        }

        public async Task AddTouristRoutePictureAsync(Guid touristRouteId, TouristRoutePicture touristRoutePicture)
        {
            if(touristRouteId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(touristRouteId));
            }
            
            if(touristRoutePicture == null)
            {
                throw new ArgumentNullException(nameof(touristRoutePicture));
            }
            
            touristRoutePicture.TouristRouteId = touristRouteId;
            await _context.TouristRoutePictures.AddAsync(touristRoutePicture);
        }

        public async Task CreateShoppingCartAsync(ShoppingCart shoppingCart)
        {
            await _context.ShoppingCarts.AddAsync(shoppingCart);
        }

        public void DeleteShoppingCartItem(LineItem lineItem)
        {
            _context.LineItems.Remove(lineItem);
        }

        public void DeleteShoppingCartItems(IEnumerable<LineItem> lineItems)
        {
            _context.LineItems.RemoveRange(lineItems);
        }

        public void DeleteTouristRoute(TouristRoute touristRoute)
        {
            _context.TouristRoutes.Remove(touristRoute);
        }

        public void DeleteTouristRoutePicture(TouristRoutePicture touristRoutePicture)
        {
            _context.TouristRoutePictures.Remove(touristRoutePicture);
        }

        public void DeleteTouristRoutes(IEnumerable<TouristRoute> touristRoutes)
        {
            _context.TouristRoutes.RemoveRange(touristRoutes);
        }

        public async Task<Order> GetOrderAsync(Guid orderId)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.TouristRoute)
                .FirstOrDefaultAsync(o => o.Id == orderId);
        }

        public async Task<PaginationList<Order>> GetOrdersByUserIdAsync(string userId, int pageNumber, int PageSize)
        {
            var orders = _context.Orders.Where(o => o.UserId == userId);
            return await PaginationList<Order>.CreateAsync(orders, pageNumber, PageSize);
        }
        
        public async Task<TouristRoutePicture> GetPictureAsync(int pictureId)
        {
            return await _context.TouristRoutePictures.FirstOrDefaultAsync(tp => tp.Id == pictureId);
        }

        public async Task<IEnumerable<TouristRoutePicture>> GetPicturesByTouristRouteIdAsync(Guid touristRouteId)
        {
            return await _context.TouristRoutePictures.Where(tp => tp.TouristRouteId == touristRouteId).ToListAsync();
        }

        public async Task<ShoppingCart> GetShoppingCartByUserIdAsync(string userId)
        {
            return await _context.ShoppingCarts
                .Include(s => s.User)
                .Include(s => s.ShoppingCartItems)
                .ThenInclude(lineItem => lineItem.TouristRoute)
                .FirstOrDefaultAsync(s => s.UserId == userId);
        }

        public async Task<LineItem> GetShoppingCartItemByItemIdAsync(int lineItemId)
        {
            return await _context.LineItems.FirstOrDefaultAsync(li => li.Id == lineItemId);
        }

        public async Task<IEnumerable<LineItem>> GetShoppingCartItemsByItemIdsAsync(IEnumerable<int> lineItemIds)
        {
            return await _context.LineItems.Where(li => lineItemIds.Contains(li.Id)).ToListAsync();
        }

        public async Task<TouristRoute> GetTouristRouteAsync(Guid touristRouteId)
        {
            return await _context.TouristRoutes.Include(t => t.TouristRoutePictures).FirstOrDefaultAsync(n => n.Id == touristRouteId);
        }

        public async Task<PaginationList<TouristRoute>> GetTouristRoutesAsync(string keyword, string operationType,
            int? ratingValue, int pageSize, int pageNumber)
        {
            IQueryable<TouristRoute> result = _context.TouristRoutes
                .Include(t => t.TouristRoutePictures);
            
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim();
                result = result.Where(t => t.Title.Contains(keyword));
            }
            
            if (ratingValue!= null && ratingValue >= 0)
            {
                result = operationType.ToLower() switch
                {
                    "largerthan" => result.Where(t => t.Rating >= ratingValue),
                    "lessthan" => result.Where(t => t.Rating <= ratingValue),
                    _ => result.Where(t => t.Rating == ratingValue),
                };
            }

            //pagination
            return await PaginationList<TouristRoute>.CreateAsync(result, pageNumber, pageSize);
        }

        public async Task<IEnumerable<TouristRoute>> GetTouristRoutesByIdsAsync(IEnumerable<Guid> touristRouteIds)
        {
            return await _context.TouristRoutes.Where(tr => touristRouteIds.Contains(tr.Id)).ToListAsync();
        }

        public async Task<bool> SaveAsync()
        {
            return await _context.SaveChangesAsync() >= 0;
        }

        public async Task<bool> TouristRouteExistsAsync(Guid touristRouteId)
        {
            return await _context.TouristRoutes.AnyAsync(t => t.Id == touristRouteId);
        }
    }
}
