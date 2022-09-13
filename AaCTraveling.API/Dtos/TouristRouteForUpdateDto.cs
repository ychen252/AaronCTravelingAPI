using AaCTraveling.API.Models;
using AaCTraveling.API.ValidationAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AaCTraveling.API.Dtos
{
    public class TouristRouteForUpdateDto : TouristRouteForManipulationDto
    {
        [Required(ErrorMessage = "Put request must provide description")]
        [MaxLength(1500)]
        public override string Description { get; set; }
    }
}
