using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zoo.Attributes;

namespace Zoo.Assets.Agents
{
    public class AgentData : GameAssetData
    {

        #region Attributes

        public CustomNameAttribute CustomName;

        public HappinessAttribute Happiness;

        public EnergyAttribute Energy;

        public HungerAttribute Hunger;

        public ThirstAttribute Thirst;


        #endregion

        [Newtonsoft.Json.JsonConstructor]
        public AgentData() : base()
        {

        }

        public override void Initialize(GameAsset parentAsset)
        {
            base.Initialize(parentAsset);
            CustomName = new CustomNameAttribute(AssetID);
            Happiness = new HappinessAttribute(AssetID);
            Energy = new EnergyAttribute(AssetID);
            Hunger = new HungerAttribute(AssetID);
            Thirst = new ThirstAttribute(AssetID);
        }

    }
}
