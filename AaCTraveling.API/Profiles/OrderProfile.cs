using AaCTraveling.API.Dtos;
using AaCTraveling.API.Models;
using AutoMapper;

namespace AaCTraveling.API.Profiles
{
    public class OrderProfile : Profile
    {
        public OrderProfile()
        {
            CreateMap<Order, OrderDto>().
                ForMember(
                    dest => dest.State,
                    opt => opt.MapFrom(src => src.State.ToString())
                );
        }
    }
}
