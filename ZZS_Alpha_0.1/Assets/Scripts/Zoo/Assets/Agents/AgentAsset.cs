using System;
using UnityEngine;
using Zoo.Attributes;

namespace Zoo.Assets.Agents
{

    public abstract class AgentAsset : GameAsset
    {

        public abstract override Type DataType { get; }
    }
}
