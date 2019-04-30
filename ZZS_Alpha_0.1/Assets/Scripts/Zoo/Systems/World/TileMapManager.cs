using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Zoo.Utilities.Rendering.TileMap;

namespace Zoo.Systems.World
{
    public class TileMapManager : SystemManager<TileMapManager>
    {
        /// <summary>
        /// Map of the currently loaded terrain.
        /// </summary>
        public MapData ActiveMapData;

        public TileMapRenderer TileMapRenderer;

        /// <summary>
        /// Render tilemap with ActiveMapData.
        /// </summary>
        void RenderActiveTileMap()
        {
            TileMapRenderer.RenderMap(ActiveMapData);
        }

        /// <summary>
        /// Set Active MapData & render it.
        /// </summary>
        /// <param name="mapToLoad"></param>
        public void InitializeTileMap(MapData mapToLoad)
        {
            ActiveMapData = mapToLoad;
            RenderActiveTileMap();
        }

        /// <summary>
        /// Returns tile at given index.
        /// </summary>
        /// <param name="tileIndex"></param>
        /// <returns></returns>
        public Tile GetTile(Vector2Int tileIndex)
        {
            return ActiveMapData.TileMap[tileIndex.x, tileIndex.y];
        }

        /// <summary>
        /// Returns tile at given index.
        /// </summary>
        /// <param name="tileIndex"></param>
        /// <returns></returns>
        public Tile GetTile(Vector2 tileIndex)
        {
            return ActiveMapData.TileMap[(int)tileIndex.x, (int)tileIndex.y];
        }

        /// <summary>
        /// Returns tile at given x & z index.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public Tile GetTile(int x, int z)
        {
            return ActiveMapData.TileMap[x, z];
        }

        /// <summary>
        /// Returns tile that has the given vertex as its' bottom left point.
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
        public Tile GetTile(Vector3 vertex)
        {
           return ActiveMapData.TileMap[(int) (vertex.x * MapData.Resolution + ActiveMapData.Offset),
                (int) (vertex.z * MapData.Resolution + ActiveMapData.Offset)];
        }


    }
}
