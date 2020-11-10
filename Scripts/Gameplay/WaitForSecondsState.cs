using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public class WaitForSecondsState : CustomYieldInstruction
    {
        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <param name="seconds"></param>
        public WaitForSecondsState(GameplayState state, float seconds)
        {
            _state = state;
            _duration = seconds;
        }

        #endregion

        #region Override

        /// <summary>
        /// 
        /// </summary>
        public override bool keepWaiting
        {
            get
            {
                switch(_state)
                {
                    case GameplayState.GameRunning:
                        _elapsed += GameplayStatics.gameFixedDeltaTime;
                        break;
                    case GameplayState.Cutscene:
                        _elapsed += GameplayStatics.cutsceneFixedDeltaTime;
                        break;
                    case GameplayState.GamePaused:
                        _elapsed += GameplayStatics.pauseFixedDeltaTime;
                        break;
                    default:
                        Debug.Assert(false);
                        break;
                }

                return _elapsed < _duration;
            }
        }

        #endregion

        #region Fields

        private GameplayState _state;
        private float _duration;
        private float _elapsed;

        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    public sealed class WaitForSecondsCutscene : WaitForSecondsState
    {
        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="duration"></param>
        public WaitForSecondsCutscene(float duration)
            : base(GameplayState.Cutscene, duration)
        { }

        #endregion
    }
}