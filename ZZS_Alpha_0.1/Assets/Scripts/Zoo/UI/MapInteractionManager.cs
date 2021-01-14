using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zoo.AssetComponents;
using Zoo.Assets;
using Zoo.Systems;
using Zoo.Systems.World;

namespace Zoo.UI
{
    public class MapInteractionManager : SystemManager<MapInteractionManager>
    {
        /// <summary>
        /// Returns currently selected tile from MapData.
        /// </summary>
        /// <returns></returns>
        public Tile GetCurrentTile()
        {
            return TileMapManager.Instance.GetTile(CurrentTileIndex);
        }

        /// <summary>
        /// Map data index of the currently selected Tile.
        /// </summary>
        public Vector2Int CurrentTileIndex { get; private set; } = new Vector2Int(0, 0);

        /// <summary>
        /// Bottom left vertex of the current tile, regardless of current vertex.
        /// </summary>
        public Vector3 CurrentTilePosition { get; private set; }

        /// <summary>
        /// World position of the nearest vertex to mouse position.
        /// </summary>
        public Vector3 CurrentVertex { get; private set; } = new Vector3(0, 0, 0);

        /// <summary>
        /// Terrain type of the nearest vertex to mouse position.
        /// </summary>
        public int CurrentVertexTerrain => TileMapManager.Instance.ActiveMapData.GetVertexTerrainType(Instance.CurrentVertex);

        public TileSelectionData CurrentTileSelection;

        public Material SelectionMeshMaterial;

        private MeshFilter _meshFilter;

        private MeshRenderer _meshRenderer;

        /// <summary>
        /// Verts of the selection mesh.
        /// </summary>
        private List<Vector3> _selectionMeshVertices = new List<Vector3>();

        /// <summary>
        /// When true, player is currently selecting a specific vertex, not a full tile.
        /// </summary>
        public bool OverVertex = false;

        public bool SelectionMeshEnabled { get; private set; } = false;

        /// <summary>
        /// Enables selection mesh, including brush functions if not disabled.
        /// </summary>
        public void EnableSelectionMesh(bool brushEnabled = true)
        {
            SelectionMeshEnabled = true;
            _meshRenderer.enabled = true;
            _brushEnabled = brushEnabled;
            if (!_brushEnabled)
                SelectionBrushSize = 1;
        }

        public void DisableSelectionMesh()
        {
            SelectionMeshEnabled = false;
            DraggingSelectionMesh = false;
            _meshRenderer.enabled = false;
        }

        //public void ToggleSelectionMesh()
        //{
        //    if (!SelectionMeshEnabled)
        //    {
        //        EnableSelectionMesh();
        //    }
        //    else
        //    {
        //        DisableSelectionMesh();
        //    }
        //}

        private Tile _dragSelectionStartTile;

        private Vector3 _dragSelectionStartVertex;

        private int _dragSelectionStartVertexTerrain;

        /// <summary>
        /// When enabled, player is dragging selection mesh.
        /// </summary>
        public bool DraggingSelectionMesh { get; private set; } = false;

        /// <summary>
        /// Begin a dragging action with the selection mesh.
        /// Tile data accessible from CurrentTileSelection field,
        /// or from DragSelectEndEvent args.
        /// Use DragSelect events to consume start/end of selection.
        /// </summary>
        /// <param name="startTile"></param>
        /// <param name="brushEnabled">If false, prevents player from resizing selection brush.</param>
        private void EnableDraggingSelectionMesh(Tile startTile, Vector3 startVertex,
            int startVertexTerrain, bool brushEnabled = true)
        {
            // Protect against calls when already dragging
            if (DraggingSelectionMesh)
                return;
            OnDragSelectStart();
            _dragSelectionStartTile = startTile;
            _dragSelectionStartVertex = startVertex;
            _dragSelectionStartVertexTerrain = startVertexTerrain;
            DraggingSelectionMesh = true;
            SelectionMeshEnabled = true;
            _brushEnabled = brushEnabled;
            if (!brushEnabled) SelectionBrushSize = 1;
            UpdateSelectionMesh();
        }

        /// <summary>
        /// Stop drag selection & raise event holding selection data.
        /// Will also disableMeshSelection if said parameter is true.
        /// If alternateFunction enabled, event will include that bool, allowing for more flexibility.
        /// Alternate function is triggered by alternate input (right mouse, left trigger, etc) dragging.
        /// </summary>
        /// <param name="disableMeshSelection">When true, mesh selection will be disabled.
        /// Intended for one-off uses, where selection is otherwise unused.</param>
        private void DisableDraggingSelectionMesh(bool disableMeshSelection = false, bool alternateFunction = false)
        {
            // Break if already not dragging
            if (!DraggingSelectionMesh)
                return;
            OnDragSelectEnd(
                new DraggingEventArgs(_dragSelectionStartTile, TileMapManager.Instance.GetTile(CurrentTileIndex),
                    _dragSelectionStartVertex, CurrentVertex, _dragSelectionStartVertexTerrain,
                    CurrentVertexTerrain, alternateFunction));
            UpdateSelectedTileData();
            DraggingSelectionMesh = false;
            if (disableMeshSelection)
            {
                SelectionMeshEnabled = false;
                return;
            }
            UpdateSelectionMesh();
        }


        /// <summary>
        /// When enabled, player can re-size selection brush.
        /// </summary>
        private bool _brushEnabled = true;

        /// <summary>
        /// When 1, player can select single tiles and verts.
        /// When larger, player selects size * size grid of tiles.
        /// </summary>
        public static int SelectionBrushSize = 1;


        /// <summary>
        /// Player brush cannot exceed this size.
        /// </summary>
        private readonly int _maxBrushSize = 25;

        //Start is called before the first frame update
        void Start()
        {
            InitializeSelectionMesh();
        }

        // Update is called once per frame
        void Update()
        {
            GetSelectedTile();
            if (SelectionMeshEnabled)
            {
                HandleSelectionMeshInput();
            }
        }

        /// <summary>
        /// Holds Tile data necessary to perform operations on a selection of tiles.
        /// </summary>
        public struct TileSelectionData
        {
            public List<Tile> Tiles;
            /// <summary>
            /// All vertices of the currently selected tiles.
            /// </summary>
            public List<Vector3> TileVertices;
            /// <summary>
            /// Currently selected vertices -- may differ from TileVertices, as it is
            /// generated from start/end vertices, not start/end tiles.
            /// </summary>
            public List<Vector3> Vertices;


            public TileSelectionData(List<Tile> tiles, List<Vector3> tileVertices, List<Vector3> vertices)
            {
                Tiles = tiles;
                TileVertices = tileVertices;
                Vertices = vertices;
            }

        }

        public void ManuallyUpdateSelectionMesh()
        {
            GetSelectedTile();
            UpdateSelectionMesh();
        }


        private void InitializeSelectionMesh()
        {
            _meshFilter = GetComponent<MeshFilter>();
            _meshRenderer = GetComponent<MeshRenderer>();
            GetComponent<MeshRenderer>().sharedMaterial = SelectionMeshMaterial;
        }

        private void HandleSelectionMeshInput()
        {
            // If mouse button held
            if ((Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) && !DraggingSelectionMesh)
            {
                _dragSelectionStartTile = TileMapManager.Instance.GetTile(CurrentTileIndex);
                _dragSelectionStartVertex = CurrentVertex;
                _dragSelectionStartVertexTerrain = CurrentVertexTerrain;
            }
            if ((Input.GetMouseButton(0) || Input.GetMouseButton(1)) && !DraggingSelectionMesh)
            {
                if (_dragSelectionStartVertex != CurrentVertex)
                    EnableDraggingSelectionMesh(_dragSelectionStartTile, _dragSelectionStartVertex,
                        _dragSelectionStartVertexTerrain);
            }

            else if (DraggingSelectionMesh)
            {
                if (Input.GetMouseButtonUp(0))
                    DisableDraggingSelectionMesh();
                else if (Input.GetMouseButtonUp(1))
                    DisableDraggingSelectionMesh(false, true);
                return;
            }

            if (_brushEnabled)
            {
                if (Input.GetKeyDown(KeyCode.LeftBracket))
                {
                    SelectionBrushSize--;
                    SelectionBrushSize = Mathf.Clamp(SelectionBrushSize, 1, _maxBrushSize);
                    UpdateSelectionMesh();
                }
                else if (Input.GetKeyDown(KeyCode.RightBracket))
                {
                    SelectionBrushSize++;
                    SelectionBrushSize = Mathf.Clamp(SelectionBrushSize, 1, _maxBrushSize);
                    UpdateSelectionMesh();
                }
            }
        }

        private void UpdateSelectionMesh()
        {
            UpdateSelectedTileData();
            RenderSelectionMesh();
        }

        private void UpdateSelectedTileData()
        {
            var currTile = GetCurrentTile();
            var currVertex = CurrentVertex;
            if (DraggingSelectionMesh)
            {
                CurrentTileSelection.Tiles = GetDragSelectionTiles(_dragSelectionStartTile,
                    TileMapManager.Instance.GetTile(CurrentTileIndex), out var tileVertices);
                CurrentTileSelection.TileVertices = tileVertices;
                CurrentTileSelection.Vertices = GetDragSelectionVertices(_dragSelectionStartVertex, currVertex);
            }
            else
            {
                CurrentTileSelection.Tiles = GetSelectionBrushData(currTile, currVertex, out var tileVertices,
                    out var vertices);
                CurrentTileSelection.TileVertices = tileVertices;
                CurrentTileSelection.Vertices = vertices;
            }
        }

        /// <summary>
        /// Get selected tile. 
        /// </summary>
        /// <param name="tileIndex"></param>
        /// <returns></returns>
        //[System.Obsolete("Now replaced by TileMapManager.Instance.GetTile()")]
        //private Tile GetTile(Vector2Int tileIndex)
        //{
        //    return TileMapManager.Instance.ActiveMapData.TileMap[tileIndex.x, tileIndex.y];
        //}

        private void RenderSelectionMesh()
        {
            

            var tris = new List<int>();
            var uvs = new List<Vector2>();
            //_selectionMeshVertices = CurrentTileSelection.TileVertices;

            // If size of 2 (we're selecting half of a tile -- a triangle)
            if (SelectionBrushSize == 2 && !DraggingSelectionMesh)
            {
                _selectionMeshVertices = CurrentTileSelection.Vertices;
                tris.Add(0);
                tris.Add(1);
                tris.Add(2);

                // UVs
                uvs.Add(new Vector2(0, 0));
                uvs.Add(new Vector2(0, 1));
                uvs.Add(new Vector2(1, 1));
            }
            // Any other size will draw quads (size of 1 = quad around selected vert).
            else
            {
                if (DraggingSelectionMesh)
                {
                    _selectionMeshVertices = CurrentTileSelection.TileVertices;
                }
                else
                {
                    //_selectionMeshVertices = SelectionBrushSize == 1
                    //    ? CurrentTileSelection.Vertices
                    //    : CurrentTileSelection.TileVertices;
                    _selectionMeshVertices = CurrentTileSelection.TileVertices;
                }

                for (var i = 0; i < _selectionMeshVertices.Count / 4; i++)
                {
                    var count = uvs.Count;

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

            if (_selectionMeshVertices.Count == 0) return;
            // Debug.Log($"Tris {tris.Count}, verts {_selectionMeshVertices.Count}, uvs {uvs.Count}");
            var newMesh = new Mesh();
            newMesh.SetVertices(_selectionMeshVertices);
            newMesh.SetTriangles(tris, 0);
            newMesh.SetUVs(0, uvs);
            newMesh.RecalculateNormals();
            _meshFilter.sharedMesh = newMesh;

        }

        /// <summary>
        /// When clicking & dragging, returns tiles between start & end tiles, and outs vertices between them
        /// </summary>
        /// <param name="startTile"></param>
        /// <param name="endTile"></param>
        /// <param name="verticesToModify"></param>
        /// <returns></returns>
        private static List<Tile> GetDragSelectionTiles(Tile startTile, Tile endTile, out List<Vector3> verticesToModify)
        {
            var tiles = new List<Tile>();
            verticesToModify = new List<Vector3>();

            var startX = startTile.TileMapDataIndex.x <= endTile.TileMapDataIndex.x
                ? startTile.TileMapDataIndex.x
                : endTile.TileMapDataIndex.x;

            var endX = startTile.TileMapDataIndex.x >= endTile.TileMapDataIndex.x
                ? startTile.TileMapDataIndex.x
                : endTile.TileMapDataIndex.x;

            var startZ = startTile.TileMapDataIndex.y <= endTile.TileMapDataIndex.y
                ? startTile.TileMapDataIndex.y
                : endTile.TileMapDataIndex.y;

            var endZ = startTile.TileMapDataIndex.y >= endTile.TileMapDataIndex.y
                ? startTile.TileMapDataIndex.y
                : endTile.TileMapDataIndex.y;

            for (var x = startX; x <= endX; x++)
            {
                for (var z = startZ; z <= endZ; z++)
                {
                    var tile = TileMapManager.Instance.ActiveMapData.TileMap[x, z];
                    tiles.Add(tile);
                    verticesToModify.Add(tile.Vertices[0, 0]);
                    verticesToModify.Add(tile.Vertices[0, 1]);
                    verticesToModify.Add(tile.Vertices[1, 1]);
                    verticesToModify.Add(tile.Vertices[1, 0]);
                }
            }

            return tiles;
        }

        private static List<Vector3> GetDragSelectionVertices(Vector3 startVertex, Vector3 endVertex)
        {
            var verticesToModify = new List<Vector3>();

            var startX = startVertex.x <= endVertex.x
                ? startVertex.x
                : endVertex.x;

            var endX = startVertex.x >= endVertex.x
                ? startVertex.x
                : endVertex.x;

            var startZ = startVertex.z <= endVertex.z
                ? startVertex.z
                : endVertex.z;

            var endZ = startVertex.z >= endVertex.z
                ? startVertex.z
                : endVertex.z;


            for (var x = startX; x <= endX; x += 0.5f)
            {
                for (var z = startZ; z <= endZ; z += 0.5f)
                {
                    verticesToModify.Add(TileMapManager.Instance.ActiveMapData.GetVertex(new Vector2(x, z)));
                }
            }

            return verticesToModify;
        }

        private static List<Tile> GetSelectionBrushData(Tile startTile, Vector3 startVertex, out List<Vector3> tileVertices, out List<Vector3> vertices)
        {
            var tiles = new List<Tile>();
            tileVertices = new List<Vector3>();
            vertices = new List<Vector3>();
            switch (SelectionBrushSize)
            {
                case 1:
                    {
                        vertices.Add(startVertex);
                        // Create diamond around start vertex.
                        var lerpedHeight = TileMapManager.Instance.ActiveMapData.GetLerpedHeight(startVertex,
                            TileMapManager.Instance.ActiveMapData.GetVertex(new Vector2(startVertex.x - .5f,
                                startVertex.z)));
                        tileVertices.Add(new Vector3(startVertex.x - 0.125f, lerpedHeight, startVertex.z));

                        lerpedHeight = TileMapManager.Instance.ActiveMapData.GetLerpedHeight(startVertex,
                            TileMapManager.Instance.ActiveMapData.GetVertex(new Vector2(startVertex.x,
                                startVertex.z + .5f)));
                        tileVertices.Add(new Vector3(startVertex.x, lerpedHeight, startVertex.z + 0.125f));

                        lerpedHeight = TileMapManager.Instance.ActiveMapData.GetLerpedHeight(startVertex,
                            TileMapManager.Instance.ActiveMapData.GetVertex(new Vector2(startVertex.x + .5f,
                                startVertex.z)));
                        tileVertices.Add(new Vector3(startVertex.x + 0.125f, lerpedHeight, startVertex.z));

                        lerpedHeight = TileMapManager.Instance.ActiveMapData.GetLerpedHeight(startVertex,
                            TileMapManager.Instance.ActiveMapData.GetVertex(new Vector2(startVertex.x,
                                startVertex.z - .5f)));
                        tileVertices.Add(new Vector3(startVertex.x, lerpedHeight, startVertex.z - 0.125f));

                        //tileVertices.Add(startTile.Vertices[0, 0]);
                        //tileVertices.Add(startTile.Vertices[0, 1]);
                        //tileVertices.Add(startTile.Vertices[1, 1]);
                        //tileVertices.Add(startTile.Vertices[1, 0]);
                        //tiles.Add(startTile);
                        tiles.Add(TileMapManager.Instance.GetTile(startVertex));
                        return tiles;
                    }
                case 2:
                    {
                        // Check which corner of the current tile the vertex is...
                        if (startVertex == startTile.BottomLeftVertex)
                        {
                            vertices.Add(startTile.BottomRightVertex);
                            vertices.Add(startVertex);
                            vertices.Add(startTile.TopLeftVertex);

                        }
                        else if (startVertex == startTile.TopLeftVertex)
                        {
                            vertices.Add(startTile.BottomLeftVertex);
                            vertices.Add(startVertex);
                            vertices.Add(startTile.TopRightVertex);
                        }
                        else if (startVertex == startTile.TopRightVertex)
                        {
                            vertices.Add(startTile.TopLeftVertex);
                            vertices.Add(startVertex);
                            vertices.Add(startTile.BottomRightVertex);
                        }
                        else if (startVertex == startTile.BottomRightVertex)
                        {
                            vertices.Add(startTile.TopRightVertex);
                            vertices.Add(startVertex);
                            vertices.Add(startTile.BottomLeftVertex);
                        }

                        //tileVertices.Add(startTile.Vertices[0, 0]);
                        //tileVertices.Add(startTile.Vertices[0, 1]);
                        //tileVertices.Add(startTile.Vertices[1, 1]);
                        //tileVertices.Add(startTile.Vertices[1, 0]);
                        tileVertices = vertices;
                        tiles.Add(startTile);
                        return tiles;
                    }
                default:
                {

                    for (var x = startTile.TileMapDataIndex.x; x < startTile.TileMapDataIndex.x + SelectionBrushSize - 2; x++)
                    {
                        for (var z = startTile.TileMapDataIndex.y; z < startTile.TileMapDataIndex.y + SelectionBrushSize - 2; z++)
                        {
                            var tile = TileMapManager.Instance.ActiveMapData.TileMap[x, z];
                            tiles.Add(tile);
                            tileVertices.Add(tile.Vertices[0, 0]);
                            tileVertices.Add(tile.Vertices[0, 1]);
                            tileVertices.Add(tile.Vertices[1, 1]);
                            tileVertices.Add(tile.Vertices[1, 0]);
                            vertices.Add(tile.Vertices[0, 0]);
                            vertices.Add(tile.Vertices[0, 1]);
                            vertices.Add(tile.Vertices[1, 1]);
                            vertices.Add(tile.Vertices[1, 0]);
                        }
                    }

                    return tiles;
                    }
            }


        }

        /// <summary>
        /// Finds tile at mouse cursor.
        /// </summary>
        private void GetSelectedTile()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            // Create layer mask to only hit layer 11 (Terrain)
            LayerMask layerMask = 1 << 11;
            if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, layerMask))
            {
                var x = hitInfo.point.x;
                var y = hitInfo.point.y;
                var z = hitInfo.point.z;

                if (x < .5 || x > TileMapManager.Instance.ActiveMapData.WidthX - .5)
                {
                    return;
                }

                if (z < .5 || z > TileMapManager.Instance.ActiveMapData.LengthZ - .5)
                {
                    return;
                }

                var newTileIndex = new Vector2Int(Mathf.RoundToInt((x - .25f) * 2),
                    Mathf.RoundToInt((z - .25f) * 2));
                var newVertex = new Vector3
                (
                    Mathf.RoundToInt(x * 2) / 2f,
                    Mathf.RoundToInt(y * 4) / 4f,
                    Mathf.RoundToInt(z * 2) / 2f
                    );
                var newTilePos = TileMapManager.Instance.GetTile(newTileIndex).BottomLeftVertex;
                SetCurrentTileInfo(newTileIndex, newTilePos, newVertex);

                CheckIfOverVertex(hitInfo.point);
            }

        }

        /// <summary>
        /// Returns the asset at the cursor, if any.
        /// </summary>
        public AssetIDComponent GetSelectedAsset()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            // Create layer mask to only hit layer 11 (Terrain)
            LayerMask layerMask = 1 << 11;
            layerMask |= 1 << 5;
            // Reverse it to ignore the mask.
            layerMask = ~layerMask;
            if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, layerMask))
            {
                Debug.Log($"Hit! {hitInfo.collider.gameObject.name}");
                if (hitInfo.collider.gameObject.GetComponent<AssetIDComponent>())
                {
                    return hitInfo.collider.gameObject.GetComponent<AssetIDComponent>();
                }
            }

            return null;
        }

        /// <summary>
        /// Sets current tile info if it has changed, and triggers selection mesh update if relevant.
        /// </summary>
        /// <param name="newTileIndex"></param>
        /// <param name="newVertex"></param>
        private void SetCurrentTileInfo(Vector2Int newTileIndex, Vector3 newTilePosition, Vector3 newVertex)
        {

            if (CurrentVertex == newVertex) return;
            CurrentVertex = newVertex;
            CurrentTileIndex = newTileIndex;
            CurrentTilePosition = newTilePosition;
            OnCurrentTileChanged();

            if (!SelectionMeshEnabled) return;
            UpdateSelectionMesh();

        }

        public delegate void CurrentTileChangedEventHandler();

        public event CurrentTileChangedEventHandler CurrentTileChanged;

        public virtual void OnCurrentTileChanged()
        {
            CurrentTileChanged?.Invoke();
        }

        public delegate void DragSelectStartEventHandler();

        public event DragSelectStartEventHandler DragSelectStart;

        public virtual void OnDragSelectStart()
        {
            DragSelectStart?.Invoke();
        }

        public delegate void DragSelectEndEventHandler(DraggingEventArgs e);

        public event DragSelectEndEventHandler DragSelectEnd;

        public virtual void OnDragSelectEnd(DraggingEventArgs e)
        {
            DragSelectEnd?.Invoke(e);
        }

        public class DraggingEventArgs : EventArgs
        {

            public TileSelectionData TileSelectionData;
            public Tile StartTile;
            public Tile EndTile;
            public Vector3 StartVertexPosition;
            public Vector3 EndVertexPosition;
            public int StartVertexTerrain;
            public int EndVertexTerrain;

            public bool AlternateFunctionEnabled = false;

            public DraggingEventArgs(Vector2Int startTileIndex, Vector2Int endTileIndex,
                Vector3 startVertexPosition, Vector3 endVertexPosition,
                int startVertexTerrain, int endVertexTerrain,
                bool alternateFunction = false)
            {
                AlternateFunctionEnabled = alternateFunction;
                StartTile = TileMapManager.Instance.ActiveMapData.TileMap[startTileIndex.x, startTileIndex.y];
                EndTile = TileMapManager.Instance.ActiveMapData.TileMap[endTileIndex.x, endTileIndex.y];
                StartVertexPosition = startVertexPosition;
                EndVertexPosition = endVertexPosition;
                StartVertexTerrain = startVertexTerrain;
                EndVertexTerrain = endVertexTerrain;

                TileSelectionData = new TileSelectionData
                {
                    Tiles = GetDragSelectionTiles(StartTile, EndTile, out var tileVertices),
                    TileVertices = tileVertices,
                    Vertices = GetDragSelectionVertices(startVertexPosition, endVertexPosition)
                };
            }

            public DraggingEventArgs(Tile startTile, Tile endTile,
                Vector3 startVertexPosition, Vector3 endVertexPosition,
                int startVertexTerrain, int endVertexTerrain,
                bool alternateFunction = false)
            {
                AlternateFunctionEnabled = alternateFunction;
                StartTile = startTile;
                EndTile = endTile;
                StartVertexPosition = startVertexPosition;
                EndVertexPosition = endVertexPosition;
                StartVertexTerrain = startVertexTerrain;
                EndVertexTerrain = endVertexTerrain;
                TileSelectionData = new TileSelectionData
                {
                    Tiles = GetDragSelectionTiles(StartTile, EndTile, out var tileVertices),
                    TileVertices = tileVertices,
                    Vertices = GetDragSelectionVertices(startVertexPosition, endVertexPosition)
                };
            }
        }

        private void CheckIfOverVertex(Vector3 mousePos)
        {
            mousePos.x *= 2;
            mousePos.z *= 2;
            var normalizedX = (mousePos.x - Math.Truncate(mousePos.x));
            var normalizedZ = (mousePos.z - Math.Truncate(mousePos.z));

            if (normalizedX < .3f || normalizedX > .7f || normalizedZ < .3f || normalizedZ > .7f)
            {
                OverVertex = true;
                return;
            }

            OverVertex = false;
        }

    }
}
