using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zoo.Systems.Landscaping;

namespace Zoo.Systems.World
{
    /// <summary>
    /// Facing of a sloped tile. For slopes, facing direction is the upper side.
    /// If None, tile is not a valid slope.
    /// </summary>
    public enum SlopeFacing { North, South, East, West, None}

    /// <summary>
    /// Data class representing a single in-game tile, of the resolution set in ActiveMapData.
    /// </summary>
    [System.Serializable]
    public class Tile
    {

        /// <summary>
        /// X/Z world position coordinates of this tile, mapped to the bottom left vertex.
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        public Vector2 TilePosition;

        /// <summary>
        /// Tiles index in the ActiveMapData array.
        /// </summary>
        public Vector2Int TileMapDataIndex;

        /// <summary>
        /// Index of chunk to which this tile belongs.
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        public Vector2Int TileChunkIndex;

        /// <summary>
        /// If false, tile is outside playable map area.
        /// </summary>
        public bool Playable;

        /// <summary>
        /// When true, terrain texture will become cliff face.
        /// </summary>
        public bool IsCliff = false;

        /// <summary>
        /// When true, the tile is a valid slope for slope-based construction checks.
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        public bool IsValidSlope { get; private set; }

        /// <summary>
        /// Slope facing of this tile, if valid slope.
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        public SlopeFacing Facing { get; private set; }

        /// <summary>
        /// True if all four vertices are the same height.
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        public bool IsFlat => (from Vector3 vertex in Vertices select vertex.y).Distinct().Count() < 2;

        /// <summary>
        /// Array of all modifiable vertices in this tile, where [0,0] (x,z) is the bottom left.
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        public readonly Vector3[,] Vertices = new Vector3[2, 2];

        private float _height;

        [Newtonsoft.Json.JsonProperty("_vertexTerrainTypes")]
        private Vector4 _vertexTerrainTypes;

       // public int TerrainTypeIndex = (int)TerrainType.Grass;

        public const int CliffTextureTypeIndex = (int)TerrainType.Cliff;

        /// <summary>
        /// Maximum valid distance in height between the two sides of a slope.
        /// If slope grade is more than the half of the max, a path placed on it
        /// should be stairs. 
        /// </summary>
        public const float MaxSlopeGrade = 1f;

        public Vector4 GetVertexTerrainTypes()
        {
            UpdateHeightInfo();

            if (IsCliff)
            {
                return new Vector4(CliffTextureTypeIndex, CliffTextureTypeIndex,
                    CliffTextureTypeIndex, CliffTextureTypeIndex);
            }
            return _vertexTerrainTypes;
        }

        public void SetVertexTerrainTypes(Vector4 value)
        {
            _vertexTerrainTypes = value;

        }

        #region Vertex Accessors

        /// <summary>
        /// Bottom Left (0,0) vertex of this tile.
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        public Vector3 BottomLeftVertex
        {
            get => Vertices[0, 0];
            set
            {
                Vertices[0, 0] = value;
                UpdateHeightInfo();
            }
        }
        [Newtonsoft.Json.JsonIgnore]
        public float BottomLeftVertexTerrain
        {
            get => GetVertexTerrainTypes().x;
            set => _vertexTerrainTypes.x = value;
        }

        /// <summary>
        /// Top Left (0,1) vertex of this tile.
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        public Vector3 TopLeftVertex
        {
            get => Vertices[0, 1];
            set
            {
                Vertices[0, 1] = value;
                UpdateHeightInfo();
            }
        }

        [Newtonsoft.Json.JsonIgnore]
        public float TopLeftVertexTerrain
        {
            get => GetVertexTerrainTypes().y;
            set => _vertexTerrainTypes.y = value;
        }


        /// <summary>
        /// Top Right (1,1) vertex of this tile.
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        public Vector3 TopRightVertex
        {
            get => Vertices[1, 1];
            set
            {
                Vertices[1, 1] = value;
                UpdateHeightInfo();
            }
        }

        [Newtonsoft.Json.JsonIgnore]
        public float TopRightVertexTerrain
        {
            get => GetVertexTerrainTypes().z;
            set => _vertexTerrainTypes.z = value;
        }


        /// <summary>
        /// Bottom Right (1,0) vertex of this tile.
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        public Vector3 BottomRightVertex
        {
            get => Vertices[1, 0];
            set
            {
                Vertices[1, 0] = value;
                UpdateHeightInfo();
            }
        }

        [Newtonsoft.Json.JsonIgnore]
        public float BottomRightVertexTerrain
        {
            get => GetVertexTerrainTypes().w;
            set => _vertexTerrainTypes.w = value;
        }

        #endregion

        /// <summary>
        ///  Height Y world position coordinate of the tile, based on lowest vertex height.
        /// </summary>
        public float GetHeight()
        {
           RecalculateHeight();
            return _height;
        }

        public Tile()
        {
            UpdateHeightInfo();
        }

        public Tile(Vector2 tilePosition, Vector2Int tileMapDataIndex, bool playable)
        {
            TilePosition = tilePosition;
            TileMapDataIndex = tileMapDataIndex;
            Playable = playable;
            UpdateHeightInfo();
        }

        /// <summary>
        /// Sets tile height to height of lowest vertex.
        /// </summary>
        public void RecalculateHeight()
        {
            _height = (from Vector3 vertex in Vertices select vertex.y).Concat(new float[] {100000}).Min();
        }

        /// <summary>
        /// Checks if tile is a valid slope. Slope is valid if two opposing sides of the tile
        /// have different heights, and have a valid angle (defined in distance between each side).
        /// </summary>
        private void CheckIfSlope()
        {
            // If flat or a cliff, we're out of here.
            if (IsFlat || IsCliff)
            {
                IsValidSlope = false;
                Facing = SlopeFacing.None;
            }
            // Otherwise, check if there are opposing sides with different heights.
            var westSide = Vertices[0, 0].y == Vertices[0, 1].y; // SW/NW
            var northSide = Vertices[0, 1].y == Vertices[1, 1].y; // NW/NE
            var eastSide = Vertices[1, 1].y == Vertices[1, 0].y; // NE/SE
            var southSide = Vertices[1, 0].y == Vertices[0, 0].y; // SE/SW
            // Proceed based on combos.
            if (westSide && eastSide)
            {
                Facing = Vertices[0, 0].y > Vertices[1, 1].y ? SlopeFacing.West : SlopeFacing.East;
                var grade = Mathf.Abs(Vertices[0, 0].y - Vertices[1, 1].y);
                IsValidSlope = CheckIfValidSlopeGrade(grade);
                // If not valid slope, set facing back to none.
                Facing = IsValidSlope ? Facing : SlopeFacing.None;
            }
            else if (northSide && southSide)
            {   
                Facing = Vertices[0, 1].y > Vertices[0, 0].y ? SlopeFacing.North : SlopeFacing.South;
                var grade = Mathf.Abs(Vertices[0, 0].y - Vertices[1, 1].y);
                IsValidSlope = CheckIfValidSlopeGrade(grade);
                // If not valid slope, set facing back to none.
                Facing = IsValidSlope ? Facing : SlopeFacing.None;
            }
            else
            {
                // Otherwise, not a valid slope.
                IsValidSlope = false;
                Facing = SlopeFacing.None;
            }

        }

        /// <summary>
        /// Returns true if given value is within the allowable slope grade,
        /// where grade is defined by the height distance between the two sides
        /// of an otherwise-valid slope.
        /// </summary>
        /// <returns></returns>
        private bool CheckIfValidSlopeGrade(float distanceBetweenSides)
        {
            return distanceBetweenSides <= MaxSlopeGrade;
        }

        /// <summary>
        /// This is bad and should not be used. Why hasn't Daniel fixed this?
        /// Does not update neighbor tiles.
        /// </summary>
        /// <param name="newHeight"></param>
        [Obsolete]
        public void SetHeight(float newHeight)
        {
            Vertices[0, 0].y = newHeight;
            Vertices[0, 1].y = newHeight;
            Vertices[1, 1].y = newHeight;
            Vertices[1, 0].y = newHeight;
            RecalculateHeight();
        }

        public Vector4 GetVertexHeights()
        {
            return new Vector4(Vertices[0, 0].y, Vertices[0, 1].y, 
                Vertices[1, 1].y, Vertices[1, 0].y);
        }

        /// <summary>
        /// Set terrain type for entire tile.
        /// </summary>
        /// <param name="terrainTypeIndex"></param>
        public void SetTerrainType(int terrainTypeIndex)
        {
            SetVertexTerrainTypes(new Vector4(terrainTypeIndex, terrainTypeIndex, 
              terrainTypeIndex, terrainTypeIndex));
        }

        /// <summary>
        /// Set terrain for entire tile, allowing for different types.
        /// </summary>
        /// <param name="terrainTypeIndices"></param>
        public void SetTerrainType(Vector4 terrainTypeIndices)
        {
            SetVertexTerrainTypes(terrainTypeIndices);
        }

        /// <summary>
        /// Performs all checks necessary when tile height is changed.
        /// </summary>
        private void UpdateHeightInfo()
        {
            if (IsFlat)
            {
                IsCliff = false;
                IsValidSlope = false;
                return;
            }
            float min = 100, max = -100;

            foreach (var vert in Vertices)
            {
                if (vert.y < min)
                    min = vert.y;
                else if (vert.y > max)
                    max = vert.y;
            }

            IsCliff = Mathf.Abs(min - max) > 1.5;
            if (IsCliff)
            {
                IsValidSlope = false;
            }
            else
            CheckIfSlope();
            // Raise TileHeightChanged event.
            OnTileHeightChanged();
        }

        public delegate void TileHeightChangedHandler(Tile tile);

        /// <summary>
        /// Called any time this tile's height changes.
        /// </summary>
        public event TileHeightChangedHandler TileHeightChanged;

        public virtual void OnTileHeightChanged()
        {
            TileHeightChanged?.Invoke(this);
        }
    }
}
