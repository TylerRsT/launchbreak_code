using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public static class AudioExtensions
    {
        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventInstance"></param>
        /// <returns></returns>
        public static FMOD.Studio.EventInstance RegisterAsGameplayLooping(this FMOD.Studio.EventInstance eventInstance)
        {
            _loopingInstances.Add(eventInstance);
            if (GameplayStatics.state == GameplayState.GamePaused)
            {
                eventInstance.setPaused(true);
            }

            return eventInstance;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventInstances"></param>
        public static void RegisterAsGameplayLooping(this IEnumerable<FMOD.Studio.EventInstance> eventInstances)
        {
            foreach(var eventInstance in eventInstances)
            {
                eventInstance.RegisterAsGameplayLooping();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventInstance"></param>
        /// <returns></returns>
        public static FMOD.Studio.EventInstance UnregisterAsGameplayLooping(this FMOD.Studio.EventInstance eventInstance)
        {
            _loopingInstances.Remove(eventInstance);
            return eventInstance;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventInstances"></param>
        public static void UnregisterAsGameplayLooping(this IEnumerable<FMOD.Studio.EventInstance> eventInstances)
        {
            foreach (var eventInstance in eventInstances)
            {
                eventInstance.UnregisterAsGameplayLooping();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="playing"></param>
        public static void SetGameplayLoopingPlaying(bool playing)
        {
            foreach (var loopingInstance in _loopingInstances)
            {
                loopingInstance.setPaused(!playing);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static void ClearGameplayLoopings()
        {
            foreach (var loopingInstance in _loopingInstances)
            {
                loopingInstance.setPaused(false);
                loopingInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            }

            _loopingInstances.Clear();
        }

        #endregion

        #region Fields

        private static HashSet<FMOD.Studio.EventInstance> _loopingInstances = new HashSet<FMOD.Studio.EventInstance>();

        #endregion
    }
}