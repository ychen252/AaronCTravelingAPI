using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AaCTraveling.API.Models;
using AaCTraveling.API.Dtos;
using AutoMapper;

namespace AaCTraveling.API.Profiles
{
    public class ShoppingCartProfile : Profile
    {
        public ShoppingCartProfile()
        {
            CreateMap<ShoppingCart, ShoppingCartDto>();
            CreateMap<LineItem, LineItemDto>();

            //CreateMap<TouristRouteForUpdateDto, TouristRoute>();
            //CreateMap<TouristRoute, TouristRouteForUpdateDto>();
        }
    }
}
