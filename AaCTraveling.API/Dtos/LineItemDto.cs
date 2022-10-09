using AaCTraveling.API.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace AaCTraveling.API.Dtos
{
    public class LineItemDto
    {
        public int Id { get; set; }
        public Guid TouristRouteId { get; set; }
        public TouristRouteDto TouristRoute { get; set; }
        public Guid? ShoppingCartId { get; set; }
        //public Guid? OrderId { get; set; }
        public decimal OriginalPrice { get; set; }
        public double? DiscountPercent { get; set; }
    }
}