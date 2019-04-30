using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Zoo.Assets;

namespace Zoo.IO.JSONConverters
{
    public class GameAssetJSONDataConverter : JsonConverter
    {

        private static readonly Type JsonType = typeof(GameAssetJSONData);

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException("Should never be called because CanWrite is false.");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return PopulateJsonData(reader);
        }

        private static GameAssetJSONData PopulateJsonData(JsonReader reader)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return new GameAssetJSONData();
            }

            var metadataTags = new List<string>();
            var dynamicAttributes = new List<string>();
            var jObject = JObject.Load(reader);
            var assetStringID = jObject["AssetStringID"].Value<string>();
            if (jObject["Metadata Tags"].HasValues)
            {
                metadataTags = jObject["Metadata Tags"].Values<string>().ToList();
            }

            if (jObject["Dynamic Attributes"].HasValues)
            {
                dynamicAttributes = jObject["Dynamic Attributes"].Values<string>().ToList();
            }
            var data = new GameAssetJSONData()
            {
                AssetStringID = assetStringID,
                MetadataTags = metadataTags,
                DynamicAttributes = dynamicAttributes,
            };
            return data;
        }


        public override bool CanWrite => false;


        public override bool CanConvert(Type objectType)
        {
            return objectType == JsonType;
        }
    }
}
