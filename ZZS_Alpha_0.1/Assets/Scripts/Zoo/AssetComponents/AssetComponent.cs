using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Zoo.Assets;

namespace Zoo.AssetComponents
{

    /// <summary>
    /// Base class for Asset Components.
    /// </summary>
    public abstract class AssetComponent : MonoBehaviour
    {

        /// <summary>
        /// Parent Asset Data of this component.
        /// </summary>
        public GameAssetData ParentAssetData;

        /// <summary>
        /// Initial setup of this component.
        /// </summary>
        public virtual void Initialize(GameAssetData parentData)
        {
            ParentAssetData = parentData;
        }

    }
}
