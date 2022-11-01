using System;

namespace AaCTraveling.API.Dtos
{
    public class TouristRouteSlimDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }
    }
}
