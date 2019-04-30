using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Zoo.AssetComponents;
using Zoo.Assets;
using Zoo.Attributes.Construction;

namespace Zoo.Systems.Construction
{
    /// <summary>
    /// Controls clearance checking for this asset.
    /// </summary>
    public class ObjectClearanceComponent : AssetComponent
    {

        /// <summary>
        /// Reference to parent attribute of this component.
        /// </summary>
        public ObjectClearanceAttribute AssetAttribute;

        public BoxCollider Collider;


        public override void Initialize(GameAssetData parentData)
        {
            base.Initialize(parentData);

        }

        private void InitializeCollider()
        {
            if(!ParentAssetData.GameObject.GetComponent<BoxCollider>())
            {
                Collider = ParentAssetData.GameObject.AddComponent<BoxCollider>();
            }
        }

        // TODO: Set collider size based on asset size
        private void SetColliderSize()
        {

        }

    }
}
