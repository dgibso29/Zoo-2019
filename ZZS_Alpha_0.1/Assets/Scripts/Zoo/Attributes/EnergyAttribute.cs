using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Zoo.Attributes
{
    public class EnergyAttribute : Attribute<EnergyAttribute>
    {

        [Range(0, 100)]
        private float _energyValue;

        public float Energy { get => _energyValue; set => _energyValue = value; }


        public EnergyAttribute(int parentAssetID) : base(parentAssetID)
        {

        }

        public override void Initialize()
        {

        }
    }
}
