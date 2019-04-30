using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zoo.Systems
{
    /// <summary>
    /// Data class holding all game configuration (Graphics, Audio, Etc)
    /// </summary>
    [System.Serializable]
    public class GameConfig
    {
        #region Camera Settings

        public float CameraPanSpeed;
        public float MinCameraZoom;
        public float MaxCameraZoom;
        public float ZoomSpeed;
        public float MouseDragSensitivity;

        #endregion

        /// <summary>
        /// Currently selected language for localization.
        /// </summary>
        public string CurrentLanguage = "en-US";

        /// <summary>
        /// If true, display temperature in celsius. If false, display in fahrenheit.
        /// </summary>
        public bool TempInCelsius = true;

        /// <summary>
        /// Creates a new GameConfig with default settings.
        /// </summary>
        public GameConfig()
        {
            CameraPanSpeed = 0.75f;
            MinCameraZoom = 10;
            MaxCameraZoom = 50;
            ZoomSpeed = 1;
            MouseDragSensitivity = 0.1f;

            CurrentLanguage = "en-US";

            TempInCelsius = true;
        }
    }
}
