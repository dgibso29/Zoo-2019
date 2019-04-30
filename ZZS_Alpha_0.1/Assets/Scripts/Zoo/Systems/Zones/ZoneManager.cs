using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Experimental.Rendering.HDPipeline;
using Zoo.Systems;
using Zoo.Systems.World;
using Zoo.UI;
using Zoo.Utilities.Rendering.TileMap;

namespace Zoo.Systems.Zones
{
    public class ZoneManager : SystemManager<ZoneManager>
    {
        /// <summary>
        /// Todo Add enclosure zone type
        /// Todo Add staff zone type
        /// </summary>
        /// 
        private bool isDraggingZone;
        
        public List<Zone> zones = new List<Zone>();
        public List<DecalProjectorComponent> DecalProjectorComponents = new List<DecalProjectorComponent>();
        public GameObject zoneObject; 

        private MeshRenderer _meshRenderer;
        private MeshFilter _meshFilter;
        public Material SelectionMeshMaterial;

        public void Start()
        {
            
            InitializeSelectionMesh();
        }

        private void InitializeSelectionMesh()
        {
            _meshFilter = GetComponent<MeshFilter>();
            _meshRenderer = GetComponent<MeshRenderer>();
            GetComponent<MeshRenderer>().sharedMaterial = SelectionMeshMaterial;
        }

        private void EnableEvents()
        {
            MapInteractionManager.Instance.DragSelectStart += HandleDragStart;
            MapInteractionManager.Instance.DragSelectEnd += HandleDragEnd;
        }

        private void DisableEvents()
        {
            MapInteractionManager.Instance.DragSelectStart -= HandleDragStart;
            MapInteractionManager.Instance.DragSelectEnd -= HandleDragEnd;
        }

        public void Update()
        {
            
        }

        public void ToggleZoningTools()
        {
            isDraggingZone = !isDraggingZone;

            if (!isDraggingZone)
            {
                MapInteractionManager.Instance.DisableSelectionMesh();
                _meshRenderer.enabled = isDraggingZone; 
                DisableEvents();
            }
            else
            {
                MapInteractionManager.Instance.EnableSelectionMesh(false);
                _meshRenderer.enabled = isDraggingZone;
                EnableEvents();
            }
        }

        public void HandleDragStart()
        {
        }

        public void HandleDragEnd(MapInteractionManager.DraggingEventArgs e)
        {
            
            for (int i = 0; i < zones.Count; i++)
            {
                for (int y = 0; y < zones[i].TilesInZone.Count; y++)
                {
                    if (zones[i].TilesInZone[y] == e.StartTile)
                    {
                        zones[i].TilesInZone.AddRange(e.TileSelectionData.Tiles); 
                        zones[i].verts.AddRange(e.TileSelectionData.Vertices);
                        
                        Rect rect = new Rect((int)e.StartTile.TilePosition.x, (int)e.StartTile.TilePosition.y,
                            (int)(e.StartTile.TilePosition.x - MapInteractionManager.Instance.GetCurrentTile().TilePosition.x),
                                (int)(e.StartTile.TilePosition.y -
                                MapInteractionManager.Instance.GetCurrentTile().TilePosition.y));
                        
                        DecalProjectorComponent tempProjector = new DecalProjectorComponent();

                        zoneObject.GetComponent<DecalProjectorComponent>().m_Size = new Vector3(rect.width,200,rect.height);
                        Instantiate(zoneObject, new Vector3(rect.x, 200, rect.y), Quaternion.identity);


                        Debug.Log($"Extended existing zone {zones[i].AssetId}");
                        return; 
                    }
                }
            }

            Debug.Log(e.EndTile);
            Rect rect1 = new Rect(
                (int)e.EndTile.TilePosition.y,
                (int)e.EndTile.TilePosition.y,
                (int)(Math.Abs(e.StartTile.TilePosition.x - e.EndTile.TilePosition.x )),
                (int)(Math.Abs(e.StartTile.TilePosition.y - e.EndTile.TilePosition.y)));
            
            zoneObject.GetComponent<DecalProjectorComponent>().m_Size = new Vector3(rect1.width, 200, rect1.height);
            Instantiate(zoneObject, new Vector3(rect1.x, 50, rect1.y), Quaternion.identity);

            var tempZone = new TestZone(e.TileSelectionData.Tiles);
            tempZone.verts = e.TileSelectionData.Vertices;
            isDraggingZone = false;

            CalculateMesh();
        }


        public void CalculateMesh()
        {



        }

        public void RegisterZone(Zone zone)
        {
            
            Debug.Log($"Added zone {zone.AssetId} with {zone.TilesInZone.Count} tiles in zone");
            zones.Add(zone);
        }
    }
}
