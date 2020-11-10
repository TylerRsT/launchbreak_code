using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public class PowerUpParticle : MonoBehaviour
    {
        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        public void SetTarget(Transform target)
        {
            _particleAttractor = GetComponent<particleAttractorLinear>();
            _particleSystem = GetComponent<ParticleSystem>();
            _particleAttractor.target = target;

            StartCoroutine(WaitDelay(0.3f, _particleAttractor));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="delay"></param>
        /// <param name="particleAttractor"></param>
        /// <returns></returns>
        private IEnumerator WaitDelay(float delay, particleAttractorLinear particleAttractor)
        {
            yield return new WaitForSeconds(delay);

            var pscollision = _particleSystem.collision;
            pscollision.enabled = true;
            particleAttractor.enabled = true;

            yield return new WaitUntil(() => _particleSystem.particleCount == 0);

            Destroy(gameObject);

        }

        #endregion

        #region Fields

        private particleAttractorLinear _particleAttractor;
        private ParticleSystem _particleSystem;

        #endregion
    }
}
