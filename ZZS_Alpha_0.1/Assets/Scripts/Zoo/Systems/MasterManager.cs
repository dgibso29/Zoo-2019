using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zoo.Attributes;
using Zoo.IO;
using Zoo.IO.AssetBundles;
using Zoo.UI.Localization;
using Zoo.Utilities.PathMeshGenerator;

namespace Zoo.Systems
{
    /// <summary>
    /// Manager of all game functions. Intended to exist as part of the Master Scene.
    /// </summary>
    public class MasterManager : SystemManager<MasterManager>
    {

        /// <summary>
        /// Currently loaded game configuration file.
        /// </summary>
        public GameConfig GameConfig;

        private new void Awake()
        {
            StartMasterManager();
            InitialiseGame();
            StartMainMenu();
        }

        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.N))
            {
                StartCoroutine(StartNewGame());
            }

            if (Input.GetKeyDown(KeyCode.B))
            {
                StartCoroutine((LoadGame()));
            }

            if (Input.GetKeyDown(KeyCode.M))
            {
                StartCoroutine(LoadMainMenu());
            }
        }

        private void StartMasterManager()
        {
            //Check if instance already exists
            if (Instance == null)
            {
                //if not, set instance to this
                Instance = this;
            }
            //If instance already exists and it's not this:
            else if (Instance != this)
            { 
                //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
                Destroy(gameObject);
            }

        //Sets this to not be destroyed when reloading scene
            DontDestroyOnLoad(gameObject);

        }

        /// <summary>
        /// Run all necessary startup functions.
        /// </summary>
        void InitialiseGame()
        {
            IOHelper.Initialize();
            AssetBundleHelper.InitializeAssetBundles();
            InitialiseConfigFile();
            //LocalizationManager.Instance.LoadLocalizedText(GameConfig.CurrentLanguage);
            AttributeHelper.InitializeAttributeDictionary();
            PathMeshGenerationTool.Initialize();
            

        }

        /// <summary>
        /// Check for the config file and generate a new one if needed, and then load & reference it.
        /// </summary>
        void InitialiseConfigFile()
        {
            if (IOHelper.CheckForConfigFile() == false)
            {
                IOHelper.SaveConfigFile(new GameConfig());
            }
            GameConfig = IOHelper.LoadConfigFile();
        }

        void StartMainMenu()
        {
            SceneManager.LoadScene("_MainMenu", LoadSceneMode.Single);
            StartCoroutine(StartNewGame());
        }

        public IEnumerator LoadMainMenu()
        {
            SceneManager.LoadScene("_MainMenu", LoadSceneMode.Single);

            yield return null;
        }

        /// <summary>
        /// Starts a new game, using a loading screen
        /// to transition to the currently active scene.
        /// </summary>
        /// <returns></returns>
        public IEnumerator StartNewGame()
        {
            SceneManager.LoadScene("_LoadScreen", LoadSceneMode.Single);

            var asyncLoad = SceneManager.LoadSceneAsync("_GameScene", LoadSceneMode.Additive);
            yield return new WaitUntil(() => asyncLoad.isDone);

            GameManager.Instance.StartNewGame();
            SceneManager.UnloadSceneAsync("_LoadScreen");
            yield return null;

        }

        /// <summary>
        /// Loads the a saved game, using a loading screen
        /// to transition to the currently active scene.
        /// </summary>
        /// <returns></returns>
        public IEnumerator LoadGame()
        {
            SceneManager.LoadScene("_LoadScreen", LoadSceneMode.Single);

            // For some reason, assets are loaded to the loadscreen scene if I do additive here.
            // Can probably fix with some events / trying harder. This works for now.
            var asyncLoad = SceneManager.LoadSceneAsync("_GameScene", LoadSceneMode.Single);
            yield return new WaitUntil(() => asyncLoad.isDone);

            GameManager.Instance.LoadGame("TestGame01");
            //SceneManager.UnloadSceneAsync("_LoadScreen");
            yield return null;
        }


    }
}