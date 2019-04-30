using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zoo.IO
{
    /// <summary>
    /// Holds general info for this saved game, such as name & time saved.
    /// </summary>
    [System.Serializable]
    public class SavedGameInfo
    {
        /// <summary>
        /// Name of the saved game.
        /// </summary>
        public string GameName;

        /// <summary>
        /// Version of game when save first created.
        /// </summary>
        public string VersionOnCreation;

        /// <summary>
        /// Current game version for this save.
        /// </summary>
        public string CurrentVersion;

        /// <summary>
        /// Used for assigning unique IDs to all in-game objects.
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        public int NextUniqueID
        {
            get => _nextUniqueId++;
            private set => _nextUniqueId = value;
        }

        /// <summary>
        /// Total time spent playing this save.
        /// </summary>
        public TimeSpan TimePlayed;

        /// <summary>
        /// Time/Date at which the game was saved.
        /// </summary>
        public System.DateTime TimeLoaded;

        /// <summary>
        /// Time/Date at which the game was saved.
        /// </summary>
        public System.DateTime TimeSaved;

        [Newtonsoft.Json.JsonProperty("NextUniqueID")]
        private int _nextUniqueId;

        /// <summary>
        /// Creates new game settings with default values.
        /// </summary>
        public SavedGameInfo()
        {
            NextUniqueID = 1;
            GameName = "TestGame01";
            TimePlayed = new TimeSpan();
        }


    }
}