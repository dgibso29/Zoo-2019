using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Zoo.Assets.Agents.Visitors
{
    [CreateAssetMenu(menuName = "GameAssets/Agents/Visitor")]
    public class VisitorAsset : AgentAsset
    {
        public override Type DataType { get; }
    }
}
