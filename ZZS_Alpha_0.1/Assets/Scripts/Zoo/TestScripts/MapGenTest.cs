using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Configuration;
using UnityEngine;
using PrankLib.Maps;
using MeshRenderer = UnityEngine.MeshRenderer;
using Random = System.Random;
using Vector2 = System.Numerics.Vector2;

[System.Obsolete]
public class MapGenTest : MonoBehaviour
{

    /// <summary>
    /// GameObject used to display generated voxel map.
    /// </summary>
    public GameObject VoxelGameObject;

    public Material[] terrainTypes;

    // Start is called before the first frame update
    void Start()
    {
        TestFunction();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void TestFunction()
    {

        var settings2D = new MapGenerator.SimplexSettings2D(528491, Vector2.Zero);
        settings2D.offset = new Vector2(5, 5);
        Debug.Log("Generated 2D Settings.");

        //settings2D.seed = new Random(DateTime.UtcNow.Millisecond).Next();

        var heightmap2D = MapGenerator.GenerateSimplexHeightMap2D(50, 50, settings2D);
        Debug.Log("Generated 2D Height Map.");
        var voxelmap3D = MapGenerator.GenerateVoxelMap3D(heightmap2D, 15);
        Debug.Log("Generated 3D Voxel Map.");
        Debug.Log($"VoxelMap Height = {voxelmap3D.Height}");
        int numRendered = 0;
        for (var z = 0; z < voxelmap3D.Height; z++)
        {
            for (var x = 0; x < voxelmap3D.Width; x++)
            {

                for (var y = 0; y < voxelmap3D.Length; y++)
                {
                    if (z == 0)
                    {
                        Debug.Log($"{x},{y}: {heightmap2D.HeightData[x, y]}");
                    }
                    //Debug.Log($"Height Info at ({x},{y},{z}): {voxelmap3D.VoxelData[x,y,z]}");
                    if (voxelmap3D.VoxelData[x, y, z] == 1)
                    {
                        // Check if voxel is exposed
                        if (!CheckIfExposedVoxel(ref voxelmap3D, new Vector3(x, y, z)))
                        {
                            // And skip if it isn't.
                            continue;
                        }
                        var debugVoxel = Instantiate(VoxelGameObject);
                        numRendered++;
                        debugVoxel.transform.position = new Vector3(x / 2f, z / 2f, y / 2f);
                        // Snow
                        if (z > 24)
                        {
                            debugVoxel.GetComponent<MeshRenderer>().sharedMaterial = terrainTypes[2];
                        }
                        // Rock
                        else if (z > 19)
                        {
                            debugVoxel.GetComponent<MeshRenderer>().sharedMaterial = terrainTypes[0];
                        }
                        // Grass
                        else if (z > 12)
                        {
                            debugVoxel.GetComponent<MeshRenderer>().sharedMaterial = terrainTypes[1];
                        }
                        // Dirt
                        else if (z > 8)
                        {
                            debugVoxel.GetComponent<MeshRenderer>().sharedMaterial = terrainTypes[3];
                        }
                        // Rock
                        else if (z > 4)
                        {
                            debugVoxel.GetComponent<MeshRenderer>().sharedMaterial = terrainTypes[0];
                        }
                        // Bedrock
                        else
                        {
                            debugVoxel.GetComponent<MeshRenderer>().sharedMaterial = terrainTypes[4];
                        }
                    }
                }
            }
        }
        Debug.Log($"Voxel rendering completed. {numRendered} cubes rendered.");

    }

    
    bool CheckIfExposedVoxel(ref VoxelMap3D voxelMap, Vector3 voxelToCheck)
    {
        // Check each neighbor to see if any are empty. If any references fail, voxel is at edge and needs to be displayed.
        try
        {        
            // Check bottom neighbor
            if (voxelMap.VoxelData[(int)voxelToCheck.x, (int)voxelToCheck.y, (int)voxelToCheck.z - 1] == 0)
            {
                return true;
            }

            // Check top neighbor
            if (voxelMap.VoxelData[(int)voxelToCheck.x, (int)voxelToCheck.y, (int)voxelToCheck.z + 1] == 0)
            {
                return true;
            }

            // Check neighbor x + 1
            if (voxelMap.VoxelData[(int)voxelToCheck.x + 1, (int)voxelToCheck.y, (int)voxelToCheck.z] == 0)
            {
                return true;
            }

            // Check neighbor x - 1
            if (voxelMap.VoxelData[(int)voxelToCheck.x - 1, (int)voxelToCheck.y, (int)voxelToCheck.z] == 0)
            {
                return true;
            }

            // Check neighbor y + 1
            if (voxelMap.VoxelData[(int)voxelToCheck.x, (int)voxelToCheck.y + 1, (int)voxelToCheck.z] == 0)
            {
                return true;
            }


            // Check neighbor y - 1
            if (voxelMap.VoxelData[(int)voxelToCheck.x, (int)voxelToCheck.y - 1, (int)voxelToCheck.z] == 0)
            {
                return true;
            }
        }
        catch
        {
            return true;

        }

        return false;
    }
}
