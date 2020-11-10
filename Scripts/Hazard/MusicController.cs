using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public class MusicController : HazardManager<KeyBasedGameMode>
    {
        #region Const

        private const string BackgroundMusicSoundKey = "BackgroundMusic";
        private const string GameStateParameterName = "GameState";

        #endregion

        #region Override

        /// <summary>
        /// 
        /// </summary>
        public override void OnGameModeStep()
        {
            if(gameMode.highestScore >= gameMode.maxScore - 1)
            {
                foreach(var sound in _sounds)
                {
                    sound.setParameterByName(GameStateParameterName, 1);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Start()
        {
            base.Start();

            _sounds = this.EmitSound(_soundEventKey);
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnDestroy()
        {
            base.OnDestroy();
            StopMusic();
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        public void StopMusic()
        {
            foreach (var sound in _sounds)
            {
                sound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            }
        }

        #endregion

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private string _soundEventKey = BackgroundMusicSoundKey;

        private IEnumerable<FMOD.Studio.EventInstance> _sounds;

        #endregion
    }
}
