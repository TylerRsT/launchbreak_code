using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public static class AudioComponentExtensions
    {
        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="component"></param>
        /// <param name="soundKey"></param>
        /// <param name="mandatory"></param>
        public static IEnumerable<FMOD.Studio.EventInstance> EmitSound(this Component component, string soundKey, bool mandatory = true)
        {
            return EmitSound(component.gameObject, soundKey, mandatory);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="soundKey"></param>
        /// <param name="mandatory"></param>
        public static IEnumerable<FMOD.Studio.EventInstance> EmitSound(this GameObject gameObject, string soundKey, bool mandatory = true)
        {
            mandatory = false; // Temp, while we integrate sounds

            var soundEmitter = gameObject.GetComponent<SoundEmitter>();
            if (soundEmitter == null)
            {
                if (mandatory)
                {
                    Debug.LogError($"Could not find SoundEmitter component on object '{gameObject.name}'.");
                }
                return new FMOD.Studio.EventInstance[0];
            }
            return soundEmitter.Emit(soundKey, mandatory);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="component"></param>
        /// <param name="soundIds"></param>
        /// <param name="mandatory"></param>
        /// <returns></returns>
        public static IEnumerable<FMOD.Studio.EventInstance> EmitSound(this Component component, IEnumerable<string> soundIds, bool mandatory = true)
        {
            return EmitSound(component.gameObject, soundIds, mandatory);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="soundIds"></param>
        /// <param name="mandatory"></param>
        /// <returns></returns>
        public static IEnumerable<FMOD.Studio.EventInstance> EmitSound(this GameObject gameObject, IEnumerable<string> soundIds, bool mandatory = true)
        {
            mandatory = false; // Temp, while we integrate sounds

            var soundEmitter = gameObject.GetComponent<SoundEmitter>();
            if (soundEmitter == null)
            {
                if (mandatory)
                {
                    Debug.LogError($"Could not find SoundEmitter component on object '{gameObject.name}'.");
                }
                return new FMOD.Studio.EventInstance[0];
            }
            return soundEmitter.Emit(soundIds);
        }

        #endregion
    }
}