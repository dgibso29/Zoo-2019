using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using Zoo.AssetComponents;
using Zoo.Attributes;
using Zoo.Systems;
using Zoo.Assets;

namespace Zoo.Assets
{
    public interface IGameAssetData
    {
        GameObject GameObject { get; set; }

        void Initialize(GameAsset parentAsset);

        /// <summary>
        /// Initialize asset as a hologram -- should ONLY be used as preview
        /// during purchasing/construction. Will not have assetID assigned.
        /// </summary>
        /// <param name="parentAsset"></param>
        void InitializeHologram(GameAsset parentAsset);

        void InitializeFromSave();

        /// <summary>
        /// Perform any necessary tasks on destruction.
        /// </summary>
        void Destroy();

    }

    public abstract class GameAssetData : IGameAssetData
    {
        /// <summary>
        /// Unique ID of this asset.
        /// </summary>
        public int AssetID;

        /// <summary>
        /// String asset type id of this asset.
        /// </summary>
        public string AssetTypeID;

        /// <summary>
        /// Reference to the AssetType.
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        public GameAsset AssetType => AssetManager.GetAssetByStringID(AssetTypeID);

        /// <summary>
        /// Position of the asset in world space. Input values should always be from the
        /// bottom left vertex of the tile being placed on. Output values
        /// will reflect the same.
        /// </summary>
        public virtual Vector3 Position
        {
            get
            {
                var pos = GameObject == null ? _position : GameObject.transform.position;
                // Offset return to get tile position.
                return pos - new Vector3(AssetType.AssetSize.x / 2, 0, AssetType.AssetSize.z / 2);
            }
            set
            {
                // Offset placement position based on asset size.
                var newPos = value + new Vector3(AssetType.AssetSize.x / 2, 0, AssetType.AssetSize.z / 2);
                 
                if (GameObject == null)
                {
                    _position = newPos;
                }
                else
                {
                    _position = newPos;
                    GameObject.transform.position = newPos;
                }
            }
        }


        private Vector3 _position;

        public virtual Vector3 RotationEuler
        {
            get => GameObject == null ? _rotationEuler : GameObject.transform.rotation.eulerAngles;
            set
            {
                if (GameObject == null)
                {
                    _rotationEuler = value;
                }
                else
                {
                    _rotationEuler = value;
                    GameObject.transform.rotation = Quaternion.Euler(value);
                }
            }
        }

        [Newtonsoft.Json.JsonIgnore]
        public virtual Quaternion Rotation => GameObject.transform.rotation;

        private Vector3 _rotationEuler;

        /// <summary>
        /// GameObject manifestation of this asset.
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        public GameObject GameObject
        {
            get => _gameObject;
            set
            {
                _gameObject = value;
                _position = value.transform.position;
                // Ensures that the gameobject always has its ID component.
                // AssetIDComponent does not allow multiple instances on a 
                // single game object, so it will not result in duplicates.
                InitializeAssetIDComponent();
            }
        }

        private GameObject _gameObject;


        /// <summary>
        /// List of any non-hard-coded (ie dynamic) attributes on this asset.
        /// </summary>
        public List<IAttribute> AssetAttributes = new List<IAttribute>();


        /// <summary>
        /// Constructor for initial creation & deserialization.
        /// Must call Initialize for any post-construction logic.
        /// </summary>
        /// <param name="parentAsset"></param>
        [Newtonsoft.Json.JsonConstructor]
        protected GameAssetData()
        {

        }

        /// <summary>
        /// Set up asset. Should only be called on initial creation, not on deserialization.
        /// </summary>
        /// <param name="parentAsset"></param>
        public virtual void Initialize(GameAsset parentAsset)
        {

            AssetID = AssetManager.GetAssetID();
            AssetTypeID = parentAsset.AssetStringID;
            Position = GameObject.transform.position;
            RotationEuler = GameObject.transform.rotation.eulerAngles;
            Debug.Log($"Init pos {Position} & rot {RotationEuler}");
            InitializeAssetIDComponent();
            LoadDynamicAttributes();
            RegisterToGameManager();
        }


        /// <inheritdoc />
        public virtual void InitializeHologram(GameAsset parentAsset)
        {
            AssetTypeID = parentAsset.AssetStringID;
            Position = GameObject.transform.position;
            RotationEuler = GameObject.transform.rotation.eulerAngles;
          

        }

        /// <summary>
        /// Perform any necessary post-load tasks.
        /// </summary>
        public virtual void InitializeFromSave()
        {
            // Re-introduce offset from tile pos.
            Position += new Vector3(AssetType.AssetSize.x / 2, 0, AssetType.AssetSize.z / 2);

        }

        /// <summary>
        /// Attaches and initializes an AssetIDComponent to this asset's GameObject.
        /// </summary>
        public void InitializeAssetIDComponent()
        {
            if (GameObject == null) return;
            // Get AssetIDComponent if it exists; otherwise, create it.
            var component = GameObject.GetComponent<AssetIDComponent>() ? 
                GameObject.GetComponent<AssetIDComponent>() : 
                GameObject.AddComponent<AssetIDComponent>();
            component.ID = AssetID;
            component.ParentAssetData = this;
            GameObject.name = $"Asset {AssetID} ({AssetTypeID})";
        }

        /// <summary>
        /// Loads all dynamic attributes. 
        /// </summary>
        private void LoadDynamicAttributes()
        {
            foreach (var attrib in AssetManager.GetAssetByStringID(AssetTypeID).DynamicAttributeIDs)
            {
                try
                {
                    AssetAttributes.Add(AttributeHelper.GetNewAttributeInstance(attrib, AssetID) as IAttribute);
                }
                catch
                {
                    Debug.Log($"Failed to load attribute of type {attrib}");
                }
            }

        }

        /// <summary>
        /// Registers this asset to the asset dictionary on creation. Further registration/de-registration
        /// will be handled elsewhere as assets are manipulated through gameplay.
        /// </summary>
        private void RegisterToGameManager()
        {
            GameManager.Instance.AddAssetToDictionary(this);
        }


        /// <inheritdoc />
        public virtual void Destroy()
        {

        }




    }
}
