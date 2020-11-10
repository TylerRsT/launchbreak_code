using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public class SoundEmitter : MonoBehaviour
    {
        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="soundKey"></param>
        /// <param name="mandatory"></param>
        public IEnumerable<FMOD.Studio.EventInstance> Emit(string soundKey, bool mandatory = true)
        {
            var soundInfo = _soundMap.FirstOrDefault(x => x.name == soundKey);
            if(soundInfo.Equals(default(SoundInfo)))
            {
                if (mandatory)
                {
                    Debug.LogError($"Could not find sound key '{soundKey}' in sound map.");
                }
                return new FMOD.Studio.EventInstance[0];
            }

            return Emit(soundInfo.soundIds);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="soundIds"></param>
        /// <returns></returns>
        public IEnumerable<FMOD.Studio.EventInstance> Emit(IEnumerable<string> soundIds)
        {
            var soundInstances = new List<FMOD.Studio.EventInstance>();
            foreach (var soundId in soundIds)
            {
                var eventInstance = FMODUnity.RuntimeManager.CreateInstance(soundId);
                eventInstance.start();
                soundInstances.Add(eventInstance);
            }

            return soundInstances;
        }

        #endregion

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private List<SoundInfo> _soundMap = new List<SoundInfo>();

        #endregion
    }

    [System.Serializable]
    public struct SoundInfo
    {
        #region Fields

        public string name;
        public List<string> soundIds;

        #endregion
    }
}