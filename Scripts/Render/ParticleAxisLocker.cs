using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    [ExecuteInEditMode]
    [RequireComponent(typeof(ParticleSystem))]
    public class ParticleAxisLocker : MonoBehaviour
    {
        #region Messages

        /// <summary>
        /// 
        /// </summary>
        private void Awake()
        {
            _particleSystem = GetComponent<ParticleSystem>();
        }

        /// <summary>
        /// 
        /// </summary>
        private void Update()
        {
            if(_particles == null)
            {
                _particles = new Particle[_maxParticleCount];
            }

            var count = _particleSystem.GetParticles(_particles, _maxParticleCount);
            for(var i = 0; i < count; ++i)
            {
                var velocity = _particles[i].velocity;
                velocity.z = 0.0f;

                _particles[i].velocity = velocity;
            }

            _particleSystem.SetParticles(_particles, count);
        }

        #endregion

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private int _maxParticleCount = 256;

        private Particle[] _particles;

        private ParticleSystem _particleSystem;

        #endregion
    }
}