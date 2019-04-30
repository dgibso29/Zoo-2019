using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zoo.Assets
{
    /// <summary>
    /// Container for data imported from asset bundle info JSONs ('zzs_core_assetinfo')
    /// </summary>
    public class GameAssetJSONData
    {

        /// <summary>
        /// String ID of associated GameAsset
        /// </summary>
        public string AssetStringID;

        /// <summary>
        /// List of metadata tags for this asset
        /// </summary>
        public List<string> MetadataTags;

        /// <summary>
        /// List of dynamic attributes for this asset
        /// </summary>
        public List<string> DynamicAttributes;

    }
}
