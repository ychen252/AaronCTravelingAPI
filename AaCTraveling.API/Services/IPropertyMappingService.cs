using System.Collections.Generic;

namespace AaCTraveling.API.Services
{
    public interface IPropertyMappingService
    {
        Dictionary<string, PropertyMappingValue> GetPropertyMapping<TSource, TDestination>();
        bool AreMappingPropertiesExisting<TSource, TDestination>(string fields);
        bool ArePropertiesExisting<T>(string fields);
    }
}