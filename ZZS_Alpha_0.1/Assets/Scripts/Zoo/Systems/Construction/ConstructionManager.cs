using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.XR.WSA;
using Zoo.Assets;
using Zoo.Assets.Objects.Paths;
using Zoo.Systems.World;
using Zoo.UI;

namespace Zoo.Systems.Construction
{
    public class ConstructionManager : SystemManager<ConstructionManager>
    {

        private bool testFunctionsEnabled = false;

        public Material HologramMat;

        /// <summary>
        /// When true, purchasing has been initiated.
        /// </summary>
        public bool IsPurchasing
        {
            get => _purchasingEnabled;
            private set
            {
                if (value == false)
                {
                    DisablePurchasingEvents();
                }
                else
                {
                    RegisterPurchasingEvents();
                }

                _purchasingEnabled = value;
            }
        }

        /// <summary>
        /// Current rotation for object being placed. Resets to 0 when object type changes.
        /// </summary>
        private Quaternion _currentRotation = new Quaternion();

        private bool _purchasingEnabled = false;

        /// <summary>
        /// If true, only constructing a single object. When false,
        /// multiple object handling will be used.
        /// </summary>
        private bool _singleObject;

        /// <summary>
        /// When true, player is attempting to place a blueprint.
        /// </summary>
        private bool _purchasingBlueprint = false;

        /// <summary>
        /// Key of the currently/last used asset type.
        /// </summary>
        public string CurrentAssetTypeKey { get; private set; }

        /// <summary>
        /// List of assets being placed if a blueprint is being purchased.
        /// </summary>
        private List<GameAssetData> _currentBlueprint = new List<GameAssetData>();

        /// <summary>
        /// List of currently displayed 'hologram' assets.
        /// Will be deleted if building is cancelled,
        /// Or properly built if confirmed.
        /// </summary>
        public List<GameAssetData> HologramAssets = new List<GameAssetData>();

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                testFunctionsEnabled = !testFunctionsEnabled;
            }

            if (testFunctionsEnabled)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    var newPos = MapInteractionManager.Instance.CurrentTilePosition;
                    //AssetManager.CreateNewAsset("scenery_block_test_large_01", newPos);
                    var posList = new List<Vector3>()
                    {
                        newPos,
                    };
                    var rotList = new List<Quaternion>(new Quaternion[1]);
                    var type = AssetManager.GetAssetByStringID("path_test_01");
                    //posList[0] += new Vector3(type.AssetSize.x / 2, 0, type.AssetSize.z /2);
                    var containers = new List<GameAssetContainer>()
                    {
                        new GameAssetContainer(type)
                    };
                    var actionParams = new ConstructionActionParameters(containers);
                    new BuildAction(actionParams, posList, rotList).ExecuteAction();
                    Debug.Log($"Placed path on tile {newPos}");
                }

                if (Input.GetMouseButtonDown(2))
                {
                    foreach (var keyValuePair in GameManager.Instance.GameAssetDirectory.Where(i => i.Value.AssetTypeID.Contains("path")))
                    {
                        ((PathData)keyValuePair.Value).PathComponent.UpdatePath();
                    }
                }
            }


            if (IsPurchasing)
            {
                HandlePurchasingInput();
            }


        }

        public void UpdateAllPaths()
        {

        }

        private void OnCurrentTileChanged()
        {
            UpdateHolograms();
        }

        private void RegisterPurchasingEvents()
        {
            MapInteractionManager.Instance.CurrentTileChanged += OnCurrentTileChanged;

        }

        private void DisablePurchasingEvents()
        {
            MapInteractionManager.Instance.CurrentTileChanged -= OnCurrentTileChanged;

        }

        /// <summary>
        /// Call in update function while IsPurchasing is true.
        /// </summary>
        private void HandlePurchasingInput()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log($"Completing purchase of {HologramAssets.Count} assets.");
                CompletePurchase();
            }
            if (Input.GetMouseButtonDown(1))
            {
                Debug.Log($"Cancelling purchase.");
                CancelPurchase();
            }

            if (Input.GetKeyDown(KeyCode.Z))
            {
                RotateClockwise();
            }
        }

        /// <summary>
        /// Set rotation 90 degrees clockwise.
        /// </summary>
        private void RotateClockwise()
        {
            _currentRotation.eulerAngles += new Vector3(0, 90f, 0);
            UpdateHolograms();
        }

        /// <summary>
        /// Set rotation 90 degrees counter-clockwise.
        /// </summary>
        private void RotateCounterClockwise()
        {
            _currentRotation.eulerAngles -= new Vector3(0, 90f, 0);
            UpdateHolograms();

        }

        private void ResetRotation()
        {
            _currentRotation = new Quaternion();
        }

        /// <summary>
        /// Initiate process of purchasing given asset.
        /// </summary>
        /// <param name="assetKeyToBuild"></param>
        public void InitiatePurchase(string assetKeyToBuild)
        {
            // If not purchasing
            if (!IsPurchasing)
            {
                IsPurchasing = true;
            }
            // If player was previously placing a blueprint..
            if (_purchasingBlueprint)
            {
                // Stop Blueprint purchase
                _purchasingBlueprint = false;
            }

            if (CurrentAssetTypeKey != assetKeyToBuild)
            {
                ResetRotation();
            }

            _singleObject = true;
            CurrentAssetTypeKey = assetKeyToBuild;
            UpdateHolograms();
            // Should be done?
        }

        /// <summary>
        /// Initiate process of purchasing the given blueprint (list of
        /// GameAssetData objects).
        /// </summary>
        public void InitiateBlueprintPurchase(List<GameAssetData> blueprintObjects)
        {
            // If not purchasing
            if (!IsPurchasing)
            {
                IsPurchasing = true;
                _purchasingBlueprint = true;
                // Do shit
                return;
            }
            if (!_purchasingBlueprint)
            {
                _purchasingBlueprint = true;
            }

            _currentBlueprint = blueprintObjects;
        }

        /// <summary>
        /// Will cancel purchase, if initiated.
        /// </summary>
        public void CancelPurchase()
        {
            // Do shit
            IsPurchasing = false;
            DeleteHolograms();
            ResetRotation();
        }

        /// <summary>
        /// Updates hologram assets to reflect current asset type &
        /// build parameters (position, etc). If asset type changed,
        /// will purge holograms and replace with new type.
        /// </summary>
        private void UpdateHolograms()
        {
            // Delete previous holograms...
            DeleteHolograms();

            // Handle single object
            if (_singleObject)
            {
                // Create the hologram
                var hologram = CreateHologram(CurrentAssetTypeKey,
                    MapInteractionManager.Instance.CurrentTilePosition, _currentRotation);
                // Update its position
                var newPos = MapInteractionManager.Instance.CurrentTilePosition;
                var assetSize = hologram.AssetType.AssetSize;
                // TODO: Make this its own (static?) method to set asset position
                // Offset placement by size
                //newPos.x += assetSize.x / 2;
                //newPos.z += assetSize.z / 2;
                // May need to account for height here..?
                //newPos.y += assetSize.y / 2;
                hologram.Position = newPos;
                hologram.GameObject.GetComponentInChildren<MeshRenderer>().material = HologramMat;
                // Add it to the list of holograms
                HologramAssets.Add(hologram);
            }
            // Handle multiple objects
            else if(!_purchasingBlueprint)
            {
                
            }
            // Handle blueprint
            else if (_purchasingBlueprint)
            {

            }

        }

        /// <summary>
        /// Will create and return a new hologram of the given type
        /// at the given position and rotation.
        /// </summary>
        private static GameAssetData CreateHologram(string assetTypeKey,
            Vector3 position, Quaternion rotation = new Quaternion())
        {
            GameAssetData newHologram = null;
            // Local function that will add the given hologram to the list.
            void AssetHologramCreatedHandler(AssetManager.AssetCreatedEventArgs e)
            {
                newHologram = e.CreatedData;
            }

            // Register the function to to the event
            AssetManager.AssetHologramCreatedEvent += AssetHologramCreatedHandler;

            // Create the asset with the given parameters, which will raise the event
            // and call AssetCreatedHandler.
            AssetManager.CreateNewAssetHologram(assetTypeKey,
                position, rotation);


            // De-register the function to to the event so it doesn't break things
            // when we do this again. (Otherwise this instance of the
            // local handler would still be called!)
            AssetManager.AssetCreatedEvent -= AssetHologramCreatedHandler;

            return newHologram;
        }

        /// <summary>
        /// Deletes all currently rendered holograms.
        /// </summary>
        private void DeleteHolograms()
        {
            foreach (var hologram in HologramAssets)
            {
                var container = new GameAssetContainer(hologram, hologram.GameObject);
                AssetManager.DestroyAssetHologram(container);
            }
            HologramAssets = new List<GameAssetData>();
        }

        /// <summary>
        /// Fully builds currently rendered holograms.
        /// </summary>
        private void CompletePurchase()
        {

            // Separate function?
            // TODO: Handle clearance checking
            var posList = new List<Vector3>();
            var rotList = new List<Quaternion>(new Quaternion[1]);
            var containers = new List<GameAssetContainer>();
            foreach (var hologram in HologramAssets)
            {
                posList.Add(hologram.Position);
                rotList.Add(hologram.Rotation);
                var type = hologram.AssetType;
                //var type = AssetManager.GetAssetByStringID("path_test_01");
                containers.Add(new GameAssetContainer(type));
            }
            var actionParams = new ConstructionActionParameters(containers);
            new BuildAction(actionParams, posList, rotList).ExecuteAction();
        }

    }
}
