using UnityEngine;
using Zoo.AssetComponents;
using Zoo.Assets;

namespace Zoo.AssetComponents
{
    /// <summary>
    /// Holds a reference to the GameObject's asset ID & Data.
    /// </summary>
    public class AssetIDComponent : AssetComponent
    {
        /// <summary>
        /// Unique asset ID of this GameObject.
        /// </summary>
        public int ID;

        public string AssetTypeID => ParentAssetData.AssetTypeID;


        public AssetIDComponent(int uniqueAssetID, GameAssetData assetData)
        {
            ID = uniqueAssetID;
            ParentAssetData = assetData;
        }

    }
}
