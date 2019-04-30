using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Jobs;
using UnityEngine;
using Unity.Collections;
using Unity.Entities;
using Zoo.Systems;
using Zoo.Systems.World;

namespace Zoo.Utilities.Rendering.TileMap
{

    public class TileMapRenderer : MonoBehaviour
    {
        
        public Material[] TerrainMaterials;

        private static MapData _currentMap;

        private static bool[] _dirtyChunks;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void RenderMap(MapData mapToRender)
        {
            //Debug.Log("Rendering playable map");
            _currentMap = mapToRender;
            _currentMap.ChunkGameObjects = new GameObject[mapToRender.TileMap.GetLength(0) / MapData.ChunkSize, mapToRender.TileMap.GetLength(1) / MapData.ChunkSize];

            var indices = new List<Vector2Int>();
            for (var x = 0; x < _currentMap.TileMap.GetLength(0) / MapData.ChunkSize; x++)
            {
                for (var z = 0; z < _currentMap.TileMap.GetLength(1) / MapData.ChunkSize; z++)
                {
                    CreateChunk(new Vector2Int(x, z));
                }
            }


        }

        public void CreateChunk(Vector2Int chunkIndex)
        {
            // Create Mesh & Mesh Game Object.
            GameObject go = new GameObject($"Chunk ({chunkIndex.x}, {chunkIndex.y})");
            go.transform.parent = transform;
            go.AddComponent<MeshFilter>();
            go.AddComponent<MeshCollider>();
            go.AddComponent<MeshRenderer>();
            go.GetComponent<MeshRenderer>().sharedMaterials = TerrainMaterials;


            go.layer = 11;

            // Add GameObject to mesh array
            _currentMap.ChunkGameObjects[chunkIndex.x, chunkIndex.y] = go;

            RenderChunk(chunkIndex);

        }

        /// <summary>
        /// Renders the given chunks.
        /// </summary>
        /// <param name="chunkIndices"></param>
        public static void RenderChunks(List<Vector2Int> chunkIndices)
        {
            var xValues = chunkIndices.Select(index => index.x).ToList();
            var yValues = chunkIndices.Select(index => index.y).ToList();
            var first = new Vector2Int(xValues.Min(), yValues.Min());
            var last = new Vector2Int(xValues.Max(), yValues.Max());
            var chunkMaxX = _currentMap.TileMap.GetLength(0) / MapData.ChunkSize;
            var chunkMaxZ = _currentMap.TileMap.GetLength(1) / MapData.ChunkSize;
            for (var x = first.x - 1; x < last.x + 2; x++)
            {
                for (var z = first.y - 1; z < last.y + 2; z++)
                {
                    // Make sure we're in bounds
                    if (x < 0 || z < 0 || x > chunkMaxX || z > chunkMaxZ)
                    {
                        continue;
                    }
                    RenderChunk(new Vector2Int(x, z));
                }
            }
        }

        public static void UpdateAllChunks()
        {
            for (var x = 0; x < _currentMap.ChunkGameObjects.GetLength(0); x++)
            {
                for (var y = 0; y < _currentMap.ChunkGameObjects.GetLength(1); y++)
                {
                    RenderChunk(new Vector2Int(x,y));
                }
            }
        }


        private static void RenderChunk(Vector2Int chunkIndex)
        {
            var chunkObject = TileMapManager.Instance.ActiveMapData.ChunkGameObjects[chunkIndex.x, chunkIndex.y];
            var verts = new List<Vector3>();
            var tris = new List<int>();
            var colors = new List<Color>();
            var terrainTypes = new List<Vector4>();
            var uvs = new List<Vector2>();

            int originX = chunkIndex.x * MapData.ChunkSize, originY = chunkIndex.y * MapData.ChunkSize;
            int maxX = originX + MapData.ChunkSize, maxY = originY + MapData.ChunkSize;


            for (var x = originX; x < maxX; x++)
            {
                for (var y = originY; y < maxY; y++)
                {
                    DrawQuad(TileMapManager.Instance.ActiveMapData.TileMap[x, y],
                        ref verts, ref tris, ref colors, ref terrainTypes, ref uvs);
                }
            }


            var mesh = new Mesh();

            mesh.SetVertices(verts);
            mesh.SetTriangles(tris, 0);
            mesh.SetUVs(0, uvs);
            mesh.SetUVs(1, terrainTypes);
            mesh.SetColors(colors);
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();

            chunkObject.GetComponent<MeshCollider>().sharedMesh = mesh;
            chunkObject.GetComponent<MeshFilter>().sharedMesh = mesh;
        }

        private static void DrawQuad(Tile tile, ref List<Vector3> verts, ref List<int> tris,
            ref List<Color> colors, ref List<Vector4> terrainTypes, ref List<Vector2> uvs)
        {

            int count = verts.Count;
            verts.Add(tile.Vertices[0, 0]);
            verts.Add(tile.Vertices[0, 1]);
            verts.Add(tile.Vertices[1, 1]);
            verts.Add(tile.Vertices[1, 0]);

            colors.Add(new Color(1f, 0f, 0f, 0f));
            colors.Add(new Color(0f, 1f, 0f, 0f));
            colors.Add(new Color(0f, 0f, 1f, 0f));
            colors.Add(new Color(0f, 0f, 0f, 1f));




            terrainTypes.Add(tile.GetVertexTerrainTypes());
            terrainTypes.Add(tile.GetVertexTerrainTypes());
            terrainTypes.Add(tile.GetVertexTerrainTypes());
            terrainTypes.Add(tile.GetVertexTerrainTypes());

            tris.Add(count);
            tris.Add(count + 1);
            tris.Add(count + 2);

            tris.Add(count);
            tris.Add(count + 2);
            tris.Add(count + 3);

            // UVs
            uvs.Add(new Vector2(0, 0));
            uvs.Add(new Vector2(0, 1));
            uvs.Add(new Vector2(1, 1));
            uvs.Add(new Vector2(1, 0));

        }
    }
}

