using System.Collections.Generic;

namespace AaCTraveling.API.Services
{
    public interface IPropertyMappingService1
    {
        bool areMappingPropertiesExisting<TSource, TDestination>(string fields);
        Dictionary<string, PropertyMappingValue> GetPropertyMapping<TSource, TDestination>();
    }
}