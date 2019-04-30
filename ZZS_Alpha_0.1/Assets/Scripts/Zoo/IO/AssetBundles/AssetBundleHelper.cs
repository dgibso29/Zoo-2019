using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zoo.Assets;
using Zoo.Attributes;

namespace Zoo.IO.AssetBundles
{
    public static class AssetBundleHelper
    {

        static List<AssetBundle> LoadedAssetBundles = new List<AssetBundle>();

        public static Dictionary<string, GameAsset> LoadedGameAssets = new Dictionary<string, GameAsset>();

        /// <summary>
        /// Pretty sure this won't actually be handled here -- 
        /// </summary>
        public static Dictionary<string, Attribute<IAttribute>> LoadedAttributes = new Dictionary<string, Attribute<IAttribute>>();

        /// <summary>
        /// Loads all asset bundles and incorporates their contents as needed.
        /// </summary>
        public static void InitializeAssetBundles()
        {
            LoadAssetBundles();
            LoadBundleData();
        }

        static void LoadAssetBundles()
        {
            List<string> bundlePaths = new List<string>();
            foreach (string path in Directory.GetFiles(Application.streamingAssetsPath))
            {
                // Skip path if it has an extension. So far, assetbundles have no extension.
                if (path.Contains("manifest") || path.Contains("meta"))
                {
                    continue;
                }
                AssetBundle.LoadFromFile(path);
            }
        }

        /// <summary>
        /// Get all tilesets from loaded assetbundles.
        /// </summary>
        /// <returns></returns>
        static void LoadBundleData()
        {
            foreach (AssetBundle bundle in AssetBundle.GetAllLoadedAssetBundles())
            {
                foreach (GameAsset asset in bundle.LoadAllAssets<GameAsset>())
                {
                    if (!LoadedGameAssets.ContainsKey(asset.AssetStringID))
                    {
                        LoadedGameAssets.Add(asset.AssetStringID, asset);
                    }
                    
                }

                var assetInfoName = $"{bundle.name}_assetinfo.json";

                if (bundle.Contains(assetInfoName))
                {

                    var assetInfoJson = bundle.LoadAsset(assetInfoName) as TextAsset;
                    var assetInfo = IOHelper.DeserializeAssetInfo(assetInfoJson);
                    AssetManager.PopulateAssetInfo(assetInfo);
                }
            }
        }
    }
}