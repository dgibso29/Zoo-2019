using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Zoo.Assets.Objects.Paths
{
    [CreateAssetMenu(menuName = "GameAssets/Objects/Path")]
    public class PathAsset : BuildableObject
    {
        public override Type DataType => typeof(PathData);
    }
}
