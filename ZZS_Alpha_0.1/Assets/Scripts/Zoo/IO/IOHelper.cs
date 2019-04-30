using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json;
using Zoo.Assets;
using Zoo.IO.JSONConverters;
using Zoo.Systems;


namespace Zoo.IO
{

    /// <summary>
    /// Handles all IO functions.
    /// </summary>
    public static class IOHelper
    {

        private static readonly string MyDocumentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
        private static readonly string CompanyFolderPath = MyDocumentsPath + "/Earring Pranks Studios";
        private static readonly string GameDataFolderPath = CompanyFolderPath + "/Zoo";
        private static readonly string StreamingAssetsPath = Application.dataPath + "/StreamingAssets";

        // Game Data Folder Paths
        public static string StreamingAssetsBundlesFolderPath = StreamingAssetsPath + "/Bundles";
        public static string StreamingAssetsSavesFolderPath = StreamingAssetsPath + "/Saves";
        public static string StreamingAssetsScenariosFolderPath = StreamingAssetsPath + "/Scenarios";
        public static string StreamingAssetsLocalizationFolderPath = StreamingAssetsPath + "/Localization";

        // User Data Folder Paths
        public static string UserDataModsFolderPath = GameDataFolderPath + "/Mods";
        public static string UserDataSavesFolderPath = GameDataFolderPath + "/Saves";
        public static string UserDataScenariosFolderPath = GameDataFolderPath + "/Scenarios";
        public static string UserDataSettingsFolderPath = GameDataFolderPath + "/Settings";


        static readonly string[] FileStructureFolders =
        {
                // Streaming Assets Folders
                StreamingAssetsBundlesFolderPath,
                StreamingAssetsSavesFolderPath,
                StreamingAssetsScenariosFolderPath,
                StreamingAssetsLocalizationFolderPath,
                // User Data Folders
                UserDataModsFolderPath,
                UserDataSavesFolderPath,
                UserDataScenariosFolderPath,
                UserDataSettingsFolderPath,

        };

        private static JsonSerializer _serializer;

        /// <summary>
        /// Perform any necessary startup tasks.
        /// </summary>
        public static void Initialize()
        {
            CheckFileStructure();
            InitializeJsonSerializer();
        }

        private static void InitializeJsonSerializer()
        {
            //_serializer = new JsonSerializer();
            var settings = JsonConvert.DefaultSettings();
            settings.Converters.Add(new TileJsonConverter());
            settings.Converters.Add(new GameAssetJSONDataConverter());
            settings.TypeNameHandling = TypeNameHandling.Auto;
        }

        /// <summary>
        /// Check for missing folders in file structure, and generate any missing entries. Run on game start.
        /// </summary>
        public static void CheckFileStructure()
        {
            foreach (string folder in FileStructureFolders)
            {
                if (!Directory.Exists(folder))
                {
                    GenerateFileStructure();
                    break;
                }
            }
        }

        /// <summary>
        /// Generate any missing folders in file structure (Ex: Saves, Scenarios).
        /// </summary>
        public static void GenerateFileStructure()
        {
            foreach (string folder in FileStructureFolders)
            {
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
            }

        }
        #region Game Config File

        /// <summary>
        /// Returns false if the game config file is not found.
        /// </summary>
        /// <returns></returns>
        public static bool CheckForConfigFile()
        {
            return File.Exists(Application.persistentDataPath + $"/config.txt");
        }

        public static void SaveConfigFile(GameConfig config)
        {
            JsonSerializer serializer = new JsonSerializer();

            using (StreamWriter sw = new StreamWriter(Application.persistentDataPath + $"/config.txt"))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, config);
            }
        }

        public static GameConfig LoadConfigFile()
        {
            JsonSerializer serializer = new JsonSerializer();

            using (StreamReader sr = new StreamReader(Application.persistentDataPath + $"/config.txt"))
            using (JsonReader reader = new JsonTextReader(sr))
            {
                return serializer.Deserialize<GameConfig>(reader);
            }
        }

        #endregion

        #region Asset Info JSON

        public static List<GameAssetJSONData> DeserializeAssetInfo(TextAsset assetInfoAsset)
        {
            var assetInfoString = assetInfoAsset.ToString();
            var loaded = JsonConvert.DeserializeObject<List<GameAssetJSONData>>(assetInfoString);
            return loaded;
            
        }

        #endregion

        #region SavedGameIO
        public static void SaveGameToDisk(GameData gameToSave, string saveName)
        {
            gameToSave.GameInfo.TimeSaved = System.DateTime.UtcNow;
            gameToSave.GameInfo.TimePlayed += gameToSave.GameInfo.TimeSaved - gameToSave.GameInfo.TimeLoaded;
            gameToSave.GameInfo.CurrentVersion = Application.version;

            var serializer = new JsonSerializer();

            using (var sw = new StreamWriter(UserDataSavesFolderPath + $"/{saveName}.zoo"))
            using (var writer = new JsonTextWriter(sw))
            {
                var text = JsonConvert.SerializeObject(gameToSave, Formatting.Indented);
                writer.WriteRaw(text);
            }
        }

        public static GameData LoadGameFromDisk(string saveName)
        {
            var serializer = new JsonSerializer();

            using (var sr = new StreamReader((UserDataSavesFolderPath + $"/{saveName}.zoo")))
            using (var reader = new JsonTextReader(sr))
            {
                var text = sr.ReadToEnd();
                var loaded = JsonConvert.DeserializeObject<GameData>(text);
                loaded.GameInfo.TimeLoaded = System.DateTime.UtcNow;
                return loaded;
            }
        }
        #endregion

        //#region ScenarioIO
        //public static void SaveScenarioToDisk(Scenario scenarioToSave, string scenarioName, string savePath)
        //{
        //    scenarioToSave = new Scenario();
        //    savePath += $"/{scenarioName}.scen";
        //    BinaryFormatter bf = new BinaryFormatter();
        //    FileStream file = File.Create(savePath);
        //    bf.Serialize(file, scenarioToSave);
        //    file.Close();
        //    Debug.Log($"Saved scenario to {savePath}.");

        //}

        //public static Scenario LoadScenarioFromDisk(string savePath)
        //{
        //    BinaryFormatter bf = new BinaryFormatter();
        //    FileStream file = File.Open(savePath, FileMode.Open);
        //    Scenario loadedGame = (Scenario)bf.Deserialize(file);
        //    file.Close();
        //    return loadedGame;
        //}
        //#endregion
    }
}