using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zoo.Systems;
using UnityEngine;
using Zoo.IO.AssetBundles;

namespace Zoo.Assets
{
    public class AssetManager : SystemManager<AssetManager>
    {
        /// <summary>
        /// Global list of all metadata tags.
        /// </summary>
        public static List<string> AllMetadataTags {get; set;} = new List<string>();

        /// <summary>
        /// Returns next available AssetID.
        /// </summary>
        /// <returns></returns>
        public static int GetAssetID()
        {
            return GameManager.Instance.CurrentSavedGameInfo.NextUniqueID;
        }

        /// <summary>
        /// Returns GameAssets with given tags. Defaults to exclusive (only assets with all tags).
        /// </summary>
        /// <param name="metadataTags"></param>
        /// <param name="exclusive">When true, only assets with all tags will be returned. When false,
        /// assets with any of the tags will be returned.</param>
        /// <returns></returns>
        public static List<GameAsset> GetAssetsWithTags(List<string> metadataTags, bool exclusive = true)
        {
            if(exclusive)
            { return AssetBundleHelper.LoadedGameAssets.Values
                .Where(a => metadataTags.TrueForAll(a.MetadataTags.Contains)).ToList();

            }
            else
            {
               return AssetBundleHelper.LoadedGameAssets.Values
                    .Where(a => metadataTags.Any(a.MetadataTags.Contains)).ToList();
            }

        }

        /// <summary>
        /// Returns Asset of given ID type.
        /// </summary>
        /// <param name="assetTypeID"></param>
        /// <returns></returns>
        public static GameAsset GetAssetByStringID(string assetTypeID)
        {
            try
            {
                return AssetBundleHelper.LoadedGameAssets[assetTypeID];
            }
            catch
            {
                Debug.Log("Asset not found!");
                return null;
            }
        }

        public static void PopulateAssetInfo(List<GameAssetJSONData> assetInfo)
        {
            foreach (var data in assetInfo)
            {
                GameAsset asset;
                try
                {
                    asset = AssetBundleHelper.LoadedGameAssets[data.AssetStringID];
                }
                catch
                {
                    Debug.Log($"Failed to load {data.AssetStringID}.");
                    continue;
                }

                if (data.MetadataTags.Count > 0)
                {
                    asset.MetadataTags = data.MetadataTags;
                    AllMetadataTags.AddRange(data.MetadataTags.Where(t => !AllMetadataTags.Contains(t))
                    );
                }
                if (data.DynamicAttributes.Count > 0)
                {
                    asset.DynamicAttributeIDs = data.DynamicAttributes;
                }
                if (asset.InGameName != null)
                {
                    var lowerCaseName = asset.InGameName.ToLower();
                    asset.MetadataTags.Add(lowerCaseName);
                    if(!AllMetadataTags.Contains(lowerCaseName))
                    AllMetadataTags.Add(lowerCaseName);
                }
            }
        }

        /// <summary>
        /// Create and initialise new asset, returning a reference to its Data object.
        /// Asset GameObject will be instantiated at the given transform, or
        /// default to 0,0,0.
        /// </summary>
        /// <param name="assetTypeID"></param>
        /// <param name="assetTransform"></param>
        /// <param name="assetRotation"></param>
        public static void CreateNewAsset(string assetTypeID, 
            Vector3 assetTransform = new Vector3(), Quaternion assetRotation = new Quaternion())
        {
            // Get the asset
            var asset = GetAssetByStringID(assetTypeID);
            // Create the game object
            var assetGameObject = Instantiate(asset.AssetPrefab, assetTransform, assetRotation);
            // Create the data class and assign gameObject reference to the data class.
            if (Activator.CreateInstance(asset.DataType) is GameAssetData data) data.GameObject = assetGameObject;
            else
            {
                // Break if we fail
                Debug.Log("Failed to instantiate asset.");
                return;
            }

            // Initialize GameData
            // data.Position = assetTransform;
            // data.RotationEuler = assetRotation.eulerAngles;
            data.Initialize(asset);

            // Fire event 
            OnAssetCreated(new AssetCreatedEventArgs(data, asset, assetGameObject));

        }

        /// <summary>
        /// Create and initialise new asset hologram.
        /// Asset GameObject will be instantiated at the given transform, or
        /// default to 0,0,0.
        /// Will NOT be assigned an ID, nor perform normal initialization.
        /// Should only be used to preview asset purchases, and
        /// be fed to appropriate BuildAction to finalize placement.
        /// </summary>
        /// <param name="assetTypeID"></param>
        /// <param name="assetTransform"></param>
        /// <param name="assetRotation"></param>
        public static void CreateNewAssetHologram(string assetTypeID,
            Vector3 assetTransform = new Vector3(), Quaternion assetRotation = new Quaternion())
        {
            // Get the asset
            var asset = GetAssetByStringID(assetTypeID);
            // Create the game object
            var assetGameObject = Instantiate(asset.AssetPrefab, assetTransform, assetRotation);
            // Create the data class and assign gameObject reference to the data class.
            if (Activator.CreateInstance(asset.DataType) is GameAssetData data) data.GameObject = assetGameObject;
            else
            {
                // Break if we fail
                Debug.Log("Failed to instantiate asset hologram.");
                return;
            }

            // Initialize GameData
            data.InitializeHologram(asset);
            // Fire event 
            OnAssetHologramCreated(new AssetCreatedEventArgs(data, asset, assetGameObject));

        }

        /// <summary>
        /// Destroy the given asset, performing all necessary cleanup tasks.
        /// </summary>
        /// <param name="asset"></param>
        public static void DestroyAsset(GameAssetContainer asset)
        {
            // Delete stuff
            // Call Destroy method on asset data
            asset.Data.Destroy();
            // Destroy the game object
            Destroy(asset.GameObject);
            // Null the reference so it fucks RIGHT off entirely (precaution)
            asset.GameObject = null;
            // De-register the asset
            GameManager.Instance.RemoveAssetFromDictionary(asset.Data.AssetID);
            // Null the data so it also dies
            asset.Data = null;
        }

        /// <summary>
        /// Destroy the given asset hologram, performing all necessary cleanup tasks.
        /// </summary>
        /// <param name="asset"></param>
        public static void DestroyAssetHologram(GameAssetContainer asset)
        {
            // Delete stuff
            // Call Destroy method on asset data
            asset.Data.Destroy();
            // Destroy the game object
            Destroy(asset.GameObject);
            // Null the reference so it fucks RIGHT off entirely (precaution)
            asset.GameObject = null;

            // Null the data so it also dies
            asset.Data = null;
        }

        public delegate void AssetHologramCreatedEventHandler(AssetCreatedEventArgs e);

        /// <summary>
        /// Invoked when a new asset is created.
        /// </summary>
        public static event AssetHologramCreatedEventHandler AssetHologramCreatedEvent;


        public static void OnAssetHologramCreated(AssetCreatedEventArgs e)
        {
            AssetHologramCreatedEvent?.Invoke(e);
        }

        public delegate void AssetCreatedEventHandler(AssetCreatedEventArgs e);

        /// <summary>
        /// Invoked when a new asset is created.
        /// </summary>
        public static event AssetCreatedEventHandler AssetCreatedEvent;


        public static void OnAssetCreated(AssetCreatedEventArgs e)
        {
            AssetCreatedEvent?.Invoke(e);
        }

        public class AssetCreatedEventArgs : EventArgs
        {
            public GameAssetData CreatedData => Container.Data;
            public GameAsset CreatedAssetType => Container.Type;
            public GameObject CreatedAssetGameObject => Container.GameObject;
            public GameAssetContainer Container;

            public AssetCreatedEventArgs(GameAssetData data, GameAsset asset, GameObject gameObject)
            {
                Container = new GameAssetContainer(data, asset, gameObject);
            }

            public AssetCreatedEventArgs(GameAssetContainer assetContainer)
            {
                Container = assetContainer;
            }
        }

        /// <summary>
        /// Performs necessary tasks to load asset from saved data. Specifically
        /// creates the GameObject.
        /// </summary>
        /// <param name="data"></param>
        public static void LoadSavedAsset(GameAssetData data)
        {
            // Get the asset
            var asset = GetAssetByStringID(data.AssetTypeID);
            // Create the game object
            var assetGameObject = Instantiate(asset.AssetPrefab, data.Position, 
                Quaternion.Euler(data.RotationEuler).normalized);
            data.GameObject = assetGameObject;
            // Initialize GameData from save
            data.InitializeFromSave();
        }
    }

    /// <summary>
    /// Class holding a fully-realized GameAsset. Intended to facilitate Construction Actions,
    /// and any other scenario that requires storing references to all 3 aspects of an instantiated GameAsset.
    /// This class is NOT serializable.
    /// </summary>
    public class GameAssetContainer
    {
        public GameAssetData Data;
        public GameAsset Type;
        public GameObject GameObject;

        /// <summary>
        /// Creates a container with only type information. Intended as the 'shell'
        /// to initiate certain game actions with.
        /// </summary>
        public GameAssetContainer(GameAsset assetType)
        {
            Type = assetType;
        }

        public GameAssetContainer(GameAssetData data, GameAsset assetType, GameObject assetGameObject)
        {
            Data = data;
            Type = assetType;
            GameObject = assetGameObject;
        }

        /// <summary>
        /// Creates a container with the given data & GO. Type is assigned in constructor.
        /// No more stupid long calls to get the Type. You're welcome.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="assetGameObject"></param>
        public GameAssetContainer(GameAssetData data, GameObject assetGameObject)
        {
            Data = data;
            Type = AssetManager.GetAssetByStringID(data.AssetTypeID);
            GameObject = assetGameObject;
        }

    }

}
