using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    [RequireComponent(typeof(LightSource))]
    public class LightController : MonoBehaviour
    {
        #region Messages

        /// <summary>
        /// 
        /// </summary>
        private void Awake()
        {
            _lightSource = GetComponent<LightSource>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        public void ChangeColor(Color color)
        {
            _lightSource.color = color;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="interval"></param>
        /// <param name="intensity"></param>
        public void Flicker(float interval, float intensity)
        {
            if(_isFlickering)
            {
                return;
            }

            _coroutine = StartFlickering(interval, intensity);
            StartCoroutine(_coroutine);
        }

        /// <summary>
        /// 
        /// </summary>
        public void StopFlickering()
        {
            _isFlickering = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator WaitForFlicker()
        {
            if(_coroutine == null)
            {
                yield break;
            }
            yield return _coroutine;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="interval"></param>
        /// <param name="intensity"></param>
        /// <returns></returns>
        private IEnumerator StartFlickering(float interval, float intensity)
        {
            _isFlickering = true;

            var originalIntensity = this.intensity;

            while (_isFlickering)
            {
                yield return new WaitForSeconds(interval);
                _lightSource.intensity = intensity;

                yield return new WaitForSeconds(interval);
                _lightSource.intensity = originalIntensity;
            }

            _coroutine = null;
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public float intensity
        {
            get => _lightSource.intensity;
            set => _lightSource.intensity = value;
        }

        #endregion

        #region Fields

        private LightSource _lightSource;

        private IEnumerator _coroutine;

        private bool _isFlickering;

        #endregion
    }
}

