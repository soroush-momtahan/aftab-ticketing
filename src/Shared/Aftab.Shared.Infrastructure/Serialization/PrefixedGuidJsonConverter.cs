using Aftab.Shared.Domain.PrefixedGuidTools;
using Newtonsoft.Json;

namespace Aftab.Shared.Infrastructure.Serialization;

public class PrefixedGuidJsonConverter : JsonConverter
{
    public override bool CanConvert(Type objectType) => 
        typeof(PrefixedGuidV3).IsAssignableFrom(objectType);

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        // نوشتن به صورت رشته (prd_guid)
        writer.WriteValue(value?.ToString());
    }

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        string? value = reader.Value as string;
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        // استفاده از متد From که نوشتیم برای پارس کردن رشته
        // چون متد From جنریک است، باید با Reflection صدا زده شود یا منطق پارس تکرار شود.
        // راه ساده‌تر: تکرار منطق پارس برای ساخت نمونه
        
        try 
        {
            // 1. جدا کردن پرفیکس و Guid
            string[] parts = value.Split('_');
            if (parts.Length < 2)
            {
                throw new JsonSerializationException("Invalid format");
            }

            // 2. پارس کردن Guid
            if (!Guid.TryParse(parts[1], out Guid guidValue))
            {
                throw new JsonSerializationException("Invalid Guid");
            }

            // 3. ساخت نمونه با Guid (چون سازنده ما Guid میگیرد)
            return Activator.CreateInstance(objectType, guidValue);
        }
        catch(Exception ex)
        {
            throw new JsonSerializationException($"Cannot convert {value} to {objectType.Name}", ex);
        }
    }
}
