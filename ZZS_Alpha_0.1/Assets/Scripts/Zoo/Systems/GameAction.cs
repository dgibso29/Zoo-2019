using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zoo.Systems.ZooManagement;


namespace Zoo.Systems
{

    public interface IGameAction
    {

        void ReverseAction();

        void RedoAction();


    };

    public abstract class GameAction<T, U> : IGameAction where T : GameActionParameters where U : UndoGameActionParameters
    {
        /// <summary>
        /// Data required to execute action.
        /// </summary>
        public T Parameters;

        /// <summary>
        /// Data required to undo action.
        /// </summary>
        public U UndoParameters;

        /// <summary>
        /// If true, action has been performed & can be reversed.
        /// </summary>
        private bool _actionPerformed = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters"></param>
        internal GameAction(T parameters)
        {
            Parameters = parameters;
        }

        /// <summary>
        /// Attempt to execute the action using the given parameters.
        /// </summary>
        public void ExecuteAction()
        {
            InitializeAction();
            // Validate before completing action.
            if (!ValidateAction()) return;
            CompleteAction();
            _actionPerformed = true;
            AddActionToQueue();
            ExitAction();
        }

        /// <summary>
        /// Adds action to the Action History queue, allowing for undoing & re-doing actions.
        /// </summary>
        public void AddActionToQueue()
        {
            ZooManager.AddActionToHistory(this);

        }

        /// <summary>
        /// Re-execute the action. Only call if action has already been executed & undone.
        /// This function does not add the action to the queue.
        /// </summary>
        public void RedoAction()
        {
            InitializeAction();
            // Validate before completing action.
            if (!ValidateAction()) return;
            CompleteAction();
            _actionPerformed = true;
            ExitAction();
        }

        /// <summary>
        /// Reverse the action using the UndoParameters.
        /// </summary>
        public void ReverseAction()
        {
            if (!_actionPerformed) return;
            UndoAction();
            _actionPerformed = false;
            ExitAction();
        }

        /// <summary>
        /// Perform any necessary preliminary work, including setting UndoParameters.
        /// </summary>
        protected virtual void InitializeAction()
        {

        }

        /// <summary>
        /// Ensure action is valid, and prevent its execution if not.
        /// Defaults to return true, assuming that validation will be implemented
        /// as needed.
        /// </summary>
        /// <returns></returns>
        protected virtual bool ValidateAction()
        {

            return true;
        }

        /// <summary>
        /// Perform the action.
        /// </summary>
        protected virtual void CompleteAction()
        {

        }

        /// <summary>
        /// Perform any post-completion tasks.
        /// </summary>
        protected virtual void ExitAction()
        {

        }

        /// <summary>
        /// Undo the action, taking care that *all* effects are reversed.
        /// </summary>
        protected virtual void UndoAction()
        {

        }

    }

        public abstract class GameActionParameters
    {
        // ?
    }


    public abstract class UndoGameActionParameters
    {
        // ?
    }

}
