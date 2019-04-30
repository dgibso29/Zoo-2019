using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Zoo.Systems.World;

namespace Zoo.Systems.Zones
{
    class TestZone: Zone
    {

        public override List<Tile> TilesInZone { get; set; }
        public override Rect zoneRect { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public TestZone(List<Tile> tilesInZone): base()
        {
            this.TilesInZone = tilesInZone;
            Register();
        }

        


        public override void Register()
        {
            ZoneManager.Instance.RegisterZone(this);
            
        }
    }
}
