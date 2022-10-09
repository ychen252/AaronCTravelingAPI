using AaCTraveling.API.Helper;
using AaCTraveling.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AaCTraveling.API.Services
{
    public interface ITouristRouteRepository
    {
        Task AddOrderAsync(Order order);
        Task AddShoppingCartItemAsync(LineItem lineItem);
        Task AddTouristRouteAsync(TouristRoute touristRoute);
        Task AddTouristRoutePictureAsync(Guid touristRouteId, TouristRoutePicture touristRoutePicture);
        Task CreateShoppingCartAsync(ShoppingCart shoppingCart);
        void DeleteShoppingCartItem(LineItem lineItem);
        void DeleteShoppingCartItems(IEnumerable<LineItem> lineItems);
        void DeleteTouristRoute(TouristRoute touristRoute);
        void DeleteTouristRoutePicture(TouristRoutePicture touristRoutePicture);
        void DeleteTouristRoutes(IEnumerable<TouristRoute> touristRoutes);
        Task<Order> GetOrderAsync(Guid orderId);
        Task<PaginationList<Order>> GetOrdersByUserIdAsync(string userId, int pageNumber, int pageSize);
        Task<TouristRoutePicture> GetPictureAsync(int pictureId);
        Task<IEnumerable<TouristRoutePicture>> GetPicturesByTouristRouteIdAsync(Guid touristRouteId);
        Task<ShoppingCart> GetShoppingCartByUserIdAsync(string userId);
        Task<LineItem> GetShoppingCartItemByItemIdAsync(int lineItemId);
        Task<IEnumerable<LineItem>> GetShoppingCartItemsByItemIdsAsync(IEnumerable<int> lineItemIds);
        Task<TouristRoute> GetTouristRouteAsync(Guid touristRouteId);
        Task<PaginationList<TouristRoute>> GetTouristRoutesAsync(string keyword, string operationType, int? ratingValue, 
            int pageSize, int PageNumber);
        Task<IEnumerable<TouristRoute>> GetTouristRoutesByIdsAsync(IEnumerable<Guid> touristRouteIds);
        Task<bool> SaveAsync();
        Task<bool> TouristRouteExistsAsync(Guid touristRouteId);
    }
}
