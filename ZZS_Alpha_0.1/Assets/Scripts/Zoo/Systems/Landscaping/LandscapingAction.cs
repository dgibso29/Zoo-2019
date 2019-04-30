using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using Zoo.Systems.World;
using Zoo.UI;
using Zoo.Utilities.Rendering.TileMap;
using Debug = UnityEngine.Debug;

namespace Zoo.Systems.Landscaping
{
    public abstract class LandscapingAction : GameAction<LandscapingActionParameters, UndoGameActionParameters>
    {

        internal LandscapingAction(LandscapingActionParameters parameters) : base(parameters)
        {

        }

        protected void PaintTerrainTypes()
        {
            foreach (var t in Parameters.VerticesToModify)
            {
                Parameters.OldTerrainTypes.Add(TileMapManager.Instance.ActiveMapData.GetVertexTerrainType(t));
                TileMapManager.Instance.ActiveMapData.SetWeldedVertexTerrain(t, Parameters.NewTerrainType);
            }
        }

        protected void RevertTerrainTypes()
        {
            for (var i = 0; i < Parameters.VerticesToModify.Count; i++)
            {
                TileMapManager.Instance.ActiveMapData.SetWeldedVertexTerrain(Parameters.VerticesToModify[i],
                    Parameters.OldTerrainTypes[i]);
            }

        }

        protected override bool ValidateAction()
        {
            // Make sure vertices are provided.
            if (Parameters.VerticesToModify.Count < 1)
                return false;
            foreach (var tile in Parameters.TilesToModify)
            {
                // Perform checks
                if (!tile.Playable)
                {
                    // If false, throw error event & return false.
                    return false;
                }
            }

            // If we made it this far, return true.
            return true;
        }

        /// <summary>
        /// Assigns distinct chunks of TilesToModify to ChunksToUpdate
        /// </summary>
        protected void SetChunksToUpdate()
        {
            Parameters.ChunksToUpdate = Parameters.TilesToModify.Select(i => i.TileChunkIndex).Distinct().ToList();
        }

        protected override void ExitAction()
        {
            SetChunksToUpdate();
            TileMapRenderer.RenderChunks(Parameters.ChunksToUpdate);
            //MapInteractionManager.Instance.ManuallyUpdateSelectionMesh();
        }


    }

    public class LandscapingActionParameters : GameActionParameters
    {
        internal List<Tile> TilesToModify;
        internal List<Vector3> VerticesToModify;
        internal List<float> OldHeights;

        internal List<Vector2Int> ChunksToUpdate;

        internal List<int> OldTerrainTypes;
        internal bool PaintTerrain;
        internal int NewTerrainType;

        public LandscapingActionParameters(List<Tile> tilesToModify, List<Vector3> verticesToModify)
        {
            TilesToModify = tilesToModify;
            VerticesToModify = verticesToModify.Distinct().ToList();
        }

        public LandscapingActionParameters(List<Tile> tilesToModify, List<Vector3> verticesToModify, bool paintTerrain, int newTerrain)
        {
            TilesToModify = tilesToModify;
            VerticesToModify = verticesToModify.Distinct().ToList();

            OldTerrainTypes = new List<int>(tilesToModify.Count);
            PaintTerrain = paintTerrain;
            NewTerrainType = newTerrain;
        }

    }

    internal class PaintTerrain : LandscapingAction
    {

        internal PaintTerrain(LandscapingActionParameters parameters) : base(parameters)
        {

        }

        protected override void CompleteAction()
        {
            PaintTerrainTypes();
        }

        protected override void UndoAction()
        {
            RevertTerrainTypes();
        }
    
    }

    internal class RaiseTerrain : LandscapingAction
    {

        internal RaiseTerrain(LandscapingActionParameters parameters) : base(parameters)
        {
            Parameters.OldHeights = new List<float>(Parameters.VerticesToModify.Count);

        }

        protected override void CompleteAction()
        {
            foreach (Vector3 vert in Parameters.VerticesToModify)
            {
                Parameters.OldHeights.Add(vert.y);
                TileMapManager.Instance.ActiveMapData.SetWeldedVertexHeight(vert,
                    vert.y + MapData.HeightIncrement);
            }
            if (Parameters.PaintTerrain)
                PaintTerrainTypes();

        }

        protected override void UndoAction()
        {
            for (var i = 0; i < Parameters.VerticesToModify.Count; i++)
            {
                TileMapManager.Instance.ActiveMapData.SetWeldedVertexHeight(Parameters.VerticesToModify[i],
                    Parameters.OldHeights[i]);
            }

            if (Parameters.PaintTerrain)
                RevertTerrainTypes();

        }

    }


    internal class LowerTerrain : LandscapingAction
    {

        internal LowerTerrain(LandscapingActionParameters parameters) : base(parameters)
        {
            Parameters.OldHeights = new List<float>(Parameters.VerticesToModify.Count);

        }

        protected override void CompleteAction()
        {
            foreach (Vector3 vert in Parameters.VerticesToModify)
            {
                Parameters.OldHeights.Add(vert.y);
                TileMapManager.Instance.ActiveMapData.SetWeldedVertexHeight(vert,
                    vert.y - MapData.HeightIncrement);
            }
            if (Parameters.PaintTerrain)
                PaintTerrainTypes();

        }

        protected override void UndoAction()
        {
            for (var i = 0; i < Parameters.VerticesToModify.Count; i++)
            {
                TileMapManager.Instance.ActiveMapData.SetWeldedVertexHeight(Parameters.VerticesToModify[i],
                    Parameters.OldHeights[i]);
            }

            if (Parameters.PaintTerrain)
                RevertTerrainTypes();

        }

    }

    internal class LevelTerrain : LandscapingAction
    {
        private float _newHeight;
        internal LevelTerrain(LandscapingActionParameters parameters, float newHeight) : base(parameters)
        {
            Parameters.OldHeights = new List<float>(Parameters.VerticesToModify.Count);
            _newHeight = newHeight;
        }

        protected override void CompleteAction()
        {
            foreach (Vector3 vert in Parameters.VerticesToModify)
            {
                Parameters.OldHeights.Add(vert.y);
                TileMapManager.Instance.ActiveMapData.SetWeldedVertexHeight(vert, _newHeight);
            }
            if (Parameters.PaintTerrain)
                PaintTerrainTypes();

        }

        protected override void UndoAction()
        {
            for (var i = 0; i < Parameters.VerticesToModify.Count; i++)
            {
                TileMapManager.Instance.ActiveMapData.SetWeldedVertexHeight(Parameters.VerticesToModify[i],
                    Parameters.OldHeights[i]);
            }

            if (Parameters.PaintTerrain)
                RevertTerrainTypes();

        }

    }


}
