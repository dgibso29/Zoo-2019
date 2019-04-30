using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Zoo.Assets.Agents.Staff
{
    [CreateAssetMenu(menuName = "GameAssets/Agents/Staff")]
    public class StaffAsset : AgentAsset
    {
        public override Type DataType { get; }
    }
}
