using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Zoo.Assets.Objects.Scenery
{
    [CreateAssetMenu(menuName = "GameAssets/Objects/Scenery")]
    public class SceneryAsset : BuildableObject
    {

        public override Type DataType => typeof(BuildableObjectData);

    }
}
