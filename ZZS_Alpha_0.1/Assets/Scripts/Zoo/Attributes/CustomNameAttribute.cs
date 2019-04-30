using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zoo.Attributes
{
    public class CustomNameAttribute : Attribute<CustomNameAttribute>
    {
        //public override string AttributeID => "attribute_CustomName";


        public string CustomName { get; private set; }

        public void ChangeName(string newName)
        {
            CustomName = newName;
        }

        public CustomNameAttribute(int parentAssetID) : base(parentAssetID)
        {
        }

        public override void Initialize()
        {
        }

        //public override void AddToRegistry()
        //{
        //    throw new NotImplementedException();
        //}

        //public override void RemoveFromRegistry()
        //{
        //    throw new NotImplementedException();
        //}
    }
}
