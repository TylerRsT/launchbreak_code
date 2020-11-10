using Elendow.SpritedowAnimator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public static class AnimatorExtensions
    {
        #region Methods

        /// <summary>
        /// 
        /// </summary><param name="animator"></param>
        /// <returns></returns>
        public static SpriteAnimator RegisterAsCutscene(this SpriteAnimator animator)
        {
            _cutsceneAnimators.Add(animator);
            if(GameplayStatics.state == GameplayState.GamePaused)
            {
                animator.Pause();
            }

            return animator;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="animator"></param>
        /// <returns></returns>
        public static SpriteAnimator UnregisterAsCutscene(this SpriteAnimator animator)
        {
            _cutsceneAnimators.Remove(animator);
            return animator;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="playing"></param>
        public static void SetCutscenesPlaying(bool playing)
        {
            var cutscenesToRemove = new List<SpriteAnimator>();
            foreach(var animator in _cutsceneAnimators)
            {
                if(animator == null)
                {
                    cutscenesToRemove.Add(animator);
                    continue;
                }

                if (playing)
                {
                    animator.Resume();
                }
                else
                {
                    animator.Pause();
                }
            }

            foreach(var cutsceneToRemove in cutscenesToRemove)
            {
                _cutsceneAnimators.Remove(cutsceneToRemove);
            }
        }

        #endregion

        #region Fields

        private static HashSet<SpriteAnimator> _cutsceneAnimators = new HashSet<SpriteAnimator>();

        #endregion
    }
}