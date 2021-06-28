#if !CLIENT
using System;
using Newtonsoft.Json;
using Stratis.Bitcoin.Utilities.JsonConverters;
using Stratis.Features.AzureIndexer;

namespace AzureIndexer.Api.JsonConverters
{
#if !NOJSONNET
	public
#else
	internal
#endif
	class BalanceLocatorJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(BalanceLocator).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            try
            {
                return reader.TokenType == JsonToken.Null ? null : BalanceLocator.Parse(reader.Value.ToString());
            }
            catch (FormatException)
            {
                throw new JsonObjectException("Invalid BalanceLocator", reader);
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value != null)
                writer.WriteValue(value.ToString());
        }
    }
}
#endif