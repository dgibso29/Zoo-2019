using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using Zoo.Systems.Landscaping;

namespace Zoo.Systems.World
{

    /// <summary>
    /// Holds 
    /// </summary>
    [System.Serializable]
    public class MapData
    {
        /// <summary>
        /// Array of all tiles on the map. Indices are TileCoordinates * Resolution.
        /// </summary>
        public Tile[,] TileMap;

        /// <summary>
        /// Width X of the map in tiles.
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        public readonly int WidthX;

        /// <summary>
        /// Length Z of the map in tiles.
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        public readonly int LengthZ;

        public int Offset;

        /// <summary>
        /// Size of chunks in tiles.
        /// </summary>
        public const int ChunkSize = 25;

        /// <summary>
        /// Distance between each allowable terrain height increment, in world space Y.
        /// </summary>
        public const float HeightIncrement = .25f;

        /// <summary>
        /// Determines number of tiles per 1x1 world space coordinates, where numTiles = 2 ^ Resolution.
        /// </summary>
        public const int Resolution = 2;

        public const int NonPlayableBufferSize = 50;

        [Newtonsoft.Json.JsonIgnore]
        public GameObject[,] ChunkGameObjects;

        /// <summary>
        /// Generates a map of the given size using the given noise data for height info. Offset is intended to be used
        /// in conjunction with generating a non-playable terrain area to border the playable map.
        /// </summary>
        /// <param name="widthX"></param>
        /// <param name="lengthZ"></param>
        /// <param name="noiseData"></param>
        /// <param name="offset"></param>
        public MapData(int widthX, int lengthZ, float[,] noiseData, int offset = 0)
        {
            WidthX = widthX * Resolution;
            LengthZ = lengthZ * Resolution;
            Offset = offset;
            TileMap = GenerateTileMap(noiseData, offset);
        }

        /// <summary>
        /// Generates a flat map of the given size. Offset is intended to be used
        /// in conjunction with generating a non-playable terrain area to border the playable map.
        /// </summary>
        public MapData(int widthX, int lengthZ, int offset = 0)
        {
            WidthX = widthX * Resolution;
            LengthZ = lengthZ * Resolution;
            Offset = offset;
            TileMap = GenerateTileMap(offset);
        }

        /// <summary>
        /// Constructs a ActiveMapData object from a pre-existing TileMap. Primarily intended for deserializing.
        /// </summary>
        /// <param name="tileMap"></param>
        /// <param name="offset"></param>
        [Newtonsoft.Json.JsonConstructor]
        public MapData(Tile[,] tileMap, int offset = 0)
        {
            WidthX = tileMap.GetLength(0);
            LengthZ = tileMap.GetLength(1);
            Offset = offset;
            TileMap = tileMap;
        }

        /// <summary>
        /// Generates a TileMap that uses noiseData values for height information.
        /// Offset is used to generate non-playable map tiles in their correct world position.
        /// </summary>
        /// <param name="noiseData"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        Tile[,] GenerateTileMap(float[,] noiseData, int offset = 0)
        {
            var tiles = new Tile[WidthX, LengthZ];
            for (var x = 0; x < WidthX; x++)
            {
                for (var z = 0; z < LengthZ; z++)
                {
                    //Debug.Log($"Trying {x},{z} with offset {offset}");
                    // Why cast every time when you can just cast the first time?
                    float adjustedX = x - offset, adjustedZ = z - offset;

                    // Check if tile is inside playable area
                    var playable = ((x >= NonPlayableBufferSize && x < WidthX - NonPlayableBufferSize) && (z >= NonPlayableBufferSize && z < LengthZ - NonPlayableBufferSize));
                    playable = true;
                    var tile = new Tile(new Vector2((adjustedX - offset) / Resolution, (adjustedZ - offset) / Resolution),
                        new Vector2Int(x, z), playable)
                    {
                        TileChunkIndex = new Vector2Int(x / ChunkSize, z / ChunkSize),
                        Vertices =
                        {
                            [0, 0] = new Vector3(adjustedX / Resolution, noiseData[x, z], adjustedZ / Resolution),
                            [1, 0] = new Vector3((adjustedX + 1) / Resolution, noiseData[x + 1, z], adjustedZ / Resolution),
                            [1, 1] = new Vector3((adjustedX + 1) / Resolution, noiseData[x + 1, z + 1],
                                (adjustedZ + 1) / Resolution),
                            [0, 1] = new Vector3(adjustedX / Resolution, noiseData[x, z + 1], (adjustedZ + 1) / Resolution)
                        }
                    };
                    tile.SetTerrainType(1);
                    tiles[x, z] = tile;

                }
            }

            return tiles;
        }
        
        /// <summary>
        /// Generates a flat TileMap.
        /// Offset is used to generate non-playable map tiles in their correct world position.
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        private Tile[,] GenerateTileMap(int offset = 0)
        {
            var tiles = new Tile[WidthX, LengthZ];
            for (var x = 0; x < WidthX; x++)
            {
                for (var z = 0; z < LengthZ; z++)
                {
                    // Why cast every time when you can just cast the first time?
                    float adjustedX = x - offset, adjustedZ = z - offset;

                    // Check if tile is inside playable area
                    var playable = (x >= NonPlayableBufferSize && x < WidthX - NonPlayableBufferSize) && (z >= NonPlayableBufferSize && z < LengthZ - NonPlayableBufferSize);

                    var tile = new Tile(new Vector2((adjustedX - offset) / Resolution, (adjustedZ - offset) / Resolution),
                        new Vector2Int(x, z), playable)
                    {
                        TileChunkIndex = new Vector2Int(x / ChunkSize, z / ChunkSize),
                        Vertices =
                        {
                            [0, 0] = new Vector3(adjustedX / Resolution, 0, adjustedZ / Resolution),
                            [1, 0] = new Vector3((adjustedX + 1) / Resolution, 0, adjustedZ / Resolution),
                            [1, 1] = new Vector3((adjustedX + 1) / Resolution, 0,
                                (adjustedZ + 1) / Resolution),
                            [0, 1] = new Vector3(adjustedX / Resolution, 0, (adjustedZ + 1) / Resolution)
                        }
                    };

                    tiles[x, z] = tile;

                }
            }


            return tiles;
        }

        /// <summary>
        /// Returns terrain type index of this vertex.
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
        public int GetVertexTerrainType(Vector3 vertex)
        {
            return (int)TileMap[(int) (vertex.x * Resolution + Offset), (int) (vertex.z * Resolution + Offset)]
                .BottomLeftVertexTerrain;
        }

        /// <summary>
        /// Returns vertex at given XZ coordinates. Y is ignored.
        /// </summary>
        /// <param name="vertexPosition"></param>
        /// <returns></returns>
        public Vector3 GetVertex(Vector3 vertexPosition)
        {
            return TileMap[(int) (vertexPosition.x * Resolution + Offset),
                (int) (vertexPosition.z * Resolution + Offset)].BottomLeftVertex;
        }

        /// <summary>
        /// Returns vertex at given XZ coordinates.
        /// </summary>
        /// <param name="vertexPosition"></param>
        /// <returns></returns>
        public Vector3 GetVertex(Vector2 vertexPosition)
        {
            return TileMap[(int)(vertexPosition.x * Resolution + Offset),
                (int)(vertexPosition.y * Resolution + Offset)].BottomLeftVertex;
        }

        /// <summary>
        /// Get lerped height between two vert heights.
        /// </summary>
        /// <returns></returns>
        public float GetLerpedHeight(Vector3 vertexOne, Vector3 vertexTwo)
        {
            var result = Mathf.Lerp(vertexOne.y, vertexTwo.y, .5f);
            return result;
            //return Mathf.Lerp(vertexOne.y, .5f, vertexTwo.y);
        }

        /// <summary>
        /// Returns height of given vertex.
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
        public float GetVertexHeight(Vector3 vertex)
        {
            return TileMap[(int)(vertex.x * Resolution + Offset), (int)(vertex.z * Resolution + Offset)]
                .BottomLeftVertex.y;
        }
        /// <summary>
        /// Returns the 4 tiles that share this vertices position.
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
        public Tile[] GetVertexTiles(Vector3 vertex)
        {
            var tiles = new Tile[4];
            // Normalise vertices to an integer index if needed.
            var vertexIndex = new Vector2(vertex.x * Resolution + Offset, vertex.z * Resolution + Offset);
            // Try to add each possible tile
            // Bottom Left (0, 0)
            try
            {
                tiles[0] = TileMap[(int)vertexIndex.x - 1, (int)vertexIndex.y - 1];
            }
            catch
            {
                // ignored
            }
            // Top Left (0, 1)
            try
            {
                tiles[1] = TileMap[(int)vertexIndex.x - 1, (int)vertexIndex.y];
            }
            catch
            {
                // ignored
            }
            // Top Right (1, 1)
            try
            {
                tiles[2] = TileMap[(int)vertexIndex.x, (int)vertexIndex.y];
            }
            catch
            {
                // ignored
            }
            // Bottom Right (1, 0)
            try
            {
                tiles[3] = TileMap[(int)vertexIndex.x, (int)vertexIndex.y - 1];
            }
            catch
            {
                // ignored
            }



            return tiles;
        }

        /// <summary>
        /// Updates all vertices at vertices X/Y with the value of newHeight.
        /// </summary>
        /// <param name="vertex"></param>
        /// <param name="newHeight"></param>
        public void SetWeldedVertexHeight(Vector3 vertex, float newHeight)
        {
            Tile[] tilesToUpdate = GetVertexTiles(vertex);

            // Tiles array should always be ordered in the same way
            tilesToUpdate[0].TopRightVertex = new Vector3(vertex.x, newHeight, vertex.z);
            tilesToUpdate[1].BottomRightVertex = new Vector3(vertex.x, newHeight, vertex.z);
            tilesToUpdate[2].BottomLeftVertex = new Vector3(vertex.x, newHeight, vertex.z);
            tilesToUpdate[3].TopLeftVertex = new Vector3(vertex.x, newHeight, vertex.z);

        }

        /// <summary>
        /// Updates all vertices at vertices X/Y with the corresponding newHeights values.
        /// </summary>
        /// <param name="vertices"></param>
        /// <param name="newHeights"></param>
        public void SetWeldedVertexHeight(List<Vector3> vertices, List<float> newHeights)
        {
            for (var i = 0; i < vertices.Count; i++)
            {
                var vertex = vertices[i];
                var tilesToUpdate = GetVertexTiles(vertex);

                // Tiles array should always be ordered in the same way
                tilesToUpdate[0].TopRightVertex = new Vector3(vertex.x, newHeights[i], vertex.z);
                tilesToUpdate[1].BottomRightVertex = new Vector3(vertex.x, newHeights[i], vertex.z);
                tilesToUpdate[2].BottomLeftVertex = new Vector3(vertex.x, newHeights[i], vertex.z);
                tilesToUpdate[3].TopLeftVertex = new Vector3(vertex.x, newHeights[i], vertex.z);
            }

        }

        /// <summary>
        /// Set given vertex position to terrain type. Will set all 4 vertices at that position.
        /// </summary>
        /// <param name="vertex"></param>
        /// <param name="terrainIndex"></param>
        public void SetWeldedVertexTerrain(Vector3 vertex, int terrainIndex)
        {
            Tile[] tilesToUpdate = GetVertexTiles(vertex);

            tilesToUpdate[0].TopRightVertexTerrain = terrainIndex;
            tilesToUpdate[1].BottomRightVertexTerrain = terrainIndex;
            tilesToUpdate[2].BottomLeftVertexTerrain = terrainIndex;
            tilesToUpdate[3].TopLeftVertexTerrain = terrainIndex;

        }

        /// <summary>
        /// Set given vertices to terrain type. Will set all 4 vertices at each vertex position.
        /// </summary>
        /// <param name="vertices"></param>
        /// <param name="terrainIndex"></param>
        public void SetWeldedVertexTerrain(List<Vector3> vertices, int terrainIndex)
        {
            foreach (var vertex in vertices)
            {
                var tilesToUpdate = GetVertexTiles(vertex);

                // Tiles array should always be ordered in the same way
                tilesToUpdate[0].TopRightVertexTerrain = terrainIndex;
                tilesToUpdate[1].BottomRightVertexTerrain = terrainIndex;
                tilesToUpdate[2].BottomLeftVertexTerrain = terrainIndex;
                tilesToUpdate[3].TopLeftVertexTerrain = terrainIndex;
            }
        }

        /// <summary>
        /// Return list of tiles corresponding to input tile indices.
        /// IE, (1,0) = TileMap[1, 0]
        /// </summary>
        /// <param name="tileIndices"></param>
        /// <returns></returns>
        public List<Tile> GetTiles(List<Vector2Int> tileIndices)
        {
            var tiles = new List<Tile>();
            foreach (var index in tileIndices)
            {
                tiles.Add(TileMap[index.x, index.y]);
            }

            return tiles;
        }

        public Tile[,] GetNeighboringTiles(Tile tile)
        {
            var tiles = new Tile[3, 3];

            var posX = tile.TileMapDataIndex.x;
            var posZ = tile.TileMapDataIndex.y;

            for (var x = -1; x < 2; x++)
            {
                for (var z = -1; z < 2; z++)
                {
                    try
                    {
                        tiles[x + 1, z + 1] = TileMap[posX + x, posZ + z];
                    }
                    catch
                    {
                        // Empty
                    }
                }
            }

            return tiles;

        }

        /// <summary>
        /// Sets tile to given terrain type, 
        /// </summary>
        /// <param name="tile"></param>
        /// <param name="terrainIndex"></param>
        public void SetTileTerrainType(Tile tile, int terrainIndex)
        {
            tile.SetTerrainType(terrainIndex);

            foreach (var vert in tile.Vertices)
            {
                SetWeldedVertexTerrain(vert, terrainIndex);
            }

        }


    }
}
