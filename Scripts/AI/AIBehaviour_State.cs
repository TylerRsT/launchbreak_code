using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    [AIBehaviour]
    [RequireComponent(typeof(AIBehaviour_Pickable))]
    public class AIBehaviour_State : AIBehaviour
    {
        #region Messages

        /// <summary>
        /// 
        /// </summary>
        private void FixedUpdate()
        {
            var oldState = _characterState;
            _characterState = character.gameplayState;

            if(oldState != _characterState)
            {
                OnStateChanged(oldState);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oldState"></param>
        private void OnStateChanged(CharacterGameplayState oldState)
        {
            if (_task != null)
            {
                behaviourManager.StopTask(_task);
                _task = null;
            }

            switch(_characterState)
            {
                case CharacterGameplayState.LaunchKey:
                    _task = new AITask_Goto(character.spawner.transform.position);
                    behaviourManager.SetTask(_task, float.MaxValue);
                    break;
            }

            switch(oldState)
            {
                case CharacterGameplayState.LaunchKey:
                    GetComponent<AIBehaviour_Pickable>().timeBeforeLaunchKey = 0.5f;
                    behaviourManager.StopTask();
                    break;
            }
        }

        #endregion

        #region Fields

        private CharacterGameplayState _characterState = CharacterGameplayState.Default;
        private AITask _task;

        #endregion
    }
}