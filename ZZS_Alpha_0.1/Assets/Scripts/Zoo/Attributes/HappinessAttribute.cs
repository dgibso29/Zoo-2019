using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Zoo.Attributes
{

    public class HappinessAttribute : Attribute<HappinessAttribute>
    {
        //public override string AttributeID => "attribute_Happiness";

        [Range(0,100)]
        private float _happinessValue;

        public float Happiness { get => _happinessValue; set => _happinessValue = value; }


        public HappinessAttribute(int parentAssetID) : base(parentAssetID)
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
