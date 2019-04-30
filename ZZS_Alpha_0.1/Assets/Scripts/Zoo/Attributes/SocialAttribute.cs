using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Zoo.Attributes
{
    public class SocialAttribute : Attribute<SocialAttribute>
    {

        [Range(0, 100)]
        private float _socialValue;

        public float Social { get => _socialValue; set => _socialValue = value; }


        public SocialAttribute(int parentAssetID) : base(parentAssetID)
        {

        }

        public override void Initialize()
        {

        }
    }
}
