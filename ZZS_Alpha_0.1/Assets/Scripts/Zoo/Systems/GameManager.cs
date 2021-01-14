using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zoo.Assets;
using Zoo.Attributes;
using Zoo.IO;
using Zoo.IO.AssetBundles;
using Zoo.Systems.Construction;
using Zoo.Systems.World;
using Zoo.Systems.ZooManagement;
using Zoo.UI;
using Zoo.Utilities.MapGeneration;

namespace Zoo.Systems
{
    /// <summary>
    /// Manages general in-game functions. Core game loop lives here.
    /// </summary>
    public class GameManager : SystemManager<GameManager>
    {

        /// <summary>
        /// Game info for the current game. Update as needed, and load/save as needed.
        /// </summary>
        public SavedGameInfo CurrentSavedGameInfo;

        /// <summary>
        /// Directory of all existing game assets.
        /// </summary>
        public Dictionary<int, GameAssetData> GameAssetDirectory { get; private set; }

        private bool testFunctionsEnabled = false;
        private bool _isDragSelecting;

        public void Start()
        {
            //MapInteractionManager.Instance.DragSelectStart += HandleDragSelectStartEvent;
            //MapInteractionManager.Instance.DragSelectEnd += HandleDragSelectEndEvent;
        }

        //private void HandleDragSelectStartEvent()
        //{
        //    _isDragSelecting = true;
        //    Debug.Log("Started dragging");
        //}

        //private void HandleDragSelectEndEvent(MapInteractionManager.DraggingEventArgs e)
        //{


        //    var newTilePos = MapInteractionManager.Instance.CurrentVertex;
        //    var type = AssetManager.GetAssetByStringID("animal_test_01");

        //    var position = new List<Vector3>()
        //    {
        //        newTilePos
        //    };

        //    var rotation = new List<Quaternion>()
        //    {
        //        new Quaternion()
        //    };

        //    var container = new List<GameAssetContainer>()
        //    {
        //        new GameAssetContainer(type)
        //    };

        //    var actionParams = new ConstructionActionParameters(container);
            

        //    if (e.AlternateFunctionEnabled)
        //    {

        //    }
        //    if (!TerrainModificationEnabled)
        //    {
        //        new PaintTerrain(actionParams).ExecuteAction();
        //    }
        //    else
        //    {
        //        new LevelTerrain(actionParams,
        //            e.StartTile.GetHeight()).ExecuteAction();
        //    }

        //    _isDragSelecting = false;
        //    Debug.Log("Stopped dragging");
        //}

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Quote))
            {
                SaveGame();
            }

            // Control-modifiers
            if (Input.GetKey(KeyCode.LeftControl))
            {

                if (Input.GetKeyDown(KeyCode.Z))
                {
                    ZooManager.UndoAction();

                }
                else if (Input.GetKeyDown(KeyCode.Y))
                {

                    ZooManager.RedoAction();

                }
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                testFunctionsEnabled = !testFunctionsEnabled;
            }


            //// temporary paths stuff 
            //if (Input.GetMouseButtonDown(0))
            //{
                

            //    if (_isDragSelecting)
            //    {
            //        return;
            //    }



            //    new BuildAction(actionParams, position, rotation).ExecuteAction(); 
            //}

            //if (Input.GetMouseButtonDown(0) && testFunctionsEnabled)
            //{
            //    var newPos = MapInteractionManager.Instance.CurrentVertex;
            //    newPos.y = 10;
            //    //AssetManager.CreateNewAsset("scenery_block_test_large_01", newPos);
            //    var posList = new List<Vector3>()
            //    {
            //        new Vector3(newPos.x + 2, 10, newPos.z),
            //        new Vector3(newPos.x, 10, newPos.z + 2),
            //        new Vector3(newPos.x + 1, 10, newPos.z + 3),
            //        new Vector3(newPos.x, 10, newPos.z),
            //    };
            //    var rotList = new List<Quaternion>(new Quaternion[4]);
            //    var type = AssetManager.GetAssetByStringID("scenery_block_test_l_01");
            //    var containers = new List<GameAssetContainer>()
            //    {
            //        new GameAssetContainer(type),
            //        new GameAssetContainer(type),
            //        new GameAssetContainer(type),
            //        new GameAssetContainer(type),
            //    };
            //    var actionParams = new ConstructionActionParameters(containers);
            //    new BuildAction(actionParams, posList, rotList).ExecuteAction();
            //}

            if (Input.GetMouseButtonDown(1) && testFunctionsEnabled)
            {
                var assetComponent = MapInteractionManager.Instance.GetSelectedAsset();
                if (assetComponent != null)
                {
                    var container = new GameAssetContainer(assetComponent.ParentAssetData, 
                        AssetManager.GetAssetByStringID(assetComponent.AssetTypeID), assetComponent.ParentAssetData.GameObject);
                    var actionParams = new ConstructionActionParameters(new List<GameAssetContainer>() {container});
                    new DestroyAction(actionParams).ExecuteAction();
                    
                }
            }
        }
        

        /// <summary>
        /// Performs all required tasks to start game loop.
        /// </summary>
        private void InitializeGame()
        {

        }

        /// <summary>
        /// Starts new game with default settings.
        /// </summary>
        public void StartNewGame()
        {
            CurrentSavedGameInfo = new SavedGameInfo()
            {
                GameName = "TestGame01",
                TimeLoaded = System.DateTime.UtcNow,
                VersionOnCreation = Application.version,
        };
            GameAssetDirectory = new Dictionary<int, GameAssetData> ();
            TileMapManager.Instance.InitializeTileMap(MapGeneratorComponent.Instance.GenerateMap());
        }

        /// <summary>
        /// Starts game from loaded GameData.
        /// </summary>
        /// <param name="gameToLoad"></param>
        private void StartLoadedGame(GameData gameToLoad)
        {
            Debug.Log($"Started loading at {DateTime.UtcNow}");
            CurrentSavedGameInfo = gameToLoad.GameInfo;
            TileMapManager.Instance.InitializeTileMap(gameToLoad.MapData);
            GameAssetDirectory = new Dictionary<int, GameAssetData>();
            foreach (GameAssetData data in gameToLoad.AssetsToSave)
            {
                AssetManager.LoadSavedAsset(data);
                AddAssetToDictionary(data);
            }
            Debug.Log($"Done loading at {DateTime.UtcNow}");
            DictCount();
        }

        /// <summary>
        /// Loads game of given name & starts game with it.
        /// </summary>
        /// <param name="gameName"></param>
        public void LoadGame(string gameName)
        {
            StartLoadedGame(IOHelper.LoadGameFromDisk(gameName));

        }

        /// <summary>
        /// Saves current game data to disk.
        /// </summary>
        public void SaveGame()
        {
            var save = new GameData(CurrentSavedGameInfo);
            save.MapData = TileMapManager.Instance.ActiveMapData;
            save.AssetsToSave = GameAssetDirectory.Values.ToArray();
            IOHelper.SaveGameToDisk(save, save.GameName);
        }

        /// <summary>
        /// Adds given asset to dictionary, if it doesn't already exist.
        /// </summary>
        /// <param name="toAdd"></param>
        public void AddAssetToDictionary(GameAssetData toAdd)
        {
            if (!GameAssetDirectory.ContainsKey(toAdd.AssetID))
            {
                GameAssetDirectory.Add(toAdd.AssetID, toAdd);
            }
        }

        /// <summary>
        /// Removes given key from dictionary, if valid.
        /// </summary>
        /// <param name="key"></param>
        public void RemoveAssetFromDictionary(int key)
        {
            if (GameAssetDirectory.ContainsKey(key)) GameAssetDirectory.Remove(key);
        }

        /// <summary>
        /// Outputs current asset directory count.
        /// </summary>
        public void DictCount()
        {
            Debug.Log(GameAssetDirectory.Count);
        }

    }
}
