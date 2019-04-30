using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Zoo.Attributes
{
    public class HungerAttribute : Attribute<HungerAttribute>
    {

        [Range(0, 100)]
        private float _hungerValue;

        public float Hunger { get => _hungerValue; set => _hungerValue = value; }


        public HungerAttribute(int parentAssetID) : base(parentAssetID)
        {

        }

        public override void Initialize()
        {

        }
    }
}
