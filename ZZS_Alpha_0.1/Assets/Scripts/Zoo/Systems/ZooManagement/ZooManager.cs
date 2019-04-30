using System;
using System.Collections.Generic;
using UnityEngine;
using Zoo.Attributes;
using Zoo.IO;
using Zoo.Settings;

namespace Zoo.Systems.ZooManagement
{
    /// <summary>
    /// Centralized manager of all in-game Zoo functions.
    /// </summary>
    public class ZooManager : SystemManager<ZooManager>
    {

        /// <summary>
        /// Active instance of Zoo Manager. Intended to allow references to all manager types.
        /// </summary>
        //public static ZooManager Instance;

        public string ZooName = "Tahendo Zoo";

        /// <summary>
        /// List of actions player has executed, ordered by most recent. Used to undo & redo actions.
        /// </summary>
        private static List<IGameAction> _actionHistory = 
            new List<IGameAction>(ConfigurationSettings.ActionHistorySize);

        /// <summary>
        /// Index of current action in _actionHistory. 
        /// </summary>
        private static int _currentActionHistoryIndex;

        public void Start()
        {

        }


        public static void AddActionToHistory(IGameAction action)
        {
            // If there are possible redo actions, remove them.
            if (_currentActionHistoryIndex > 0)
            {
                _actionHistory = _actionHistory.GetRange(_currentActionHistoryIndex - 1,
                    _actionHistory.Count - _currentActionHistoryIndex);
            }

            // Insert the action
            _actionHistory.Insert(0, action);
            // Reset action index
            _currentActionHistoryIndex = 0;

            // If list is at capacity, trim the excess.
            if (_actionHistory.Count == ConfigurationSettings.ActionHistorySize)
            {
                if (_actionHistory.Count == ConfigurationSettings.ActionHistorySize)
                {
                    _actionHistory.RemoveAt(ConfigurationSettings.ActionHistorySize);
                }
            }
        }

        public static void UndoAction()
        {
            try
            {
                _actionHistory[_currentActionHistoryIndex].ReverseAction();
            }
            catch
            {
                return;
            }

            _currentActionHistoryIndex++;
        }

        public static void RedoAction()
        {
            // Make sure there are actions to redo.
            if (_currentActionHistoryIndex == 0)
                return;

            try
            {
                _actionHistory[_currentActionHistoryIndex - 1].RedoAction();
            }
            catch
            {
                return;
            }

            _currentActionHistoryIndex--;
        }

    }
}
