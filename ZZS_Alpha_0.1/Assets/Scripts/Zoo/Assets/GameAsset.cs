using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zoo.Attributes;
using Zoo.IO;

namespace Zoo.Assets
{
    public interface IGameAsset
    {
    }

    public abstract class GameAsset : ScriptableObject, IGameAsset
    {

        /// <summary>
        /// Internal name of asset, IE animal_Lion or staff_Keeper
        /// </summary>
        public string AssetStringID;

        /// <summary>
        /// When false, this asset will be skipped by the icon generation tool.
        /// </summary>
        public bool AllowIconGeneration = true;

        /// <summary>
        /// Prefab GameObject for this asset. Holds model, animations, etc.
        /// </summary>
        public GameObject AssetPrefab;

        public abstract Type DataType { get; }

        public Texture2D AssetUITexture;

        /// <summary>
        /// Size of the asset in world units, anchored at the bottom left.
        /// This should properly reflect the size of the mesh, rounded up to nearest
        /// half for x/z, and nearest quarter for y.
        /// For example: A 1x1x1 cube, covering a 2x2x2 tile area, would be (1,1,1),
        /// while a 0.5x0.5x0.5 cube, covering a 1x1x1 tile area, would be (.5,.5,.5).
        /// </summary>
        public Vector3 AssetSize;

        /// <summary>
        /// Size of the asset in tiles.
        /// For example: A 1x1x1 cube, covering a 2x2 tile area, would be (2,2)
        /// </summary>
        public Vector2Int AssetTileSize;

        /// <summary>
        /// Size of this assets collision box. By default, this equates to the AssetSize.
        /// Can be changed if necessary for special cases.
        /// </summary>
        public virtual Vector3 AssetColliderSize => AssetSize;

        /// <summary>
        /// List of any dynamic attributes to add to assets of this type.
        /// </summary>
        public List<string> DynamicAttributeIDs;

        /// <summary>
        /// List of all metadata tags for this asset. Loaded in from XML/JSON
        /// at application start based on AssetStringID.
        /// </summary>
        public List<string> MetadataTags;

        /// <summary>
        /// English, in-game name of this item. Intended to provided a basis for localization.
        /// </summary>        
        public string InGameName => _inGameName;

        /// <summary>
        /// Item name's localization key.
        /// </summary>
        public string NameLocalizationKey => _nameLocalizationKey;

        /// <summary>
        /// Item description in English. Intended to provided a basis for localization.
        /// </summary>
        public string DescriptionEnglish => _descriptionEnglish;

        /// <summary>
        /// Localization key for the Item's in-game description.
        /// </summary>        
        public string DescriptionLocalizationKey => _descriptionLocalizationKey;

        [SerializeField]
        string _inGameName;

        [SerializeField]
        string _nameLocalizationKey;

        [SerializeField]
        string _descriptionEnglish;

        [SerializeField]
        string _descriptionLocalizationKey;


    }
}
