using AaCTraveling.API.Models;
using System.Collections.Generic;
using System;

namespace AaCTraveling.API.Dtos
{
    public class ShoppingCartDto
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public ICollection<LineItemDto> ShoppingCartItems { get; set; }
    }
}
