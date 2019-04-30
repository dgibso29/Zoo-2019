using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zoo.Systems.World;
using UnityEngine;
using Zoo.Assets;

namespace Zoo.IO
{    
    /// <summary>
    /// Holds all serializable game data for saving & loading.
    /// </summary>    
    public class GameData
    {
        /// <summary>
        /// Version of the game for this save.
        /// </summary>
        public string Version => GameInfo.CurrentVersion;

        // General Game Info
        /// <summary>
        /// Name of the saved game.
        /// </summary>
        public string GameName { get => GameInfo.GameName; set => GameInfo.GameName = value; }

        /// <summary>
        /// All info for this particular game.
        /// </summary>
        public SavedGameInfo GameInfo;

        /// <summary>
        /// Keep this at the end to keep saves legible (why hasn't Daniel made this binary yet?)
        /// JK has to be at the front because assets may need to reference it!
        /// </summary>
        public MapData MapData;

        /// <summary>
        /// All save game assets. Must be converted to & from GameManager Asset dictionary.
        /// </summary>
        public GameAssetData[] AssetsToSave;




        /// <summary>
        /// Creates new game data with default settings.
        /// </summary>
        public GameData()
        {
            GameInfo = new SavedGameInfo();

        }


        /// <summary>
        /// Creates new game data with the given settings.
        /// </summary>
        /// <param name="newGameInfo"></param>
        public GameData(SavedGameInfo newGameInfo)
        {
            GameInfo = newGameInfo;
        }

    }
}
