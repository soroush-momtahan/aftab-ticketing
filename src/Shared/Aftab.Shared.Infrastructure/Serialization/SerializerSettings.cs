using Newtonsoft.Json;

namespace Aftab.Shared.Infrastructure.Serialization;

public static class SerializerSettings
{
    public static readonly JsonSerializerSettings Instance = new()
    {        TypeNameHandling = TypeNameHandling.All,  
        MetadataPropertyHandling = MetadataPropertyHandling.ReadAhead,
        Converters = new List<JsonConverter>
        {
            new PrefixedGuidJsonConverter()
        }
    };  
}
