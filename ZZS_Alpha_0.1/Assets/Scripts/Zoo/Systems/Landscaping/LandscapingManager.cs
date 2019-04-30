using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using UnityEngine;
using Zoo.Systems.World;
using Zoo.UI;
using Zoo.Utilities.Rendering.TileMap;
using Input = UnityEngine.Input;

namespace Zoo.Systems.Landscaping
{

    public enum TerrainType
    {
        Cliff,
        Grass,
        Mud,
        Dirt,
        Sand
    };

    public class LandscapingManager : SystemManager<LandscapingManager>
    {

        private bool _isDragSelecting = false;

        #region Boolean Toggles

        /// <summary>
        /// When enabled, landscaping tools are available.
        /// </summary>
        public bool LandscapingToolsEnabled { get; private set; }

        /// <summary>
        /// When enabled, terrain modification tools are available.
        /// </summary>
        public bool TerrainModificationEnabled { get; private set; }

        /// <summary>
        /// When enabled, terrain painting is enabled.
        /// </summary>
        public bool TerrainPaintingEnabled { get; private set; }

        /// <summary>
        /// Toggles Landscaping Tools, setting SelectionMesh to match.
        /// </summary>
        public void ToggleLandscapingTools()
        {
            if (LandscapingToolsEnabled)
            {
                LandscapingToolsEnabled = false;
                DisableEvents();
                MapInteractionManager.Instance.DisableSelectionMesh();
                TerrainModificationEnabled = false;
                TerrainPaintingEnabled = false;
            }
            else
            {
                EnableLandscapingTools();
            }
        }

        /// <summary>
        /// Enables landscaping tools & selection mesh.
        /// </summary>
        public void EnableLandscapingTools()
        {
            LandscapingToolsEnabled = true;
            // Enable terrain modification by default
            TerrainModificationEnabled = true;
            RegisterEvents();
            MapInteractionManager.Instance.EnableSelectionMesh();
        }

        /// <summary>
        /// Toggles Terrain Modification, and selection mesh, if relevant.
        /// </summary>
        public void ToggleTerrainModification()
        {
            if (TerrainModificationEnabled)
            {
                TerrainModificationEnabled = false;
                if (!TerrainPaintingEnabled)
                    MapInteractionManager.Instance.DisableSelectionMesh();
            }
            else
            {
                TerrainModificationEnabled = true;
                MapInteractionManager.Instance.EnableSelectionMesh();
            }
        }

        /// <summary>
        /// Toggles Terrain Painting, and selection mesh, if relevant.
        /// </summary>
        public void ToggleTerrainPainting()
        {
            if (TerrainPaintingEnabled)
            {
                TerrainPaintingEnabled = false;
                if(!TerrainModificationEnabled)
                MapInteractionManager.Instance.DisableSelectionMesh();
            }
            else
            {
                TerrainPaintingEnabled = true;
                MapInteractionManager.Instance.EnableSelectionMesh();
            }
        }

        #endregion

        /// <summary>
        /// Index of terrain type to paint.
        /// </summary>
        public int TerrainToPaint { get; private set; } = (int)TerrainType.Sand;

        /// <summary>
        /// Sets terrain painting type to given terrain index.
        /// </summary>
        /// <param name="newTerrainTypeIndex"></param>
        public void SetTerrainToPaint(int newTerrainTypeIndex)
        {
            TerrainToPaint = newTerrainTypeIndex;
        }


        // Start is called before the first frame update
        void Start()
        {
            Initialize();

        }

        public void Initialize()
        {
        }

        // Update is called once per frame
        void Update()
        {
            if(LandscapingToolsEnabled)
                HandleLandscapingInput();

            if (Input.GetKeyDown(KeyCode.P))
            {
                ToggleTerrainPainting();
                Debug.Log("What");
            }

            if (Input.GetMouseButtonDown(2))
            {
                Debug.Log($"Current tile terrain types: {MapInteractionManager.Instance.GetCurrentTile().GetVertexTerrainTypes()}");
            }
        }

        private void RegisterEvents()
        {
            MapInteractionManager.Instance.DragSelectStart += HandleDragSelectStartEvent;
            MapInteractionManager.Instance.DragSelectEnd += HandleDragSelectEndEvent;
        }

        private void DisableEvents()
        {
            MapInteractionManager.Instance.DragSelectStart -= HandleDragSelectStartEvent;
            MapInteractionManager.Instance.DragSelectEnd -= HandleDragSelectEndEvent;
        }

        /// <summary>
        /// Cycles up or down through terrain types based on boolean.
        /// </summary>
        /// <param name="increaseIndex"></param>
        private void CycleTerrainTypeIndex(bool increaseIndex = true)
        {
            if (increaseIndex)
            {
                if (TerrainToPaint >= 4)
                {
                    TerrainToPaint = 1;
                }
                else
                {
                    TerrainToPaint++;
                }
            }
            else
            {
                if (TerrainToPaint <= 1)
                {
                    TerrainToPaint = 4;
                }
                else
                {
                    TerrainToPaint--;
                }
                
            }
        }

        private void HandleLandscapingInput()
        {

            if (TerrainPaintingEnabled)
            {
                if (Input.GetKeyDown(KeyCode.Equals))
                {
                    CycleTerrainTypeIndex();
                }
                else if (Input.GetKeyDown(KeyCode.Minus))
                {
                    CycleTerrainTypeIndex(false);
                }
            }

            if (_isDragSelecting)
            {
                return;
            }



            if (TerrainModificationEnabled || TerrainPaintingEnabled)
            {

                if (Input.GetMouseButtonUp(0))
                {
                    var actionParams = GetLandscapingActionParameters();
                    if (TerrainModificationEnabled)
                        new RaiseTerrain(actionParams).ExecuteAction();
                    else if (TerrainPaintingEnabled)
                    {
                        new PaintTerrain(actionParams).ExecuteAction();
                    }
                }
                else if (Input.GetMouseButtonUp(1))
                {
                    var actionParams = GetLandscapingActionParameters();
                    if (TerrainModificationEnabled)
                        new LowerTerrain(actionParams).ExecuteAction();
                    else if (TerrainPaintingEnabled)
                    {
                        new PaintTerrain(actionParams).ExecuteAction();
                    }
                }
                else if (TerrainPaintingEnabled && Input.GetMouseButtonDown(2))
                {
                    var actionParams = GetLandscapingActionParameters();
                    new PaintTerrain(actionParams).ExecuteAction();
                }
            }


        }

        private void HandleDragSelectStartEvent()
        {
            _isDragSelecting = true;
            Debug.Log("Started dragging");
        }

        private void HandleDragSelectEndEvent(MapInteractionManager.DraggingEventArgs e)
        {
            var actionParams = GetLandscapingActionParameters();
            if (e.AlternateFunctionEnabled)
            {
                actionParams.NewTerrainType = (int)e.StartTile.BottomLeftVertexTerrain;
            }
            if(!TerrainModificationEnabled)
            {
                new PaintTerrain(actionParams).ExecuteAction();
            }
            else
            {
                new LevelTerrain(actionParams,
                    e.StartTile.GetHeight()).ExecuteAction();
            }

            _isDragSelecting = false;
            Debug.Log("Stopped dragging");
        }

        /// <summary>
        /// Returns new LandscapingActionParameters based on current tile selection & terrain painting data.
        /// </summary>
        /// <returns></returns>
        private LandscapingActionParameters GetLandscapingActionParameters()
        {
            try
            {
                return new LandscapingActionParameters(MapInteractionManager.Instance.CurrentTileSelection.Tiles,
                    MapInteractionManager.Instance.CurrentTileSelection.Vertices,
                    TerrainPaintingEnabled, TerrainToPaint);
            }
            catch
            {
                return new LandscapingActionParameters(new List<Tile>(), new List<Vector3>());
            }

        }
    }
    

}
