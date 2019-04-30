using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zoo.Attributes;
using Zoo.Systems.Construction;

namespace Zoo.Attributes.Construction
{
    /// <summary>
    /// Assets with this attribute will be considered in construction clearance checks, when enabled.
    /// </summary>
    public class ObjectClearanceAttribute : Attribute<ObjectClearanceAttribute>
    {

        public ObjectClearanceComponent ClearanceComponent;

        public ObjectClearanceAttribute(int parentAssetID) : base(parentAssetID)
        {

        }

        public override void Initialize()
        {

        }

        private void InitializeClearanceComponent()
        {
            if (ClearanceComponent == null)
            {
                ClearanceComponent = new ObjectClearanceComponent();
                ClearanceComponent.AssetAttribute = this;
                ClearanceComponent.Initialize(ParentGameData);

            }
        }
    }
}
