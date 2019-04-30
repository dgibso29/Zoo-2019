using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Zoo.Attributes;

namespace Zoo.Assets.Agents.Animals
{
    [CreateAssetMenu(menuName = "GameAssets/Agents/Animal")]
    public class AnimalAsset : AgentAsset
    {
        public override Type DataType => typeof(AnimalData);
    }
}
