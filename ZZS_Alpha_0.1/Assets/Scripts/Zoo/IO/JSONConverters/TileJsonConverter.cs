using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using Zoo.Systems.World;

namespace Zoo.IO.JSONConverters
{
    public class TileJsonConverter : JsonConverter
    {
        private static readonly Type TileType = typeof(Tile);

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
            }
            else if (value.GetType() != TileType)
            {
                writer.WriteNull();
            }

            WriteTile(writer, value as Tile);
        }

        private static void WriteTile(JsonWriter writer, Tile tile)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("x");
            writer.WriteValue(tile.TileMapDataIndex.x);
            writer.WritePropertyName("y");
            writer.WriteValue(tile.TileMapDataIndex.y);
            writer.WritePropertyName("playable");
            writer.WriteValue(tile.Playable);
            writer.WritePropertyName("types");
            var vector = tile.GetVertexTerrainTypes();
            writer.WriteValue(JsonConvert.SerializeObject(vector));
            writer.WritePropertyName("heights");
            vector = tile.GetVertexHeights();
            writer.WriteValue(JsonConvert.SerializeObject(vector));
            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return PopulateTile(reader);
        }

        private static Tile PopulateTile(JsonReader reader)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return new Tile();
            }
            var jObject = JObject.Load(reader);
            var index = new Vector2Int(jObject["x"].Value<int>(), jObject["y"].Value<int>());
            
            var heights = JsonConvert.DeserializeObject<Vector4>(jObject["heights"].Value<string>());
            var tilePos = new Vector2((float)index.x / MapData.Resolution, (float)index.y / MapData.Resolution);
            //var heights = jObject["heights"].Value<Vector4>();
            var tile = new Tile
            {
                TilePosition = tilePos,
                TileMapDataIndex = index,
                TileChunkIndex = new Vector2Int(index.x / MapData.ChunkSize, index.y / MapData.ChunkSize),
                Playable = jObject["playable"].Value<bool>(),
                Vertices =
                {
                    [0, 0] = new Vector3((float)index.x / MapData.Resolution, heights.x, (float)index.y / MapData.Resolution),
                    [1, 0] = new Vector3(((float)index.x + 1) / MapData.Resolution, heights.w, (float)index.y / MapData.Resolution),
                    [1, 1] = new Vector3(((float)index.x + 1) / MapData.Resolution, heights.z,
                        ((float)index.y + 1) / MapData.Resolution),
                    [0, 1] = new Vector3((float)index.x / MapData.Resolution, heights.y, ((float)index.y + 1) / MapData.Resolution)
                }
            };
            var types = JsonConvert.DeserializeObject<Vector4>(jObject["types"].Value<string>());

            //tile.SetVertexTerrainTypes(jObject["types"].Value<Vector4>());
            tile.SetVertexTerrainTypes(types);

            return tile;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == TileType;
        }
    }
}
