using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Zoo.Assets;
using Zoo.IO;
using Zoo.Systems.World;

namespace Zoo.Systems.Zones
{
    public abstract class Zone : IPersistent
    {
        public int AssetId;
        public List<Vector3> verts; 

        protected Zone()
        {
            Initialize();
        }

        public void Initialize()
        {
            AssetId = AssetManager.GetAssetID();

        }

        /// <summary>
        /// Registers Zone to Zone Manager
        /// </summary>
        public abstract void Register();

        /// <summary>
        /// The tiles owned by the zone
        /// </summary>
        public abstract List<Tile> TilesInZone { get; set; }

        /// <summary>
        /// Used for checking whether or not zone overlaps with another zone
        /// </summary>
        public  abstract Rect zoneRect { get; set; }

        public void InitializeFromSave()
        {
            
        }
    }
}
