using System.Collections.Generic;

namespace AaCTraveling.API.Dtos
{
    public class ProductCollectionDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public IEnumerable<TouristRouteDto> TouristRoutes { get; set; }
    }
}
