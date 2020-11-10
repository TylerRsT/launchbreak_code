using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public static class TweenExtensions
    {
        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tween"></param>
        public static void SafeKill<T>(ref T tween) where T : Tween
        {
            if(tween != null && tween.IsActive())
            {
                tween.Kill();
                tween = null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tween"></param>
        public static void SafeComplete<T>(ref T tween) where T : Tween
        {
            if (tween != null && tween.IsActive())
            {
                tween.Complete();
                tween = null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tween"></param>
        public static T SetAsCutscene<T>(this T tween) where T : Tween
        {
            tween.SetUpdate(true);
            _cutsceneTweens.Add(tween);

            var callback = CutsceneTweenEnd(tween);
            tween.onComplete += (callback);
            tween.onKill += (callback);

            return tween;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="active"></param>
        public static void SetCutscenesActive(bool active)
        {
            foreach(var tween in _cutsceneTweens)
            {
                if(tween.IsActive())
                {
                    if(active)
                    {
                        tween.Play();
                    }
                    else
                    {
                        tween.Pause();
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tween"></param>
        /// <returns></returns>
        private static TweenCallback CutsceneTweenEnd(Tween tween)
        {
            return () =>
            {
                _cutsceneTweens.Remove(tween);
            };
        }

        #endregion

        #region Fields

        private static List<Tween> _cutsceneTweens = new List<Tween>();

        #endregion
    }
}