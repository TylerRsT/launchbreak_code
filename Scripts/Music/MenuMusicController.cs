using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public static class MenuMusicController
    {
        /// <summary>
        /// 
        /// </summary>
        public static void StartMusic()
        {
            _instance = FMODUnity.RuntimeManager.CreateInstance("event:/Musics/MenuMusic");
            _instance.start();
        }

        /// <summary>
        /// 
        /// </summary>
        public static void StopMusic()
        {
            _instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public static bool isPlaying
        {
            get
            {
                if(!_instance.isValid())
                {
                    return false;
                }

                PLAYBACK_STATE state;
                _instance.getPlaybackState(out state);

                return state == PLAYBACK_STATE.PLAYING;
            }
        }

        #endregion

        #region Fields

        private static EventInstance _instance;

        #endregion
    }
}
