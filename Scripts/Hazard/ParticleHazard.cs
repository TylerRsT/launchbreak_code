using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    [RequireComponent(typeof(ParticleSystem))]
    public class ParticleHazard : RandomHazard
    {
        #region Override

        /// <summary>
        /// 
        /// </summary>
        public override void Play()
        {
            GetComponent<ParticleSystem>().Play();
            this.EmitSound(PlaySoundKey, false);
        }

        #endregion
    }
}