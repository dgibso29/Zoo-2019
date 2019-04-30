using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Zoo.Attributes
{
    public class ThirstAttribute : Attribute<ThirstAttribute>
    {

        [Range(0, 100)]
        private float _thirstValue;

        public float Thirst { get => _thirstValue; set => _thirstValue = value; }


        public ThirstAttribute(int parentAssetID) : base(parentAssetID)
        {

        }

        public override void Initialize()
        {

        }

    }
}
