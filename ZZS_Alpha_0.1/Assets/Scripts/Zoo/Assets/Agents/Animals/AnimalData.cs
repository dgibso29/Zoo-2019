using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zoo.Attributes;
using UnityEngine;

namespace Zoo.Assets.Agents.Animals
{
    public class AnimalData : AgentData
    {

        

        #region Attributes

        public SocialAttribute Social;

        #endregion

        [Newtonsoft.Json.JsonConstructor]
        public AnimalData() : base()
        {
        }

        public override void Initialize(GameAsset parentAsset)
        {
            base.Initialize(parentAsset);
            Social = new SocialAttribute(AssetID);

        }

    }
}
