﻿namespace Stratis.Features.AzureIndexer.Converters
{
    using System;
    using NBitcoin;
    using Newtonsoft.Json;

    public class ScriptJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(Script).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            return new Script(Convert.FromBase64String((string)reader.Value));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(((Script)value).ToBytes(true));
        }
    }
}
