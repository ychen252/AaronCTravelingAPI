using AaCTraveling.API.Dtos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AaCTraveling.API.ValidationAttributes
{
    public class TouristRouteTitleMustBeDifferentFromDescriptionAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var touristRouteDtoForManipulation = (TouristRouteForManipulationDto) validationContext.ObjectInstance;
            if (touristRouteDtoForManipulation.Title == touristRouteDtoForManipulation.Description)
            {
                return new ValidationResult("Tile must be different from Description.",
                    new[] { "TouristRouteForCreationDto" });
            }
            return ValidationResult.Success;
        }
    }
}
