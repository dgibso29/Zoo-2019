using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Zoo.Assets;
using Zoo.Systems;
using Object = UnityEngine.Object;

namespace Zoo.Systems.Construction
{
    public abstract class ConstructionAction : GameAction<ConstructionActionParameters, UndoGameActionParameters>
    {
        internal ConstructionAction(ConstructionActionParameters parameters) : base(parameters)
        {

        }

        /// <summary>
        /// Builds the assets in Parameters.AssetsToModify with the other info in parameters.
        /// </summary>
        public void BuildsAssets()
        {
            // Declare this here so it will be valid for the local function.
            var index = 0;

            // Local function that will replace the current Asset container with
            // the created one, once it is created.
            void AssetCreatedHandler(AssetManager.AssetCreatedEventArgs e)
            {
                Parameters.AssetsToModify[index] = e.Container;
            }

            // Register the function to to the event
            AssetManager.AssetCreatedEvent += AssetCreatedHandler;

            for (index = 0; index < Parameters.AssetsToModify.Count; index++)
            {
                // Create the asset with the given parameters, which will raise the event
                // and call AssetCreatedHandler.
                AssetManager.CreateNewAsset(Parameters.AssetsToModify[index].Type.AssetStringID,
                    Parameters.NewPositions[index], Parameters.NewRotations[index]);

                // Presumably we have to sacrifice a goat or something now

            }

            // De-register the function to to the event so it doesn't break things
            // when we do this again. (Otherwise this instance of the
            // local handler would still be called!)
            AssetManager.AssetCreatedEvent -= AssetCreatedHandler;
        }

    }

    public class ConstructionActionParameters : GameActionParameters
    {

        /// <summary>
        /// Container for each asset modified by this action.
        /// </summary>
        internal List<GameAssetContainer> AssetsToModify;

        internal List<Vector3> NewPositions;
        internal List<Quaternion> NewRotations;

        internal List<Vector3> OldPositions;
        internal List<Quaternion> OldRotations;



        public ConstructionActionParameters(List<GameAssetContainer> assetsToModify)
        {
            AssetsToModify = assetsToModify;
        }
        
    }

    internal class BuildAction : ConstructionAction
    {
        public BuildAction(ConstructionActionParameters parameters, 
            List<Vector3> positions, List<Quaternion> rotations) : base(parameters)
        {
            Parameters.NewPositions = positions;
            Parameters.NewRotations = rotations;
        }

        protected override void CompleteAction()
        {
           BuildsAssets();
        }

        protected override void UndoAction()
        {
            foreach (var asset in Parameters.AssetsToModify)
            {
                AssetManager.DestroyAsset(asset);
            }
        }

    }
    
    internal class DestroyAction : ConstructionAction
    {
        public DestroyAction(ConstructionActionParameters parameters) : base(parameters)
        {
            Parameters.NewPositions = new List<Vector3>(Parameters.AssetsToModify.Count);
            Parameters.NewRotations = new List<Quaternion>(Parameters.AssetsToModify.Count);
        }

        protected override void CompleteAction()
        {
            
            for (var i = 0; i < Parameters.AssetsToModify.Count; i++)
            {
                var asset = Parameters.AssetsToModify[i];
                // Save the old info to new -- will be used in redo via BuildAssets()
                Parameters.NewPositions.Add(asset.GameObject.transform.position);
                Parameters.NewRotations.Add(asset.GameObject.transform.rotation);

                AssetManager.DestroyAsset(asset);
            }

        }

        protected override void UndoAction()
        {
           BuildsAssets();
        }
    }

    internal class MoveObjectAction : ConstructionAction
    {
        public MoveObjectAction(ConstructionActionParameters parameters,
            List<Vector3> newPositions, List<Quaternion> newRotations) : base(parameters)
        {
            Parameters.NewPositions = newPositions;
            Parameters.NewRotations = newRotations;
            Parameters.OldPositions = new List<Vector3>(newPositions.Count);
            Parameters.OldRotations = new List<Quaternion>(newRotations.Count);
        }

        protected override void CompleteAction()
        {

            for (var i = 0; i < Parameters.AssetsToModify.Count; i++)
            {
                var asset = Parameters.AssetsToModify[i];
                // Save the old info
                Parameters.OldPositions[i] = asset.GameObject.transform.position;
                Parameters.OldRotations[i] = asset.GameObject.transform.rotation;
                // Set it to the new info
                asset.GameObject.transform.position = Parameters.NewPositions[i];
                asset.GameObject.transform.rotation = Parameters.NewRotations[i];
            }
        }

        protected override void UndoAction()
        {
            for (var i = 0; i < Parameters.AssetsToModify.Count; i++)
            {
                var asset = Parameters.AssetsToModify[i];
                // Set transform back to original
                asset.GameObject.transform.position = Parameters.OldPositions[i];
                asset.GameObject.transform.rotation = Parameters.OldRotations[i];
            }
        }
    }

    /// <summary>
    /// Not certain this will need to exist. If you're trying to use it,
    /// double check with me/yourself.
    /// </summary>
    [System.Obsolete]
    internal class BuildPathAction : BuildAction
    {
        public BuildPathAction(ConstructionActionParameters parameters,
            List<Vector3> positions, List<Quaternion> rotations) : base(parameters, positions, rotations)
        {

        }

        protected override void CompleteAction()
        {
            BuildsAssets();
        }

        protected override void UndoAction()
        {
            foreach (var asset in Parameters.AssetsToModify)
            {
                AssetManager.DestroyAsset(asset);
            }
        }
    }
}
