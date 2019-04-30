using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zoo.Assets;
using Zoo.Systems.Construction;


namespace Zoo.UI.Elements
{
    /// <summary>
    /// Attach to UIGameAssetContainer prefab. When loaded with AssetTypeID,
    /// initializes the prefab with that asset's data.
    /// </summary>
    [DisallowMultipleComponent]
    public class UIGameAssetContainer : MonoBehaviour, IDynamicUIElement, ITooltip
    {

        /// <summary>
        /// String asset type ID to display in this container.
        /// </summary>
        public string AssetTypeID;

        public RawImage Icon;

        public Text AssetNameText;

        /// <summary>
        /// Reference to the asset in this container.
        /// </summary>
        private GameAsset _containerAsset;

        // Element references


        private void Awake()
        {
            //SetComponentReferences();

        }

        void Start()
        {
            Initialize(AssetTypeID);
            UpdateElementData();
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        /// <summary>
        /// Loads the container with the asset of assetTypeID.
        /// </summary>
        /// <param name="assetTypeID"></param>
        public void Initialize(string assetTypeID)
        {
            AssetTypeID = assetTypeID;
            _containerAsset = AssetManager.GetAssetByStringID(assetTypeID);
        }

        public void SetComponentReferences()
        {
            // Not needed here
        }

        public void UpdateElementData()
        {
            Icon.texture = _containerAsset.AssetUITexture;
            AssetNameText.text = _containerAsset.AssetStringID;
        }

        /// <summary>
        /// Initiates purchase of this asset type through ConstructionManager.
        /// </summary>
        public void InitiatePurchase()
        {
            Debug.Log("Called!");
            ConstructionManager.Instance.InitiatePurchase(AssetTypeID);
        }

        public string GetTooltip()
        {
            throw new System.NotImplementedException();
        }
    }
}
