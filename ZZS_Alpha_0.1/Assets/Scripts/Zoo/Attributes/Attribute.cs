using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using Zoo.Assets;
using Zoo.Systems;


namespace Zoo.Attributes
{
    /// <summary>
    /// Template for any attribute (component) that will be added to a GameAsset.
    /// Attributes can be hard-coded to GameAssets, or added via XML.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Attribute<T> : IAttribute
    {

        /// <summary>
        /// Global registry of all attributes of type T, where T should be the class type.
        /// Attributes are registered and de-registered as needed, and acted upon
        /// by game logic by means of the Registry.
        /// </summary>
        public static List<T> Registry = new List<T>();

        /// <summary>
        /// String ID of this attribute, used to grab the type as needed.
        /// </summary>
        public static string AttributeID = typeof(T).Name;

        /// <summary>
        /// ID of the asset to which this attribute belongs.
        /// </summary>
        public int ParentAssetID;

        /// <summary>
        /// GameData to which this attribute belongs.
        /// </summary>
        public GameAssetData ParentGameData => GameManager.Instance.GameAssetDirectory[ParentAssetID];

        /// <summary>
        /// Allows for global enabling/disabling of this attribute type.
        /// </summary>
        public static bool EnabledGlobally = true;

        /// <summary>
        /// Allows for local enabling/disabling of this attribute instance.
        /// </summary>
        public bool Enabled = true;

        protected Attribute(int parentAssetID)
        {
            ParentAssetID = parentAssetID;
            AddToRegistry();
            Initialize();
        }


        public abstract void Initialize();


        /// <summary>
        /// This is probably redundant, but leaving for now.
        /// </summary>
        public static void RegisterToAttributeDictionary()
        {
            var thisType = typeof(T);
            Debug.Log(thisType.Name);
            if (AttributeHelper.AttributeDictionary.ContainsKey(thisType.Name))
            {
                return;
            }
            AttributeHelper.AttributeDictionary.Add(thisType.Name, thisType);
        }

        /// <summary>
        /// Register the Attribute to its type registry.
        /// </summary>
        public void AddToRegistry()
        {
            dynamic instance = this;
            Registry.Add(instance);
        }

        /// <summary>
        /// De-register the Attribute from its type registry.
        /// </summary>
        public void RemoveFromRegistry()
        {
            dynamic instance = this;
            Registry.Remove(instance);
        }


    }
}
