using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zoo.Attributes;
using Zoo.Attributes.Construction;

namespace Zoo.Assets.Objects
{
    public class BuildableObjectData : GameAssetData
    {


        #region Attributes

        public CustomNameAttribute CustomName;
        public ObjectClearanceAttribute ObjectClearance;

        #endregion

        [Newtonsoft.Json.JsonConstructor]
        public BuildableObjectData() : base()
        {
        }
        

        public override void Initialize(GameAsset parentAsset)
        {
            base.Initialize(parentAsset);
            CustomName = new CustomNameAttribute(AssetID);

        }
    }
}
