using System;
using UnityEngine;
using Zoo.Attributes;

namespace Zoo.Assets.Objects
{
    public abstract class BuildableObject : GameAsset
    {

        public abstract override Type DataType { get; }

    }
}
