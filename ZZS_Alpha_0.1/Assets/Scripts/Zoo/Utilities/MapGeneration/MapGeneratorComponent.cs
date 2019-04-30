using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using PrankLib.Maps;
using Unity.Jobs;
using Unity.Collections;
using Zoo.Systems;
using Zoo.Systems.World;
using Zoo.UI;
using Zoo.Utilities.Rendering.TileMap;
using UnityEngine.Profiling;
using Zoo.Systems.ZooManagement;


namespace Zoo.Utilities.MapGeneration
{

    public class MapGeneratorComponent : SystemManager<MapGeneratorComponent>
    {
        [Header("Map Size Parameters")]
        [Tooltip("Width of the playable map in world units X.")]
        public int MapWidthX = 50;

        [Tooltip("Length of the playable map in world units Z.")]
        public int MapLengthZ = 50;

        [Tooltip("Height of generated terrain in world units Y.")]
        public int MapHeightY = 5;

        [Tooltip("When true, a flat map will be generated.")]
        public bool GenerateFlatMap = true;

        [Tooltip("Determines number of tiles per 1x1 world space coordinates, where numTiles = 2 ^ Resolution.")]
        public int VerticeResolution = 2;

        [Tooltip("'Buffer' filled layers below generated layer, where # Layers = LowerBuffer * VerticeResolution.")]
        public int LowerBuffer = 4;

        [Tooltip("'Buffer' empty layers above generated layer, where # Layers = UpperBuffer * VerticeResolution.")]
        public int UpperBuffer = 6;

        [Header("Map Generation Parameters")] [Tooltip("Random seed for map generation.")]
        public int Seed = 528491;

        [Tooltip("When enabled, seed will be randomised on generation.")]
        public bool RandomSeed = false;

        public float Scale = 50;

        [Min(1)] public int Octaves = 6;

        [Range(0, 1)] public float Persistence = .6f;

        [Min(0.0001f)] public float Lacunarity = 2;

        public Vector2 Offset;

        // Current Simplex Settings.
        public static MapGenerator.SimplexSettings2D CurrentSimplexSettings2D;

        /// <summary>
        /// Current generated HeightMap2D;
        /// </summary>
        public static HeightMap2D CurrentHeightMap2D;

        public int type = 1;

        private JobHandle jobHandle;



        public MapData GenerateMap()
        {
            if (GenerateFlatMap)
            {
                return new MapData(MapWidthX + MapData.NonPlayableBufferSize, 
                    MapLengthZ + MapData.NonPlayableBufferSize);
            }
            else
            {
                if (RandomSeed)
                {
                    Seed = new System.Random(DateTime.UtcNow.Millisecond).Next();
                }

                // Generate Simplex Settings based on parameters set in inspector.
                CurrentSimplexSettings2D =
                    new MapGenerator.SimplexSettings2D(Seed, new System.Numerics.Vector2(Offset.x, Offset.y),
                        Scale, Octaves, Persistence, Lacunarity);

                // Generate HeighMap2D from Simplex Settings.
                CurrentHeightMap2D = MapGenerator.GenerateSimplexHeightMap2D(((MapWidthX + (MapData.NonPlayableBufferSize)) * VerticeResolution) + 1,
                    ((MapLengthZ + (MapData.NonPlayableBufferSize)) * VerticeResolution) + 1, CurrentSimplexSettings2D);

                var normalisedNoise = GetNormalisedHeightData(CurrentHeightMap2D);

                return new MapData(MapWidthX + MapData.NonPlayableBufferSize, 
                    MapLengthZ + MapData.NonPlayableBufferSize, normalisedNoise);
            }

          //  TileMapManager.Instance.ActiveMapData.SetTileTerrainType(TileMapManager.Instance.ActiveMapData.TileMap[26, 27], 5);
            
        }

        private struct NormaliseHeightJob : IJobParallelFor
        {

            public NativeArray<float> NoiseData;
            public int MapHeightY;

            public void Execute(int i)
            {
                var sample = NoiseData[i];
                sample *= 4;
                sample *= MapHeightY;

                sample = Mathf.Round(sample) / 4f;
                // hope it works
                NoiseData[i] = sample;
            }
        }

        /// <summary>
        /// Returns noise values of given heightmap normalised to map generation height & world height increments.
        /// </summary>
        /// <param name="heightMap"></param>
        /// <returns></returns>
        public float[,] GetNormalisedHeightData(HeightMap2D heightMap)
        {
            // Create job
            var normaliseJob = new NormaliseHeightJob()
            {
                NoiseData = new NativeArray<float>(heightMap.HeightData1D, Allocator.TempJob),
                MapHeightY = MapHeightY
            };

            // Schedule job
            jobHandle = normaliseJob.Schedule(heightMap.HeightData1D.Length, 1);
            // Execute job & wait til completion
            jobHandle.Complete();

            //var modifiedFlatData = new float[heightMap.HeightData1D.Length];
            var noise2D = new float[heightMap.HeightData.GetLength(0), heightMap.HeightData.GetLength(1)];
            for (int x = 0, i = 0; x < heightMap.HeightData.GetLength(0); x++)
            {
                for (var z = 0; z < heightMap.HeightData.GetLength(1); z++, i++)
                {
                    noise2D[x, z] = normaliseJob.NoiseData[i];
                }
            }

            //normaliseJob.NoiseData.CopyTo(modifiedFlatData);

            normaliseJob.NoiseData.Dispose();
            return noise2D;
            //return modifiedFlatData;

        }

    }
}